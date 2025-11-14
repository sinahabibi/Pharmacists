# Permission System Documentation

## 📋 Overview

This project implements a comprehensive **Role-Based Access Control (RBAC)** system with advanced permission management for the Admin Panel.

---

## 🏗️ Architecture

### Entities

#### 1. **Permission**
Represents individual permissions in the system.

```csharp
public class Permission
{
    public int PermissionId { get; set; }
    public string PermissionName { get; set; }      // e.g., "Users.View"
    public string DisplayName { get; set; }         // e.g., "View Users"
    public string? Category { get; set; }           // e.g., "Users"
    public string? Description { get; set; }
}
```

#### 2. **Role**
Represents roles that can be assigned to users.

```csharp
public class Role
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }            // e.g., "SuperAdmin"
    public string DisplayName { get; set; }         // e.g., "Super Administrator"
    public string? Description { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsActive { get; set; }
}
```

#### 3. **UserRole**
Links users to their roles.

```csharp
public class UserRole
{
    public int UserRoleId { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedDate { get; set; }
    public int? AssignedBy { get; set; }
}
```

#### 4. **RolePermission**
Links roles to their permissions.

```csharp
public class RolePermission
{
    public int RolePermissionId { get; set; }
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}
```

---

## 🎯 Default Roles

### 1. **SuperAdmin**
- Full access to all features
- All 19 permissions

### 2. **CourseManager**
- Manage courses and enrollments
- Permissions:
  - Dashboard.View
  - Courses.View
  - Courses.Create
  - Courses.Edit
  - Courses.Delete
  - Courses.ManageEnrollments

### 3. **Accountant**
- Manage payments only
- Permissions:
  - Dashboard.View
  - Payments.View
  - Payments.Manage

---

## 🔐 Available Permissions

### Dashboard
- `Dashboard.View` - View Dashboard

### Courses
- `Courses.View` - View Courses
- `Courses.Create` - Create Course
- `Courses.Edit` - Edit Course
- `Courses.Delete` - Delete Course
- `Courses.ManageEnrollments` - Manage Enrollments

### Users
- `Users.View` - View Users
- `Users.Create` - Create User
- `Users.Edit` - Edit User
- `Users.Delete` - Delete User
- `Users.Ban` - Ban/Unban User
- `Users.AssignRoles` - Assign Roles to Users

### Roles
- `Roles.View` - View Roles
- `Roles.Create` - Create Role
- `Roles.Edit` - Edit Role
- `Roles.Delete` - Delete Role
- `Roles.AssignPermissions` - Assign Permissions to Role

### Payments
- `Payments.View` - View Payments
- `Payments.Manage` - Manage Payments

---

## 💻 Usage

### 1. Using Permission Attribute in Controllers

```csharp
[Area("Admin")]
[Authorize]
public class UsersController : Controller
{
    // Require "Users.View" permission to access this action
    [Permission("Users.View")]
    public async Task<IActionResult> Index()
    {
        // Your code here
    }

    // Require "Users.Create" permission
    [Permission("Users.Create")]
    public IActionResult Create()
    {
        return View();
    }

    // Require "Users.Edit" permission
    [Permission("Users.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        // Your code here
    }

    // Require "Users.Delete" permission
    [Permission("Users.Delete")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        // Your code here
    }
}
```

### 2. Using Permission Service Programmatically

```csharp
public class MyController : Controller
{
    private readonly IPermissionService _permissionService;

    public MyController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task<IActionResult> SomeAction()
    {
        int userId = GetCurrentUserId();

        // Check if user has specific permission
        bool hasPermission = await _permissionService
            .UserHasPermissionAsync(userId, "Users.Edit");

        if (!hasPermission)
        {
            return RedirectToAction("AccessDenied", "Dashboard", new { area = "Admin" });
        }

        // Get all user permissions
        var permissions = await _permissionService.GetUserPermissionsAsync(userId);

        // Get all user roles
        var roles = await _permissionService.GetUserRolesAsync(userId);

        // Check if user has specific role
        bool isSuperAdmin = await _permissionService
            .UserHasRoleAsync(userId, "SuperAdmin");

        return View();
    }
}
```

### 3. Assigning Roles to Users

```csharp
// Assign role to user
int userId = 5;
int roleId = 2; // CourseManager
int assignedBy = 1; // Admin user ID

bool success = await _permissionService
    .AssignRoleToUserAsync(userId, roleId, assignedBy);

// Remove role from user
bool removed = await _permissionService
    .RemoveRoleFromUserAsync(userId, roleId);
```

---

## 🚀 How It Works

### Authorization Flow

1. **User makes request** → Controller Action with `[Permission("SomePermission")]`

2. **PermissionFilter executes** → Checks authentication

3. **Gets User ID** → From Claims (`ClaimTypes.NameIdentifier`)

