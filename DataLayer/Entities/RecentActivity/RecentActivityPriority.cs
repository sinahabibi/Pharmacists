using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities.RecentActivity
{
    public class RecentActivityPriority
    {
        [Key]
        public int PriorityId { get; set; }
        public string Title { get; set; }
        public virtual IEnumerable<RecentActivity> RecentActivities { get; set; }
    }
}
