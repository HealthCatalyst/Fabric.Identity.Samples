using System.Threading.Tasks;

namespace Fabric.Identity.Samples.Mvc.Services
{
    public interface IFabricAuthorizationService
    {
        void SetAccessToken(string accessToken);

        Task<dynamic> CreatePermission(dynamic permission);

        Task<dynamic> CreatRole(dynamic role);

        Task AddPermissionToRole(dynamic permission, dynamic role);

        Task AddRoleToGroup(dynamic role, string groupName);

        Task<dynamic> GetGroupByName(string groupName);
    }
}
