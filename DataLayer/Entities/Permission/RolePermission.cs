using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities.Permission
{
    public class RolePermission
    {
        [Key]
        public int RolePermissionId { get; set; }

        public int RoleId { get; set; }

        public int PermissionId { get; set; }

        #region Relations

        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }

        #endregion
    }
}
