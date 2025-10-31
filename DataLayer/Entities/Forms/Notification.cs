using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities.Forms
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Title { get; set; }

        [Display(Name = "پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Message { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreateDate { get; set; }

        public bool IsSeen { get; set; }
    }
}
