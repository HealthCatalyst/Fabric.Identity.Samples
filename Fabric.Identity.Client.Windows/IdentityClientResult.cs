using System;

namespace Fabric.Identity.Client.Windows
{
    public class IdentityClientResult
    {
        public string IdentityToken { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string UserName { get; set; }
    }
}