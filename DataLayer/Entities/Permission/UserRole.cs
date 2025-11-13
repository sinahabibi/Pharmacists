using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Entities.Permission;

public class UserRole
{
    [Key]
    public int UserRoleId { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public DataLayer.Entities.User.User User { get; set; }

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }
    public Role Role { get; set; }

    public DateTime AssignedDate { get; set; }
    public int? AssignedBy { get; set; }
}