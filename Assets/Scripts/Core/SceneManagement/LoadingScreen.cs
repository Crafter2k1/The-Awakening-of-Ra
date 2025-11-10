using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.SceneManagement
{
    [DefaultExecutionOrder(-100)]
    public sealed class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private GameObject root;   
        [SerializeField] private Slider progressBar;  

        [Header("Config (Inspector)")]
        [SerializeField, Tooltip("Fill smoothing speed")]
        private float smoothSpeed = 6f;
        
        private const float DefaultFakeFinishTime = 1.0f;

        private Coroutine _routine;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (root) root.SetActive(false);
            if (progressBar) progressBar.value = 0f;
        }
        
        public void LoadScene(string sceneName, float? fakeDelayOverride = null)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("[LoadingScreen] Scene name is null or empty.");
                return;
            }

            if (_routine != null) StopCoroutine(_routine);

            root?.SetActive(true);
            if (progressBar) progressBar.value = 0f;

            float fakeDelay = fakeDelayOverride ?? DefaultFakeFinishTime;
            _routine = StartCoroutine(LoadSceneRoutine(sceneName, fakeDelay));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, float fakeDelay)
        {
            Canvas.ForceUpdateCanvases();
            yield return null;

            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;

            float shown = 0f;
            const float realPhaseMax = 0.75f;  
            const float unityCap = 0.9f;      
            
            while (async.progress < unityCap)
            {
                float normalized = Mathf.Clamp01(async.progress / unityCap); 
                float target = normalized * realPhaseMax;                 
                shown = Mathf.MoveTowards(shown, target, Time.unscaledDeltaTime * smoothSpeed);
                if (progressBar) progressBar.value = shown;
                yield return null;
            }
            
            float timer = 0f;
            while (timer < fakeDelay)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(timer / fakeDelay);
                float target = Mathf.Lerp(realPhaseMax, 1f, t); 
                shown = Mathf.MoveTowards(shown, target, Time.unscaledDeltaTime * smoothSpeed);
                if (progressBar) progressBar.value = shown;
                yield return null;
            }

            if (progressBar) progressBar.value = 1f;
            yield return null;

            async.allowSceneActivation = true;
            while (!async.isDone) yield return null;

            if (root) root.SetActive(false);
        }
    }
}
