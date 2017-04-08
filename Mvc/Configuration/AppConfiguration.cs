using Fabric.Platform.Shared.Configuration;

namespace Fabric.Identity.Samples.Mvc.Configuration
{
    public class AppConfiguration : IAppConfiguration
    {
        public ElasticSearchSettings ElasticSearchSettings { get; set; }

        public IdentityServerConfidentialClientSettings IdentityServerConfidentialClientSettings { get; set; }
    }
}
