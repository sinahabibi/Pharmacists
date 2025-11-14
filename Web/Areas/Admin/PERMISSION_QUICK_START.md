# Quick Start Guide - Permission System

## 🚀 Getting Started

### Step 1: Login as Admin
```
Username: admin
Password: admin123
```

The admin user has **SuperAdmin** role with full access to all features.

---

## 🎯 Common Tasks

### 1. Protect a Controller Action

```csharp
using Web.Attributes;

[Area("Admin")]
[Authorize]
public class MyController : Controller
{
    // Requires "Users.View" permission
    [Permission("Users.View")]
    public async Task<IActionResult> Index()
    {
        // Your code
    }

    // Requires "Users.Create" permission
    [Permission("Users.Create")]
    public IActionResult Create()
    {
        return View();
    }

    // Multiple actions can have different permissions
    [Permission("Users.Delete")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        // Your code
    }
}
```

### 2. Check Permission Programmatically

```csharp
using Core.Interfaces;

public class MyController : Controller
{
    private readonly IPermissionService _permissionService;

    public MyController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task<IActionResult> MyAction()
    {
        // Get current user ID
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        int userId = int.Parse(userIdClaim.Value);

        // Check permission
        bool canDelete = await _permissionService
            .UserHasPermissionAsync(userId, "Users.Delete");

        if (canDelete)
        {
            // Show delete button
        }

        return View();
    }
}
```

### 3. Check in Razor Views

```razor
@inject Core.Interfaces.IPermissionService PermissionService

@{
    var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
    var canCreate = await PermissionService.UserHasPermissionAsync(userId, "Users.Create");
    var canDelete = await PermissionService.UserHasPermissionAsync(userId, "Users.Delete");
}

@if (canCreate)
{
    <a asp-action="Create" class="btn btn-primary">
        <i class="fas fa-plus"></i> Create New User
    </a>
}

@if (canDelete)
{
    <button onclick="deleteUser(@user.UserId)" class="btn btn-danger">
        <i class="fas fa-trash"></i> Delete
    </button>
}
```

---

## 📋 Available Permissions

Copy-paste these permission names when using `[Permission("...")]`:

### Dashboard
- `Dashboard.View`

### Users
- `Users.View`
- `Users.Create`
- `Users.Edit`
- `Users.Delete`
- `Users.Ban`
- `Users.AssignRoles`

### Roles
- `Roles.View`
- `Roles.Create`
- `Roles.Edit`
- `Roles.Delete`
- `Roles.AssignPermissions`

### Courses
- `Courses.View`
- `Courses.Create`
- `Courses.Edit`
- `Courses.Delete`
- `Courses.ManageEnrollments`

### Payments
- `Payments.View`
- `Payments.Manage`

---

## 🔐 Default Roles

### SuperAdmin
- All permissions ✅
- Cannot be deleted or deactivated
- Default admin user has this role

### CourseManager
- Dashboard.View
- All Courses permissions

### Accountant
- Dashboard.View
- All Payments permissions

---

## 💡 Examples

### Example 1: Simple Protection

```csharp
[Area("Admin")]
[Authorize]
public class ProductsController : Controller
{
    [Permission("Products.View")]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);
    }
}
```

### Example 2: Different Permissions for GET/POST

```csharp
[Permission("Settings.View")]
public IActionResult Edit()
{
    var settings = GetSettings();
    return View(settings);
}

[HttpPost]
[ValidateAntiForgeryToken]
[Permission("Settings.Edit")]  // Different permission!
public async Task<IActionResult> Edit(SettingsViewModel model)
{
    // Save settings
    return RedirectToAction(nameof(Index));
}
```

### Example 3: Service Usage

```csharp
public class UserManagementService
{
    private readonly IPermissionService _permissionService;

    public async Task<bool> CanUserManageOtherUsers(int userId)
    {
        // Check if user has any user management permission
        var hasView = await _permissionService
            .UserHasPermissionAsync(userId, "Users.View");
        
        var hasEdit = await _permissionService
            .UserHasPermissionAsync(userId, "Users.Edit");

        return hasView || hasEdit;
    }

    public async Task<List<string>> GetUserCapabilities(int userId)
    {
        // Get all permissions for the user
        return await _permissionService.GetUserPermissionsAsync(userId);
    }
}
```

---

## 🛠️ Troubleshooting

### "Access Denied" error

**Possible causes:**
1. User doesn't have the required permission
2. User is not authenticated
3. Permission name is misspelled
4. Role is not active

**Solution:**
1. Check user's roles in database (UserRoles table)
2. Verify role has the permission (RolePermissions table)
3. Ensure role is active (Roles.IsActive = true)

### Permission not working after adding

**Solution:**
1. Log out and log back in (claims are cached)
2. Check database seeding ran correctly
3. Verify permission name matches exactly (case-sensitive)

---

## 📖 Best Practices

### 1. Use Descriptive Permission Names
```csharp
✅ Good: [Permission("Users.Edit")]
❌ Bad:  [Permission("edit_user")]
```

### 2. Follow Naming Convention
```
Category.Action
Examples:
- Users.View
- Products.Create
- Reports.Download
```

### 3. Group Related Permissions
```csharp
// All actions in UsersController use Users.* permissions
[Area("Admin")]
public class UsersController : Controller
{
    [Permission("Users.View")]
    public async Task<IActionResult> Index() { }

    [Permission("Users.Create")]
    public IActionResult Create() { }

    [Permission("Users.Edit")]
    public async Task<IActionResult> Edit(int id) { }
}
```

### 4. Cache Permission Checks (Future Enhancement)
```csharp
// Consider caching for frequently checked permissions
private async Task<bool> CanEditUsers(int userId)
{
    var cacheKey = $"perm_{userId}_Users.Edit";
    
    if (_cache.TryGetValue(cacheKey, out bool hasPermission))
        return hasPermission;

    hasPermission = await _permissionService
        .UserHasPermissionAsync(userId, "Users.Edit");

    _cache.Set(cacheKey, hasPermission, TimeSpan.FromMinutes(5));
    
    return hasPermission;
}
```

---

## 🎓 Learning Resources

1. **Permission System Docs**: See `PERMISSION_SYSTEM.md` for complete documentation
2. **Example Controllers**: Check `UsersController.cs` for usage examples
3. **Database Schema**: Review migrations for table structure

---

## 💬 Need Help?

If you encounter issues:

1. Check the full documentation: `PERMISSION_SYSTEM.md`
2. Review example implementations in existing controllers
3. Verify database seeding completed successfully
4. Check application logs for detailed error messages

---

## ✅ Checklist for Adding New Protected Feature

- [ ] Create controller action
- [ ] Add `[Permission("Category.Action")]` attribute
- [ ] Add permission to database seed if not exists
- [ ] Assign permission to appropriate roles
- [ ] Test with different user roles
- [ ] Update UI to show/hide based on permissions
- [ ] Document the new permission

---

**Happy Coding! 🚀**
