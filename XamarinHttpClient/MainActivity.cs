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

namespace XamarinHttpClient
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button btnWebSearch;
        EditText editTextURLEndpoint;
        TextView TextViewResults;

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
            btnWebSearch = FindViewById<Button>(Resource.Id.button1);
            editTextURLEndpoint = FindViewById<EditText>(Resource.Id.editText1); ;
            TextViewResults = FindViewById<TextView>(Resource.Id.textView4);

            btnWebSearch.Click+= btnWebSearch_Click;
        }

        private void AppCenterSetup()
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            AppCenter.Start("", typeof(Crashes), typeof(Analytics));
            Analytics.TrackEvent("Startup Succeeded");
        }

        private void btnWebSearch_Click(object sender, EventArgs e)
        {
            Analytics.TrackEvent($"WebSearch Started: {editTextURLEndpoint.Text}");
            HttpClient httpClient;
            Analytics.TrackEvent($"btnWebSearch_Click at {DateTime.Now.ToLongTimeString()}");

            try
            {
                string URL = editTextURLEndpoint.Text;
                if (Uri.IsWellFormedUriString(URL, UriKind.Absolute))
                {
                    httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Add("Bearer", "");
                    //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "5492a5ebcdbc9388f772429885e6840cd756383e");
                    var result = httpClient.GetAsync(URL);
                    TextViewResults.Text = result.Result.Content.ToString();
                    btnWebSearch.Text += $": Status Code = {result.Result.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Analytics.TrackEvent($"btnWebSearch_Click threw error {DateTime.Now.ToLongTimeString()}");
                TextViewResults.Text += ex.Message;
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
