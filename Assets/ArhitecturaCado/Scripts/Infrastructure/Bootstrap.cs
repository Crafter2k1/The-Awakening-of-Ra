using MainTool.Infrastructure;
using MainTool.Utils;
using UnityEngine;

namespace MainTool
{
    public class Bootstrap : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private OrientationType _defaultOrientation;
        
        [Header("References")] 
        [SerializeField] private UniWebViewService _uniWebViewService;
        [SerializeField] private WebGetService _webGetService;

        private ConfigService _configService;
        private OneSignalService _oneSignalService;
        
        private async void Awake()
        {
            _configService = new ConfigService();

            await _configService.Init();

            InitializeServices();

            if (!DataUtility.IsUrlSaved())
                InitServices();
        }

        private void InitializeServices()
        {
            _oneSignalService = new OneSignalService();
            _webGetService.Construct(_oneSignalService, _configService);
            new FailureHandler(_webGetService, _defaultOrientation);
            _uniWebViewService.Construct(_webGetService);
        }

        private void InitServices()
        {
            _uniWebViewService.Init();
            _oneSignalService.Init();
            
            if (_configService.IsInitialized && !string.IsNullOrEmpty(_configService.BaseEndPoint))
            {
                _webGetService.Init();
            }
            else
            {
                Debug.LogWarning("Config service not properly initialized, do with fallback behavior");
                _webGetService.Failed?.Invoke();
            }
        }
    }
}