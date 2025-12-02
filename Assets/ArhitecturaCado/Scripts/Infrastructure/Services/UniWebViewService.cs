using System.Collections;
using MainTool.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainTool.Infrastructure
{
    public class UniWebViewService : UIBehaviour
    {
        [SerializeField] private RectTransform _referenceRect;
        [SerializeField] private GameObject _navBar;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _closeButton;
        
        private WebGetService _webGetService;
        private UniWebView _view;
        
        private string _mainPageUrl;
        private bool _isSaved;

        protected override void Start()
        {
            base.Start();

            if (DataUtility.IsUrlSaved())
                InitAndShowWebView(DataUtility.GetSavedUrl());
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            SaveCookies();

            if (_webGetService != null)
                _webGetService.UrlReady -= ShowWebView;
        }
        
        public void Construct(WebGetService webGetService)
        {
            _webGetService = webGetService;
            _webGetService.UrlReady += ShowWebView;
        }

        public void Init()
        {
            UniWebView.SetAllowJavaScriptOpenWindow(true);
            UniWebView.SetAllowAutoPlay(true);
            UniWebView.SetAllowInlinePlay(true);
            _view = gameObject.AddComponent<UniWebView>();

            string savedCookies = DataUtility.GetSavedCookies();

            if (!string.IsNullOrEmpty(savedCookies))
            {
                string[] cookies = savedCookies.Split(';');
                foreach (string cookie in cookies)
                {
                    string script = $"document.cookie = '{cookie.Trim()}';";
                    _view.EvaluateJavaScript(script);
                }
            }
        }
        
        private void InitAndShowWebView(string url)
        {
            Init();
            ShowWebView(url);
        }
        
        private void ShowWebView(string url)
        {
            _view.SetSupportMultipleWindows(true, true);
            _view.OnMultipleWindowOpened += (view, id) => ShowSecondButton();
            _view.ReferenceRectTransform = _referenceRect;
            _view.Load(url, true);
            _navBar.SetActive(true);
            _backButton.onClick.AddListener(GoBack);

            _view.OnPageFinished += (view, statusCode, url) =>
            {
                if (!_isSaved)
                {
                    _isSaved = true;
                    DataUtility.SaveUrl(url);
                }

                _view.UpdateFrame();
            };

            _view.Show();
            StartCoroutine(GetUrl());
        }
        
        private IEnumerator GetUrl()
        {
            yield return new WaitForSeconds(4);

            if (string.IsNullOrEmpty(_mainPageUrl))
                _mainPageUrl = _view.Url;
        }

        private void SaveCookies()
        {
            if (_view != null)
            {
                _view.EvaluateJavaScript("document.cookie;", (result) =>
                {
                    if (!string.IsNullOrEmpty(result.data))
                        DataUtility.SaveCookies(result.data);
                });
            }
        }

        public string GetUserAgent() => _view.GetUserAgent();
        
        private void GoBack() => _view.GoBack();

        private void CloseView()
        {
            _closeButton.gameObject.SetActive(false);
            _view.EvaluateJavaScript("window.close();");
            InitAndShowWebView(_mainPageUrl);
        }
        
        private void ShowSecondButton()
        {
            _closeButton.gameObject.SetActive(true);
            _closeButton.onClick.AddListener(CloseView);
        }
        
        protected override void OnRectTransformDimensionsChange()
        {
            if (_view != null)
                _view.UpdateFrame();
        }
    }
}