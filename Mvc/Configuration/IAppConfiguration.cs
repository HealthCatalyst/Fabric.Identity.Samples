using Fabric.Platform.Shared.Configuration;

namespace Fabric.Identity.Samples.Mvc.Configuration
{
    public interface IAppConfiguration
    {
        ElasticSearchSettings ElasticSearchSettings { get; }
        IdentityServerConfidentialClientSettings IdentityServerConfidentialClientSettings { get; }
    }
}
