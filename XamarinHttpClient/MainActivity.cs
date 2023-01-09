using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Android.Widget;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net;
using Android.Content;

namespace XamarinHttpClient
{
#if DEBUG
    //[Application(Debuggable = true, NetworkSecurityConfig = "@xml/network_security_config")]
#else
    [Application(Debuggable = false)]
#endif

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        string AppCenterAnalyticsKey = "2f5f11e2-a59a-4a5b-974f-b77278ea8516";
        Button btnHttpClientApiTester;
        bool Usefiddler = true;
        string FiddlerUri = $"http://127.0.0.1:8888";
        Button btnSumbitAPICall;
        EditText editTextURLEndpoint;
        TextView textViewApiToken;
        TextView textViewResults;
        RadioButton radioButtonUseProxy;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCenterSetup();

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            //Setup Controls
            /*            btnSumbitAPICall = FindViewById<Button>(Resource.Id.btnSubmitApiCall);
                        editTextURLEndpoint = FindViewById<EditText>(Resource.Id.editTextAPIEndpointInput);
                        textViewApiToken = FindViewById<TextView>(Resource.Id.textViewtextViewAppCenterApiKeyInput);
                        textViewResults = FindViewById<TextView>(Resource.Id.textViewResults);
                        radioButtonUseProxy = FindViewById<RadioButton>(Resource.Id.radioButtonUseProxy);
                        btnSumbitAPICall.Click+= btnSumbitAPICall_Click;*/

            btnHttpClientApiTester = FindViewById<Button>(Resource.Id.buttonHttpClientApiTester);

            btnHttpClientApiTester.Click += BtnHttpClientApiTester_Click;
        }

        private void BtnHttpClientApiTester_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(APITester));
            intent.PutExtra("HelloWorld", DateTime.Now.ToLongTimeString());  
            this.StartActivity(intent);
        }

        private void AppCenterSetup()
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start(AppCenterAnalyticsKey, typeof(Crashes), typeof(Analytics));
            Analytics.TrackEvent("Startup Succeeded");
        }

        private void btnSumbitAPICall_Click(object sender, EventArgs e)
        {
            bool bContinue = true;

            Analytics.TrackEvent($"btnSumbitAPICall Started: {editTextURLEndpoint.Text}  at {DateTime.Now.ToLongTimeString()}");
            string URL = editTextURLEndpoint.Text;
            string TOKEN = textViewApiToken.Text;
            string results = "";
            HttpClient httpClient;
            WebProxy proxy;
            HttpClientHandler httpClientHandler;

            try
            {
                if (TOKEN.Length <= 0 || URL.Length <= 0)
                {
                    bContinue = false;
                }

                if (bContinue)
                {
                    if (Uri.IsWellFormedUriString(URL, UriKind.Absolute))
                    {
                        if (radioButtonUseProxy.Checked)
                        {
                            proxy = new WebProxy
                            {
                                Address = new Uri(FiddlerUri),
                                BypassProxyOnLocal = false,
                                UseDefaultCredentials = false,
                            };

                            httpClientHandler = new HttpClientHandler
                            {
                                Proxy = proxy,
                            };

                            // Disable SSL verification
                            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                            httpClient = new HttpClient(httpClientHandler, true);
                        }
                        else
                        {
                            httpClient = new HttpClient();
                        }

                        //httpClient.DefaultRequestHeaders.Add("Bearer", TOKEN);
                        httpClient.DefaultRequestHeaders.Add("X-API-Token", TOKEN);
                        var result = httpClient.GetAsync(URL);
                        btnSumbitAPICall.Text += $": Status Code = {result.Result.StatusCode}";
                        results = result.Result.Content.ToString();
                    }
                    else
                    {
                        bContinue = false;
                    }
                }

                if (bContinue)
                {

                }

            }
            catch (Exception ex)
            {
                bContinue = false;
                Crashes.TrackError(ex);
                Analytics.TrackEvent($"btnWebSearch_Click threw error {DateTime.Now.ToLongTimeString()}");
                results += ex.Message;
            }

            textViewResults.Text = results;

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

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
