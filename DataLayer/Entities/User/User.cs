using DataLayer.Entities.UserTraker;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities.User
{
    public class User
    {

        [Key]
        public int UserId { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string UserName { get; set; }

        [Display(Name = "نام")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string? LastName { get; set; }

        [Display(Name = "ایمیل")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        public string? Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Password { get; set; }


        [Display(Name = "تاریخ آخرین ورود")]
        public DateTime? LastLoginDate { get; set; }
        [Display(Name = "دفعات تلاش")]
        public int TryCount { get; set; }

        [Display(Name = "تاریخ آخرین تلاش")]
        public DateTime? LastTry { get; set; }

        [Display(Name = "اعمال محدودیت")]
        public bool IsBan { get; set; }


        [Display(Name = "کد فعال سازی")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string ActiveCode { get; set; }

        [Display(Name = "کد امنیتی")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string SecurityCode { get; set; }

        public bool IsPhoneNumberActive { get; set; }
        public bool IsEmailActive { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        [MaxLength(13, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        public string PhoneNumber { get; set; }

        [Display(Name = "اخیرین تغییرات")]
        public DateTime LastChange { get; set; }

        [Display(Name = "کد فعال سازی شماره تلفن")]
        [MaxLength(10, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        public string ActivePhoneNumberCode { get; set; }

        [Display(Name = "تاریخ ثبت نام")]
        public DateTime RegisterDate { get; set; }

        [Display(Name = "حذف شده")]
        public bool IsDelete { get; set; }
        
        #region Relations

        public virtual IEnumerable<Post.Post> Posts { get; set; }
        public virtual IEnumerable<Visitor> Visitors { get; set; }
        #endregion

    }
}
