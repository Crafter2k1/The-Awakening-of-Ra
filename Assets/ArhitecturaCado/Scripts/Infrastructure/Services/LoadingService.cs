using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainTool.Infrastructure
{
    public class LoadingService : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private WebGetService _webGetService;
        
        [Header("Parameters")]
        [SerializeField] private float _delay;
        [SerializeField] private string _sceneName;

        private void OnEnable()
        {
            if (_webGetService != null)
                _webGetService.Failed += OnFailed;
        }

        private void OnDisable()
        {
            if (_webGetService != null)
                _webGetService.Failed -= OnFailed;
        }

        private void OnFailed() => StartCoroutine(LoadSceneRoutine());

        private IEnumerator LoadSceneRoutine()
        {
            yield return new WaitForSeconds(_delay);
            SceneManager.LoadScene(_sceneName);
        }
    }
}