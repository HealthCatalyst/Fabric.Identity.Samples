using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Xamarin.Forms;

namespace HybridPkce.Core
{
    public partial class MainPage : ContentPage
    {
        OidcClient _client;
        LoginResult _result;
        IBrowser _browser;

        private bool _isAuthenticated;

        public MainPage()
        {
            InitializeComponent();

            _browser = DependencyService.Get<IBrowser>();

            var options = new OidcClientOptions
            {
                // 10.0.2.2 is an alias for localhost on the computer when using the android emulator
                Authority = "http://10.0.2.2:50405/identity",
                ClientId = "native.hybrid",
                Scope = "openid profile email offline_access",
                RedirectUri = "xamarinformsclients://callback",
                PostLogoutRedirectUri = "xamarinformsclients://callback",
                Browser = _browser,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Policy = new Policy()
                {
                    Discovery = new DiscoveryPolicy() { RequireHttps = false }
                }
            };
        }
    }
}
