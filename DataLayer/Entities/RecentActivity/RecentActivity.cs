using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities.RecentActivity
{
    public class RecentActivity
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public int PriorityId { get; set; }
        [ForeignKey("PriorityId")]
        public virtual RecentActivityPriority RecentActivityPriority { get; set; }
    }
}
