using System;
using System.Text;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Xamarin.Forms;

namespace HybridPkce
{
    public partial class MainPage : ContentPage
    {
        private readonly OidcClient _client;
        private LoginResult _result;

        public MainPage()
        {
            InitializeComponent();

            Login.IsVisible = true;
            Login.Clicked += Login_Clicked;

            Logout.IsVisible = false;
            Logout.Clicked += Logout_Clicked;

            var browser = DependencyService.Get<IBrowser>();

            var options = new OidcClientOptions
            {
                // 10.0.2.2 is an alias for localhost on the computer when using the android emulator
                Authority = "http://localhost:5001",
                ClientId = "hybrid-pkce-android-sample",
                Scope = "openid profile email offline_access",
                RedirectUri = "xamarinformsclients://callback",
                PostLogoutRedirectUri = "xamarinformsclients://callback",
                Browser = browser,
                Flow = OidcClientOptions.AuthenticationFlow.Hybrid,
                ResponseMode = OidcClientOptions.AuthorizeResponseMode.Redirect,
                Policy = new Policy
                {
                    Discovery = new DiscoveryPolicy() { RequireHttps = false }
                }
            };

            _client = new OidcClient(options);
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            _result = await _client.LoginAsync(new LoginRequest());

            if (_result.IsError)
            {
                OutputText.Text = _result.Error;
                return;
            }

            CallApi.IsVisible = true;
            Login.IsVisible = false;
            Logout.IsVisible = true;

            var sb = new StringBuilder(128);
            foreach (var claim in _result.User.Claims)
            {
                sb.AppendFormat("{0}: {1}\n", claim.Type, claim.Value);
            }

            sb.AppendFormat("\n{0}: {1}\n", "refresh token", _result?.RefreshToken ?? "none");
            sb.AppendFormat("\n{0}: {1}\n", "access token", _result.AccessToken);

            OutputText.Text = sb.ToString();
        }

        private async void Logout_Clicked(object sender, EventArgs e)
        {
            var logoutRequest = new LogoutRequest();

            CallApi.IsVisible = false;
            Login.IsVisible = true;
            Logout.IsVisible = false;

            OutputText.Text = string.Empty;
            await _client.LogoutAsync(logoutRequest);
        }
    }
}
