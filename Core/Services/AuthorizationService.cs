using Core.Interfaces;
using DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly WebContext _context;
        public AuthorizationService(WebContext context)
        {
            _context = context;
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permissionName)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => _context.RolePermissions.Where(rp => rp.RoleId == ur.RoleId))
                .AnyAsync(rp => _context.Permissions.Any(p => p.PermissionId == rp.PermissionId && p.PermissionName == permissionName));
        }

        public async Task<HashSet<string>> GetUserPermissionsAsync(int userId)
        {
            var query = from ur in _context.UserRoles
                        join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
                        join p in _context.Permissions on rp.PermissionId equals p.PermissionId
                        where ur.UserId == userId
                        select p.PermissionName;

            return (await query.Distinct().ToListAsync()).ToHashSet();
        }
    }
}