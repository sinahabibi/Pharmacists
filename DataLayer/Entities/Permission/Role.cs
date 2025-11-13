using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities.Permission
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required, MaxLength(100)]
        public string RoleName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string DisplayName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreateDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}