namespace Fabric.Identity.Client.Windows
{
    public class IdentityClientConfiguration
    {
        public string ClientId { get; set; }
        public string HostBaseUri { get; set; }
        public int Port { get; set; }
        public string Scope { get; set; }
        public string Authority { get; set; }
    }
}