using DataLayer.Entities;
using DataLayer.Entities.Course;
using DataLayer.Entities.Email;
using DataLayer.Entities.Permission;
using DataLayer.Entities.Post;
using DataLayer.Entities.RecentActivity;
using DataLayer.Entities.User;
using DataLayer.Entities.UserTraker;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
    public class WebContext : DbContext
    {
        public WebContext(DbContextOptions<WebContext> options) : base(options)
        {

        }

        #region User

        public DbSet<User> Users { get; set; }

        #endregion

        #region Post

        public DbSet<Post>? Posts { get; set; }

        #endregion

        #region Link

        public DbSet<Link> Links { get; set; }
        public DbSet<Visitor> Visitors { get; set; }

        #endregion

        #region RecentActivity

        public DbSet<RecentActivity> RecentActivities { get; set; }
        public DbSet<RecentActivityPriority> RecentActivityPriorities { get; set; }

        #endregion

        #region Email

        public DbSet<EmailAccount> Emails { get; set; }

        #endregion

        #region Permission & Role

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        #endregion

        #region Course

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }

        #endregion

        public DbSet<Setting> Settings { get; set; }
        public DbSet<Sms> Sms { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => u.IsDelete == false);

            // اطمینان از یکتا بودن UserName
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            // اطمینان از یکتا بودن PhoneNumber
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber)
                .IsUnique();

            // اطمینان از یکتا بودن Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            #region All User Data

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    UserId = 1,
                    FirstName = "Admin",
                    LastName = "",
                    ActiveCode = "811b78fab121434b8e20b2b263e6bd7e",
                    ActivePhoneNumberCode = "12345",
                    Email = "Admin@example.com",
                    IsDelete = false,
                    IsEmailActive = true,
                    IsPhoneNumberActive = true,
                    LastChange = new DateTime(2024, 1, 1),
                    PhoneNumber = "09xxxxxxxxx",
                    RegisterDate = new DateTime(2024, 1, 1),
                    SecurityCode = "40c87d0a7d83413ab8e8c59229e8949e",
                    Password = "admin123",
                    UserName = "admin"
                });

            #endregion

            #region Settings Data

            modelBuilder.Entity<Setting>().HasData(
                new Setting()
                {
                    Id = 1,
                    Title = "نام سایت",
                    DataString = "فروشگاه",
                    Changeable = true
                },
                new Setting()
                {
                    Id = 2,
                    Title = "توضیحات سایت",
                    Description = "شما میتوانید در توضیحات متا در صفحه اصلی ببنید",
                    DataString = "این سایت برای مدریت و فروش بهتر شما اینجاست",
                    Changeable = true
                });

            #endregion

            #region RecentActivity

            modelBuilder.Entity<RecentActivityPriority>().HasData(
                new RecentActivityPriority()
                {
                    PriorityId = 1,
                    Title = "زیاد"
                },
                new RecentActivityPriority()
                {
                    PriorityId = 2,
                    Title = "متوسط"
                },
                new RecentActivityPriority()
                {
                    PriorityId = 3,
                    Title = "کم"
                });

            #endregion

            #region Permissions Data

            modelBuilder.Entity<Permission>().HasData(
                // Dashboard
                new Permission { PermissionId = 1, PermissionName = "Dashboard.View", DisplayName = "View Dashboard", Category = "Dashboard" },
                
                // Courses
                new Permission { PermissionId = 2, PermissionName = "Courses.View", DisplayName = "View Courses", Category = "Courses" },
                new Permission { PermissionId = 3, PermissionName = "Courses.Create", DisplayName = "Create Course", Category = "Courses" },
                new Permission { PermissionId = 4, PermissionName = "Courses.Edit", DisplayName = "Edit Course", Category = "Courses" },
                new Permission { PermissionId = 5, PermissionName = "Courses.Delete", DisplayName = "Delete Course", Category = "Courses" },
                new Permission { PermissionId = 6, PermissionName = "Courses.ManageEnrollments", DisplayName = "Manage Enrollments", Category = "Courses" },
                
                // Users
                new Permission { PermissionId = 7, PermissionName = "Users.View", DisplayName = "View Users", Category = "Users" },
                new Permission { PermissionId = 8, PermissionName = "Users.Create", DisplayName = "Create User", Category = "Users" },
                new Permission { PermissionId = 9, PermissionName = "Users.Edit", DisplayName = "Edit User", Category = "Users" },
                new Permission { PermissionId = 10, PermissionName = "Users.Delete", DisplayName = "Delete User", Category = "Users" },
                new Permission { PermissionId = 11, PermissionName = "Users.Ban", DisplayName = "Ban/Unban User", Category = "Users" },
                
                // Roles
                new Permission { PermissionId = 12, PermissionName = "Roles.View", DisplayName = "View Roles", Category = "Roles" },
                new Permission { PermissionId = 13, PermissionName = "Roles.Create", DisplayName = "Create Role", Category = "Roles" },
                new Permission { PermissionId = 14, PermissionName = "Roles.Edit", DisplayName = "Edit Role", Category = "Roles" },
                new Permission { PermissionId = 15, PermissionName = "Roles.Delete", DisplayName = "Delete Role", Category = "Roles" },
                new Permission { PermissionId = 16, PermissionName = "Roles.AssignPermissions", DisplayName = "Assign Permissions to Role", Category = "Roles" },
                new Permission { PermissionId = 17, PermissionName = "Users.AssignRoles", DisplayName = "Assign Roles to Users", Category = "Users" },
                
                // Payments
                new Permission { PermissionId = 18, PermissionName = "Payments.View", DisplayName = "View Payments", Category = "Payments" },
                new Permission { PermissionId = 19, PermissionName = "Payments.Manage", DisplayName = "Manage Payments", Category = "Payments" }
            );

            #endregion

            #region Roles Data

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = 1,
                    RoleName = "SuperAdmin",
                    DisplayName = "Super Administrator",
                    Description = "Full access to all features",
                    CreateDate = new DateTime(2024, 1, 1),
                    IsActive = true
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "CourseManager",
                    DisplayName = "Course Manager",
                    Description = "Manage courses and enrollments",
                    CreateDate = new DateTime(2024, 1, 1),
                    IsActive = true
                },
                new Role
                {
                    RoleId = 3,
                    RoleName = "Accountant",
                    DisplayName = "Accountant",
                    Description = "Manage payments only",
                    CreateDate = new DateTime(2024, 1, 1),
                    IsActive = true
                }
            );

            #endregion

            #region RolePermissions Data

            // SuperAdmin - All permissions
            var superAdminPermissions = new List<RolePermission>();
            for (int i = 1; i <= 19; i++)
            {
                superAdminPermissions.Add(new RolePermission
                {
                    RolePermissionId = i,
                    RoleId = 1,
                    PermissionId = i
                });
            }

            // CourseManager - Course and Dashboard permissions
            var courseManagerPermissions = new List<RolePermission>
            {
                new RolePermission { RolePermissionId = 20, RoleId = 2, PermissionId = 1 }, // Dashboard.View
                new RolePermission { RolePermissionId = 21, RoleId = 2, PermissionId = 2 }, // Courses.View
                new RolePermission { RolePermissionId = 22, RoleId = 2, PermissionId = 3 }, // Courses.Create
                new RolePermission { RolePermissionId = 23, RoleId = 2, PermissionId = 4 }, // Courses.Edit
                new RolePermission { RolePermissionId = 24, RoleId = 2, PermissionId = 5 }, // Courses.Delete
                new RolePermission { RolePermissionId = 25, RoleId = 2, PermissionId = 6 }  // Courses.ManageEnrollments
            };

            // Accountant - Payment and Dashboard permissions
            var accountantPermissions = new List<RolePermission>
            {
                new RolePermission { RolePermissionId = 26, RoleId = 3, PermissionId = 1 },  // Dashboard.View
                new RolePermission { RolePermissionId = 27, RoleId = 3, PermissionId = 18 }, // Payments.View
                new RolePermission { RolePermissionId = 28, RoleId = 3, PermissionId = 19 }  // Payments.Manage
            };

            modelBuilder.Entity<RolePermission>().HasData(
                superAdminPermissions.Concat(courseManagerPermissions).Concat(accountantPermissions)
            );

            #endregion

            #region UserRoles Data

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    UserRoleId = 1,
                    UserId = 1,
                    RoleId = 1,
                    AssignedDate = new DateTime(2024, 1, 1)
                }
            );

            #endregion

            base.OnModelCreating(modelBuilder);
        }

    }
}
