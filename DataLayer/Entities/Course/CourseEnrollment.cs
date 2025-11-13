using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities.Course;

public class CourseEnrollment
{
    [Key]
    public int EnrollmentId { get; set; }

    [ForeignKey(nameof(Course))]
    public int CourseId { get; set; }
    public Course Course { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public DataLayer.Entities.User.User User { get; set; }

    public DateTime EnrollmentDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletionDate { get; set; }
    public int ProgressPercentage { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaymentDate { get; set; }
    public decimal? PaymentAmount { get; set; }
}