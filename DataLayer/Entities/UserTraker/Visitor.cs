using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities.UserTraker
{
    public class Visitor
    {
        public Visitor()
        {
            Links = new List<Link>();
        }

        public int Id { get; set; }

        [MaxLength(50)]
        public string DeviceType { get; set; }

        [MaxLength(100)]
        public string Browser { get; set; }

        [MaxLength(100)]
        public string PlatformName { get; set; }

        [MaxLength(45)]
        public string Ip { get; set; }

        [MaxLength(128)]
        public string UserIdKey { get; set; }

        public DateTime FirstVisit { get; set; }

        public bool IsBan { get; set; }

        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User.User? User { get; set; }

        // Changed property type to ICollection and initialized in constructor
        public virtual ICollection<Link> Links { get; set; }
    }
}
