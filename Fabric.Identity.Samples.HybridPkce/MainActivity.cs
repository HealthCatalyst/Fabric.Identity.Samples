using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using OpenId.AppAuth;

namespace Fabric.Identity.Samples.HybridPkce
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private static string ClientId = "hybrid-pkce-sample";
        private static string RedirectUri = "hybrid-pkce-sample:/oauth2redirect";
        private static string AuthEndpoint = http://localhost:5001/connect/authorize;
        private static string TokenEndpoint = http://localhost:5001/connect/token;
        private AuthorizationService _authorizationService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            _authorizationService = new AuthorizationService(this);

            try
            {
                AuthorizationServiceConfiguration serviceConfiguration;
                serviceConfiguration = new AuthorizationServiceConfiguration(
                    Android.Net.Uri.Parse(AuthEndpoint),
                    Android.Net.Uri.Parse(TokenEndpoint),
                    null);

                MakeAuthRequest(serviceConfiguration, new AuthState());
            }
            catch (AuthorizationException ex)
            {
                Console.WriteLine("Failed to retrieve configuration:" + ex);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void MakeAuthRequest(AuthorizationServiceConfiguration serviceConfig, AuthState authState)
        {
            var authRequest = new AuthorizationRequest.Builder(serviceConfig, ClientId, ResponseTypeValues.Code, Android.Net.Uri.Parse(RedirectUri))
                .SetScope("openid profile email")
                .Build();

            Console.WriteLine("Making auth request to " + serviceConfig.AuthorizationEndpoint);
            _authorizationService.PerformAuthorizationRequest(
                authRequest,
                TokenActivity.CreatePostAuthorizationIntent(this, authRequest, serviceConfig.DiscoveryDoc, authState),
                _authorizationService.CreateCustomTabsIntentBuilder().SetToolbarColor(Resources.GetColor(Resource.Color.colorAccent)).Build());
        }

        public PendingIntent CreatePostAuthorizationIntent(Context context, AuthorizationRequest request, AuthorizationServiceDiscovery discoveryDoc, AuthState authState)
        {
            var intent = new Intent(context, typeof(this));
            intent.PutExtra(EXTRA_AUTH_STATE, authState.JsonSerializeString());
            if (discoveryDoc != null)
            {
                intent.PutExtra(EXTRA_AUTH_SERVICE_DISCOVERY, discoveryDoc.DocJson.ToString());
            }

            return PendingIntent.GetActivity(context, request.GetHashCode(), intent, 0);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }
	}
}

