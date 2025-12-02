using System;
using System.Globalization;
using MainTool.Utils;
using UnityEngine;


namespace MainTool.Infrastructure
{
    public class WebGetService : MonoBehaviour
    {
        private readonly UniWebViewService _uniWebViewService;

        private OneSignalService _oneSignalService;
        private ConfigService _configService;
        private LinksResponse _linksResponse;
        private AttributionResponse _attributionResponse;
        private UniWebView _uniWebView;
        
        private string _ip;

        public event Action<string> UrlReady;
        public Action Failed;

        public void Construct(OneSignalService oneSignalService, ConfigService configService)
        {
            _oneSignalService = oneSignalService;
            _configService = configService;
            Debug.Log("Constructed");
        }

        public void Init()
        {
            if (!string.IsNullOrEmpty(_configService.BaseEndPoint))
            {
                UniWebView.SetAllowJavaScriptOpenWindow(true);
                UniWebView.SetAllowAutoPlay(true);
                UniWebView.SetAllowInlinePlay(true);
                _uniWebView = gameObject.AddComponent<UniWebView>();

                Debug.Log("Created webView");

                IPGetRequest();
            }
            else
            {
                Failed?.Invoke();
                Debug.Log("failed");
            }
        }
        
        private void IPGetRequest()
        {
            Debug.Log("starting Ip request");

            _uniWebView.OnPageFinished += OnIpPageFinished;
            _uniWebView.Load(Constants.IP_END_POINT);
            _uniWebView.Show();
        }
        
        private void OnIpPageFinished(UniWebView view, int code, string url)
        {
            if (CheckFailureStatusCode(code)) 
                return;
            
            view.EvaluateJavaScript("document.body.innerText", (payload) =>
            {
                _ip = payload.data;
                _uniWebView.OnPageFinished -= OnIpPageFinished;
                
                Debug.Log("ip:" + _ip);
                
                MakeAPIRequest();
            });
        }

        private void MakeAPIRequest()
        {
            _uniWebView.SetHeaderField("apikey", Constants.API_KEY);
            _uniWebView.SetHeaderField("bundle", Constants.BUNDLE);
            _uniWebView.OnPageFinished += OnApiPageFinished;
            _uniWebView.Load(_configService.BaseEndPoint);
        }

        private void OnApiPageFinished(UniWebView view, int code, string url)
        {
            if (CheckFailureStatusCode(code)) 
                return;

            view.EvaluateJavaScript("document.body.innerText", (payload) =>
            {
                string json = payload.data;
                _linksResponse = JsonUtility.FromJson<LinksResponse>(json);
                
                Debug.Log("LinkResponse:" + json);
                
                _uniWebView.OnPageFinished -= OnApiPageFinished;
                MakeCloackRequest();
            });
        }
        
        private void MakeCloackRequest()
        {
            CleanHeaders(new[] { "apikey", "bundle" });
            _uniWebView.SetHeaderField("apikeyapp", Constants.API_KEY_APP);
            _uniWebView.SetHeaderField("useragent", _uniWebView.GetUserAgent());
            _uniWebView.SetHeaderField("ip", _ip);
            _uniWebView.SetHeaderField("langcode", GetCountryCode());
            _uniWebView.OnPageFinished += OnCloakPageFinished;
            _uniWebView.Load(_linksResponse.cloack_url);
        }
        
        private void OnCloakPageFinished(UniWebView view, int code, string url)
        {
            if (CheckFailureStatusCode(code)) 
                return;
            
            _uniWebView.OnPageFinished -= OnCloakPageFinished;
            Debug.Log("Cloack passed succesfuly");
            MakeAttributionRequest();
        }
        
        private void MakeAttributionRequest()
        {
            _uniWebView.OnPageFinished += OnAttributionPageFinished;
            _uniWebView.Load(_linksResponse.atr_service);
        }
        
        private void OnAttributionPageFinished(UniWebView view, int code, string url)
        {
            if (CheckFailureStatusCode(code)) return;

            view.EvaluateJavaScript("document.body.innerText", (payload) =>
            {
                string json = payload.data;
                _attributionResponse = json.ToDeserialize<AttributionResponse>();

                Debug.Log("AttributionResponse:" + json);
                Debug.Log("finalUrl" + _attributionResponse.final_url);

                UrlReady?.Invoke(_attributionResponse.final_url);
                _oneSignalService.Login(_attributionResponse.os_user_key);
                _oneSignalService.SendPush(_attributionResponse.push_sub);
            
                _uniWebView.OnPageFinished -= OnAttributionPageFinished;
            });
        }
        
        private bool CheckFailureStatusCode(int statusCode)
        {
            if (statusCode != 200)
            {
                Failed?.Invoke();
                Debug.Log("Failed somewhere");
                return true;
            }

            return false;
        }
        
        private void CleanHeaders(string[] keys)
        {
            foreach (string key in keys)
                _uniWebView.SetHeaderField(key, null);
        }

        private string GetCountryCode()
        {
            RegionInfo regionInfo = RegionInfo.CurrentRegion;
            return regionInfo.TwoLetterISORegionName;
        }
    }
}