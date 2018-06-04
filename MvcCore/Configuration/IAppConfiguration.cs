using Fabric.Platform.Shared.Configuration;

namespace Fabric.Identity.Samples.MvcCore.Configuration
{
    public interface IAppConfiguration
    {
        ElasticSearchSettings ElasticSearchSettings { get; }
        IdentityServerConfidentialClientSettings IdentityServerConfidentialClientSettings { get; }
    }
}
