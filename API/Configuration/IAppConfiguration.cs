using Fabric.Platform.Shared.Configuration;

namespace Fabric.Identity.Samples.API.Configuration
{
    public interface IAppConfiguration
    {
        ElasticSearchSettings ElasticSearchSettings { get; }
        IdentityServerConfidentialClientSettings IdentityServerConfidentialClientSettings { get; }
    }
}