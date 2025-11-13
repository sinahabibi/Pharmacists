using System.Security.Claims;

namespace Core.Interfaces
{
    public interface IAuthorizationService
    {
        Task<bool> UserHasPermissionAsync(int userId, string permissionName);
        Task<HashSet<string>> GetUserPermissionsAsync(int userId);
    }
}