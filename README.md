# XamarinHttpClient
HTTPClient API Tester 

* I need to fix the radio button. I should have used a checkbox. After the proxy is enabled, you can't disable it. 

I run fiddler on a remote machine. (I had trouble getting this to work when both the app and fiddler were on the same machine)
the proxy address should be: http://{IP}:{PORT}

The approach here is to create a proxy and handler to pass into the new HttpClient:

```
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


httpClient.DefaultRequestHeaders.Add("X-API-Token", TOKEN.Trim());
var result = httpClient.GetAsync(URL.Trim());
results += result.Result.Content.ReadAsStringAsync().Result;

```