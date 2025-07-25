﻿using Hybrsoft.Infrastructure.Common;
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
		IList<NavigationItem> GetNavigationItemByAppType(AppType appType);
		bool HasPermission(long userID, string permissionName);

		Task<IList<Country>> GetCountriesAsync();
		Task<IList<SubscriptionPlan>> GetSubscriptionPlansAsync();

		Task<AppLog> GetAppLogAsync(long id);
		Task<IList<AppLog>> GetAppLogsAsync(int skip, int take, DataRequest<AppLog> request);
		Task<IList<AppLog>> GetAppLogKeysAsync(int skip, int take, DataRequest<AppLog> request);
		Task<int> GetAppLogsCountAsync(DataRequest<AppLog> request);
		Task<int> CreateAppLogAsync(AppLog entity);
		Task<int> DeleteAppLogsAsync(params AppLog[] entities);
		Task MarkAllAsReadAsync();

		Task<Company> GetCompanyAsync(long id);
		Task<IList<Company>> GetCompaniesAsync(int skip, int take, DataRequest<Company> request);
		Task<IList<Company>> GetCompanyKeysAsync(int skip, int take, DataRequest<Company> request);
		Task<int> GetCompaniesCountAsync(DataRequest<Company> request);
		Task<int> UpdateCompanyAsync(Company entity);
		Task<int> DeleteCompaniesAsync(params Company[] entities);

		Task<CompanyUser> GetCompanyUserAsync(long id);
		Task<IList<CompanyUser>> GetCompanyUsersAsync(int skip, int take, DataRequest<CompanyUser> request);
		Task<IList<CompanyUser>> GetCompanyUserKeysAsync(int skip, int take, DataRequest<CompanyUser> request);
		Task<IList<long>> GetAddedUserKeysInCompanyAsync(long parentID);
		Task<int> GetCompanyUsersCountAsync(DataRequest<CompanyUser> request);
		Task<int> UpdateCompanyUserAsync(CompanyUser entity);
		Task<int> DeleteCompanyUsersAsync(params CompanyUser[] entities);

		Task<Permission> GetPermissionAsync(long id);
		Task<IList<Permission>> GetPermissionsAsync(int skip, int take, DataRequest<Permission> request);
		Task<IList<Permission>> GetPermissionKeysAsync(int skip, int take, DataRequest<Permission> request);
		Task<int> GetPermissionsCountAsync(DataRequest<Permission> request);
		Task<int> UpdatePermissionAsync(Permission entity);
		Task<int> DeletePermissionsAsync(params Permission[] entities);

		Task<Role> GetRoleAsync(long id);
		Task<IList<Role>> GetRolesAsync(int skip, int take, DataRequest<Role> request);
		Task<IList<Role>> GetRoleKeysAsync(int skip, int take, DataRequest<Role> request);
		Task<int> GetRolesCountAsync(DataRequest<Role> request);
		Task<int> UpdateRoleAsync(Role entity);
		Task<int> DeleteRolesAsync(params Role[] entities);

		Task<RolePermission> GetRolePermissionAsync(long id);
		Task<IList<RolePermission>> GetRolePermissionsAsync(int skip, int take, DataRequest<RolePermission> request);
		Task<IList<RolePermission>> GetRolePermissionKeysAsync(int skip, int take, DataRequest<RolePermission> request);
		Task<IList<long>> GetAddedPermissionKeysInRoleAsync(long parentID);
		Task<int> GetRolePermissionsCountAsync(DataRequest<RolePermission> request);
		Task<int> UpdateRolePermissionAsync(RolePermission entity);
		Task<int> DeleteRolePermissionsAsync(params RolePermission[] entities);

		Task<Subscription> GetSubscriptionAsync(long id);
		Task<IList<Subscription>> GetSubscriptionsAsync(int skip, int take, DataRequest<Subscription> request);
		Task<IList<Subscription>> GetSubscriptionKeysAsync(int skip, int take, DataRequest<Subscription> request);
		Task<int> GetSubscriptionsCountAsync(DataRequest<Subscription> request);
		Task<int> UpdateSubscriptionAsync(Subscription entity);
		Task<int> DeleteSubscriptionsAsync(params Subscription[] entities);

		Task<User> GetUserAsync(long id);
		Task<User> GetUserByEmailAsync(string email);
		Task<IList<User>> GetUsersAsync(int skip, int take, DataRequest<User> request);
		Task<IList<User>> GetUserKeysAsync(int skip, int take, DataRequest<User> request);
		Task<int> GetUsersCountAsync(DataRequest<User> request);
		Task<int> UpdateUserAsync(User entity);
		Task<int> DeleteUsersAsync(params User[] entities);

		Task<UserRole> GetUserRoleAsync(long id);
		Task<IList<UserRole>> GetUserRolesAsync(int skip, int take, DataRequest<UserRole> request);
		Task<IList<UserRole>> GetUserRoleKeysAsync(int skip, int take, DataRequest<UserRole> request);
		Task<IList<long>> GetAddedRoleKeysInUserAsync(long parentID);
		Task<int> GetUserRolesCountAsync(DataRequest<UserRole> request);
		Task<int> UpdateUserRoleAsync(UserRole entity);
		Task<int> DeleteUserRolesAsync(params UserRole[] entities);
		#endregion

		#region Schema Learn
		Task<IList<ScheduleType>> GetScheduleTypesAsync();
		Task<IList<RelativeType>> GetRelativeTypesAsync();

		Task<Student> GetStudentAsync(long id);
		Task<IList<Student>> GetStudentsAsync(int skip, int take, DataRequest<Student> request);
		Task<IList<Student>> GetStudentKeysAsync(int skip, int take, DataRequest<Student> request);
		Task<int> GetStudentsCountAsync(DataRequest<Student> request);
		Task<int> UpdateStudentAsync(Student entity);
		Task<int> DeleteStudentsAsync(params Student[] entities);

		Task<StudentRelative> GetStudentRelativeAsync(long id);
		Task<IList<StudentRelative>> GetStudentRelativesAsync(int skip, int take, DataRequest<StudentRelative> request);
		Task<IList<StudentRelative>> GetStudentRelativeKeysAsync(int skip, int take, DataRequest<StudentRelative> request);
		Task<IList<long>> GetAddedRelativeKeysInStudentAsync(long parentID);
		Task<int> GetStudentRelativesCountAsync(DataRequest<StudentRelative> request);
		Task<int> UpdateStudentRelativeAsync(StudentRelative entity);
		Task<int> DeleteStudentRelativesAsync(params StudentRelative[] entities);

		Task<Classroom> GetClassroomAsync(long id);
		Task<IList<Classroom>> GetClassroomsAsync(int skip, int take, DataRequest<Classroom> request);
		Task<IList<Classroom>> GetClassroomKeysAsync(int skip, int take, DataRequest<Classroom> request);
		Task<int> GetClassroomsCountAsync(DataRequest<Classroom> request);
		Task<int> UpdateClassroomAsync(Classroom entity);
		Task<int> DeleteClassroomsAsync(params Classroom[] entities);

		Task<ClassroomStudent> GetClassroomStudentAsync(long id);
		Task<IList<ClassroomStudent>> GetClassroomStudentsAsync(int skip, int take, DataRequest<ClassroomStudent> request);
		Task<IList<ClassroomStudent>> GetClassroomStudentKeysAsync(int skip, int take, DataRequest<ClassroomStudent> request);
		Task<IList<long>> GetAddedStudentKeysInClassroomAsync(long parentID);
		Task<int> GetClassroomStudentsCountAsync(DataRequest<ClassroomStudent> request);
		Task<int> UpdateClassroomStudentAsync(ClassroomStudent entity);
		Task<int> DeleteClassroomStudentsAsync(params ClassroomStudent[] entities);

		Task<Relative> GetRelativeAsync(long id);
		Task<IList<Relative>> GetRelativesAsync(int skip, int take, DataRequest<Relative> request);
		Task<IList<Relative>> GetRelativeKeysAsync(int skip, int take, DataRequest<Relative> request);
		Task<int> GetRelativesCountAsync(DataRequest<Relative> request);
		Task<int> UpdateRelativeAsync(Relative entity);
		Task<int> DeleteRelativesAsync(params Relative[] entities);

		Task<IList<ClassroomStudent>> GetDismissibleStudentsAsync(int skip, int take, DataRequest<ClassroomStudent> request);
		Task<IList<ClassroomStudent>> GetDismissibleStudentKeysAsync(int skip, int take, DataRequest<ClassroomStudent> request);
		Task<int> GetDismissibleStudentsCountAsync(DataRequest<ClassroomStudent> request);
		Task<Dismissal> GetDismissalAsync(long id);
		Task<IList<Dismissal>> GetDismissalsAsync(int skip, int take, DataRequest<Dismissal> request);
		Task<IList<Dismissal>> GetDismissalKeysAsync(int skip, int take, DataRequest<Dismissal> request);
		Task<int> GetDismissalsCountAsync(DataRequest<Dismissal> request);
		Task<int> UpdateDismissalAsync(Dismissal entity);
		Task<int> ApproveDismissalsAsync(params Dismissal[] entities);
		#endregion
	}
}