4. **Checks Permission** → Calls `IPermissionService.UserHasPermissionAsync()`

5. **Permission Service**:
   - Queries `UserRoles` for user's roles
   - Joins with `RolePermissions` to get permissions
   - Checks if required permission exists

6. **Result**:
   - ✅ **Has Permission** → Action executes
   - ❌ **No Permission** → Redirects to `AccessDenied` page

---

## 📁 File Structure

```
DataLayer/
├── Entities/
│   └── Permission/
│       ├── Permission.cs
│       ├── Role.cs
│       ├── RolePermission.cs
│       └── UserRole.cs
├── Context/
│   └── WebContext.cs

Core/
├── Interfaces/
│   └── IPermissionService.cs
└── Services/
    └── PermissionService.cs

Web/
├── Attributes/
│   └── PermissionAttribute.cs
└── Areas/Admin/
    ├── Controllers/
    │   ├── UsersController.cs
    │   ├── DashboardController.cs
    │   └── ...
    └── Views/
        └── Shared/
            └── AccessDenied.cshtml
```

---

## 🔧 Configuration

### Program.cs

```csharp
// Register Permission Service
builder.Services.AddScoped<IPermissionService, PermissionService>();
```

### Default Admin User

- **Username**: `admin`
- **Password**: `admin123`
- **Role**: SuperAdmin (all permissions)
- **Email**: `Admin@example.com`

---

## 📝 Adding New Permissions

### 1. Add Permission to Database Seed

In `WebContext.cs`:

```csharp
modelBuilder.Entity<Permission>().HasData(
    new Permission 
    { 
        PermissionId = 20, 
        PermissionName = "Settings.Edit", 
        DisplayName = "Edit Settings", 
        Category = "Settings" 
    }
);
```

### 2. Assign to Roles

```csharp
modelBuilder.Entity<RolePermission>().HasData(
    new RolePermission 
    { 
        RolePermissionId = 29, 
        RoleId = 1,      // SuperAdmin
        PermissionId = 20 // Settings.Edit
    }
);
```

### 3. Use in Controller

```csharp
[Permission("Settings.Edit")]
public async Task<IActionResult> EditSettings()
{
    // Your code
}
```

---

## 🛡️ Security Features

1. **Claim-Based Authentication** - Uses ASP.NET Core Claims
2. **Database-Driven** - All permissions stored in database
3. **Dynamic Permission Checking** - Real-time permission validation
4. **Role Hierarchy** - Users can have multiple roles
5. **Permission Aggregation** - Permissions from all roles combined
6. **Active Role Check** - Only active roles grant permissions

---

## 🎨 Access Denied Page

When a user tries to access a resource without permission:

- Redirects to: `/Admin/Dashboard/AccessDenied`
- Shows friendly error message
- Provides options:
  - Return to Home
  - Logout

---

## 📊 Database Schema

```
Users
  ↓
UserRoles (UserId, RoleId)
  ↓
Roles
  ↓
RolePermissions (RoleId, PermissionId)
  ↓
Permissions
```

---

## 🧪 Testing

### Test User Permissions

```csharp
[Fact]
public async Task User_Should_Have_SuperAdmin_Permissions()
{
    // Arrange
    var service = GetPermissionService();
    int userId = 1; // Admin user

    // Act
    bool hasPermission = await service
        .UserHasPermissionAsync(userId, "Users.Delete");

    // Assert
    Assert.True(hasPermission);
}
```

---

## 🚨 Common Issues

### Issue: Access Denied even with correct role

**Solution**: Check that:
1. Role is Active (`IsActive = true`)
2. RolePermission exists in database
3. User is authenticated
4. UserId in claims matches database

### Issue: Permission not working

**Solution**: 
1. Verify permission name matches exactly
2. Check database seeding ran correctly
3. Clear cookies and re-login

---

## 📚 References

- ASP.NET Core Authorization: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/
- Claims-Based Authorization: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/claims
- Role-Based Authorization: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles

---

## 👥 Contributors

- Permission System Design: Advanced RBAC Pattern
- Implementation: ASP.NET Core 8.0
- Database: Entity Framework Core with SQL Server

---

## 📄 License

This permission system is part of the main project and follows the same license.

---

## 🔄 Changelog

### Version 1.0 (Current)
- ✅ Basic RBAC implementation
- ✅ 3 default roles (SuperAdmin, CourseManager, Accountant)
- ✅ 19 permissions across 5 categories
- ✅ Custom Permission Attribute
- ✅ Permission Service with async operations
- ✅ Access Denied page
- ✅ Database seeding

### Future Enhancements
- 🔜 UI for managing roles and permissions
- 🔜 Permission caching for better performance
- 🔜 Activity logging for permission changes
- 🔜 Permission groups/modules
- 🔜 Time-based permissions
- 🔜 IP-based restrictions
