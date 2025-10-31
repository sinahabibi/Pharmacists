using DataLayer.Entities;
using DataLayer.Entities.Email;
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
                    FirstName = "ادمین",
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

            base.OnModelCreating(modelBuilder);
        }

    }
}
