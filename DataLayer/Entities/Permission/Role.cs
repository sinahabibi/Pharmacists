using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities.Permission
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; }

        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreateDate { get; set; }

        public bool IsActive { get; set; }

        #region Relations

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        #endregion
    }
}
