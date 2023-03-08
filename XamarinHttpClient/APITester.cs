using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Net.Http.Json;

namespace XamarinHttpClient
{
    [Activity(Label = "APITester")]
    public class APITester : Activity
    {
        Button btnSumbitAPICall;
        
        EditText textViewURLEndpoint;
        EditText textViewApiToken;
        EditText textViewUseProxyUrl;
        
        TextView textViewResults;

        CheckBox CheckBoxUseProxy;
        //RadioButton radioButtonUseProxy;


        string dataPassedIntoThisClass;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ApiTester);

            dataPassedIntoThisClass = Intent.GetStringExtra("HelloWorld");
            Analytics.TrackEvent($"APITester Entered at {dataPassedIntoThisClass}");

            // Create your application here

            Setupcontrols();
        }

        private void Setupcontrols()
        {
            //Setup Controls
            btnSumbitAPICall = FindViewById<Button>(Resource.Id.buttonSubmit);
            textViewURLEndpoint = FindViewById<EditText>(Resource.Id.editTextApiUrl);
            textViewApiToken = FindViewById<EditText>(Resource.Id.editTextApiKey);
            textViewResults = FindViewById<TextView>(Resource.Id.textViewApiResults);
            textViewUseProxyUrl = FindViewById<EditText>(Resource.Id.editTextProxyUrl);
            CheckBoxUseProxy = FindViewById<CheckBox>(Resource.Id.checkBoxUseProxy);
            btnSumbitAPICall.Click += btnSumbitAPICall_Click;
        }

        private void btnSumbitAPICall_Click(object sender, EventArgs e)
        {
            bool bContinue = true;
            Analytics.TrackEvent($"btnSumbitAPICall Started: {textViewURLEndpoint.Text}  at {DateTime.Now.ToLongTimeString()}");
            string URL = textViewURLEndpoint.Text;
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
                        if (CheckBoxUseProxy.Checked)
                        {
                            proxy = new WebProxy
                            {
                                Address = new Uri(textViewUseProxyUrl.Text.Trim()),
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
                        httpClient.DefaultRequestHeaders.Add("X-API-Token", TOKEN.Trim());
                        //var result = httpClient.GetFromJsonAsync<object>(URL.Trim());
                        var result = httpClient.GetAsync(URL.Trim());
                        results += $": Status Code = {result} {System.Environment.NewLine}";
                        results += result.Result.Content.ReadAsStringAsync().Result;
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
    }
}