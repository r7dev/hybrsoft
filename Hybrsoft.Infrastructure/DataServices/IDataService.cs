using Hybrsoft.Infrastructure.Common;
using Hybrsoft.Infrastructure.Enums;
using Hybrsoft.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybrsoft.Infrastructure.DataServices
{
	public interface IDataService : IDisposable
	{
		#region Schema Universal
		bool HasPermission(long userId, string permissionName);

		IList<NavigationItem> GetNavigationItemByAppType(AppType appType);

		Task<AppLog> GetLogAsync(long id);
		Task<IList<AppLog>> GetLogsAsync(int skip, int take, DataRequest<AppLog> request);
		Task<IList<AppLog>> GetLogKeysAsync(int skip, int take, DataRequest<AppLog> request);
		Task<int> GetLogsCountAsync(DataRequest<AppLog> request);
		Task<int> CreateLogAsync(AppLog appLog);
		Task<int> DeleteLogsAsync(params AppLog[] logs);
		Task MarkAllAsReadAsync();

		Task<Permission> GetPermissionAsync(long id);
		Task<IList<Permission>> GetPermissionsAsync(int skip, int take, DataRequest<Permission> request);
		Task<IList<Permission>> GetPermissionKeysAsync(int skip, int take, DataRequest<Permission> request);
		Task<int> GetPermissionsCountAsync(DataRequest<Permission> request);
		Task<int> UpdatePermissionAsync(Permission permission);
		Task<int> DeletePermissionsAsync(params Permission[] permission);

		Task<Role> GetRoleAsync(long id);
		Task<IList<Role>> GetRolesAsync(int skip, int take, DataRequest<Role> request);
		Task<IList<Role>> GetRoleKeysAsync(int skip, int take, DataRequest<Role> request);
		Task<int> GetRolesCountAsync(DataRequest<Role> request);
		Task<int> UpdateRoleAsync(Role role);
		Task<int> DeleteRolesAsync(params Role[] roles);

		Task<RolePermission> GetRolePermissionAsync(long rolePermissionId);
		Task<IList<RolePermission>> GetRolePermissionsAsync(int skip, int take, DataRequest<RolePermission> request);
		Task<IList<RolePermission>> GetRolePermissionKeysAsync(int skip, int take, DataRequest<RolePermission> request);
		Task<int> GetRolePermissionsCountAsync(DataRequest<RolePermission> request);
		Task<int> UpdateRolePermissionAsync(RolePermission rolePermission);
		Task<int> DeleteRolePermissionsAsync(params RolePermission[] rolePermissions);

		Task<User> GetUserAsync(long id);
		Task<User> GetUserByEmailAsync(string email);
		Task<IList<User>> GetUsersAsync(int skip, int take, DataRequest<User> request);
		Task<IList<User>> GetUserKeysAsync(int skip, int take, DataRequest<User> request);
		Task<int> GetUsersCountAsync(DataRequest<User> request);
		Task<int> UpdateUserAsync(User user);
		Task<int> DeleteUsersAsync(params User[] users);

		Task<UserRole> GetUserRoleAsync(long userRoleId);
		Task<IList<UserRole>> GetUserRolesAsync(int skip, int take, DataRequest<UserRole> request);
		Task<IList<UserRole>> GetUserRoleKeysAsync(int skip, int take, DataRequest<UserRole> request);
		Task<int> GetUserRolesCountAsync(DataRequest<UserRole> request);
		Task<int> UpdateUserRoleAsync(UserRole userRole);
		Task<int> DeleteUserRolesAsync(params UserRole[] userRoles);
		#endregion

		#region Schema Learn
		Task<Student> GetStudentAsync(long id);
		Task<Student> GetStudentByEmailAsync(string email);
		Task<IList<Student>> GetStudentsAsync(int skip, int take, DataRequest<Student> request);
		Task<IList<Student>> GetStudentKeysAsync(int skip, int take, DataRequest<Student> request);
		Task<int> GetStudentsCountAsync(DataRequest<Student> request);
		Task<int> UpdateStudentAsync(Student Student);
		Task<int> DeleteStudentsAsync(params Student[] Students);
		#endregion
	}
}
