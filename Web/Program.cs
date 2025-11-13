using Core.Interfaces;
using Core.Services;
using DataLayer.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Web.Authorization;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add DbContext
            builder.Services.AddDbContext<WebContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.SaveTokens = true;
            });

            // Register DI Services
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IUserService, UserService>();

            // RBAC services
            builder.Services.AddScoped<Core.Interfaces.IAuthorizationService, AuthorizationService>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin.Dashboard.View", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Dashboard.View")));

                options.AddPolicy("Admin.Courses.View", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Courses.View")));
                options.AddPolicy("Admin.Courses.Create", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Courses.Create")));
                options.AddPolicy("Admin.Courses.Edit", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Courses.Edit")));
                options.AddPolicy("Admin.Courses.Delete", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Courses.Delete")));
                options.AddPolicy("Admin.Courses.ManageEnrollments", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Courses.ManageEnrollments")));

                options.AddPolicy("Admin.Users.View", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Users.View")));
                options.AddPolicy("Admin.Users.Create", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Users.Create")));
                options.AddPolicy("Admin.Users.Edit", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Users.Edit")));
                options.AddPolicy("Admin.Users.Delete", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Users.Delete")));
                options.AddPolicy("Admin.Users.Ban", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Users.Ban")));
                options.AddPolicy("Admin.Users.AssignRoles", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Users.AssignRoles")));

                options.AddPolicy("Admin.Roles.View", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Roles.View")));
                options.AddPolicy("Admin.Roles.Create", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Roles.Create")));
                options.AddPolicy("Admin.Roles.Edit", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Roles.Edit")));
                options.AddPolicy("Admin.Roles.Delete", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Roles.Delete")));
                options.AddPolicy("Admin.Roles.AssignPermissions", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Roles.AssignPermissions")));

                options.AddPolicy("Admin.Payments.View", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Payments.View")));
                options.AddPolicy("Admin.Payments.Manage", policy =>
                    policy.Requirements.Add(new PermissionRequirement("Payments.Manage")));
            });

            // Add Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Area routing
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
