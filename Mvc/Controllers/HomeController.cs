using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Fabric.Identity.Samples.Mvc.Services;
using Fabric.Platform.Http;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Fabric.Identity.Samples.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFabricAuthorizationService _fabricAuthorizationService;
        public HomeController(IHttpClientFactory httpClientFactory, IFabricAuthorizationService authorizationService)
        {
            _httpClientFactory = httpClientFactory;
            _fabricAuthorizationService = authorizationService ??
                                    throw new ArgumentNullException(nameof(authorizationService));
        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Patient()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        
        public IActionResult Error()
        {
            return View();
        }

        public async Task Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            await HttpContext.Authentication.SignOutAsync("oidc");
        }

        public async Task<IActionResult> CallApiUsingClientCredentials()
        {
            try
            {
                var tokenClient = new TokenClient("http://localhost:5001/connect/token", "fabric-mvcsample", "secret");
                var tokenResponse = await tokenClient.RequestClientCredentialsAsync("patientapi");
                return await CallApiWithToken(tokenResponse.AccessToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task<IActionResult> CallApiUsingUserAccessToken()
        {
            try
            {
                var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");
                return await CallApiWithToken(accessToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task<IActionResult> SetupRolesAndPermissions()
        {
            var viewerGroup = await SetupGroup(
                new {Grain = "app", SecurableItem = "fabric-mvcsample", Name = "viewpatient"},
                new {Grain = "app", SecurableItem = "fabric-mvcsample", Name = "viewer"}, "Health Catalyst Viewer").Result;

            var editorGroup = await SetupGroup(
                new {Grain = "app", SecurableItem = "fabric-mvcsample", Name = "editpatient"},
                new {Grain = "app", SecurableItem = "fabric-mvcsample", Name = "editor"}, "Health Catalyst Editor").Result;

            ViewBag.CreatedGroups = new List<dynamic> { viewerGroup, editorGroup };
            return View("Json");
        }

        public async Task<IActionResult> GetPermissionsForUser()
        {
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");
            _fabricAuthorizationService.SetAccessToken(accessToken);
            var permissions = _fabricAuthorizationService.GetPermissionsForUser("app", "fabric-mvcsample").Result;
            ViewBag.UserPermissions = permissions;
            return View("Json");
        }

        private async Task<dynamic> SetupGroup(dynamic permission, dynamic role, string group)
        {
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");
            _fabricAuthorizationService.SetAccessToken(accessToken);
            permission = await _fabricAuthorizationService.CreatePermission(permission);
            role = await _fabricAuthorizationService.CreatRole(role);
            await _fabricAuthorizationService.AddPermissionToRole(permission, role);
            await _fabricAuthorizationService.AddRoleToGroup(role, group);
            return _fabricAuthorizationService.GetGroupByName(group);
        }

        private async Task<IActionResult> CallApiWithToken(string accessToken)
        {
            var uri = new Uri("http://localhost:5003/patients/123");
            var client = _httpClientFactory.CreateWithAccessToken(uri, accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.PatientDataResponse = JsonConvert.DeserializeObject<PatientDataResponse>(await response.Content.ReadAsStringAsync());
                return View("Json");
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                ViewBag.ErrorMessage = $"Received 403 Forbidden when calling: {uri}";
                return View("Json");
            }
            throw new Exception($"Error received: {response.StatusCode} when trying to contact remote server: {uri}");
        }
    }

    public class PatientDataResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class UserClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
