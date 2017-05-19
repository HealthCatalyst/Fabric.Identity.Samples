using System.Threading.Tasks;

namespace Fabric.Platform.Authorization
{
    public interface IAuthorizationClient
    {
        void SetAccessToken(string accessToken);

        Task<dynamic> CreatePermission(dynamic permission);

        Task<dynamic> GetPermission(string grain, string securableItem, string name);

        Task<dynamic> CreatRole(dynamic role);

        Task<dynamic> GetRole(string grain, string securableItem, string name);

        Task<bool> AddPermissionToRole(dynamic permission, dynamic role);

        Task<bool> AddRoleToGroup(dynamic role, string groupName);

        Task<dynamic> GetGroupByName(string groupName);

        Task<UserPermissions> GetPermissionsForUser(string grain, string securableItem);
    }
}
