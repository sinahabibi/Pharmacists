using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities.Course
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string? ImageName { get; set; }

        public decimal Price { get; set; }

        public int? DurationHours { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? LastUpdate { get; set; }

        #region Relations

        public virtual ICollection<CourseEnrollment> Enrollments { get; set; }

        #endregion
    }
}
