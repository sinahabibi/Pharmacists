using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities.Post
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Display(Name = "عنوان")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        [MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        public string Description { get; set; }

        [Display(Name = "توضیحات متا")]
        [MaxLength(160, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        public string MetaDescription { get; set; }

        [Display(Name = "عکس")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        public string ImageName { get; set; }

        [Display(Name = "پست")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        public string Body { get; set; }

        [Display(Name = "کلمات کلیدی")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        public string Tags { get; set; }

        [MaxLength(20, ErrorMessage = "{0} نمیتواند بیشتر از {1} کارکتر باشد")]
        public string? ShortKey { get; set; }

        public int Visit { get; set; }

        public bool IsPublish { get; set; }

        public DateTime CreateDate { get; set; }

        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public virtual User.User User { get; set; }

        public virtual List<PostComment> PostComments { get; set; }

    }
}
