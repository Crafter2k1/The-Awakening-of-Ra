using System.Collections;
using Core.EventBusSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ✨ додали
using Core.SceneManagement;    // ✨ додали (для SceneLoadingStarted/Loaded/Ready)

namespace Core.SceneManagement
{
    [DefaultExecutionOrder(-100)]
    public sealed class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }

        [Header("UI")]
        [SerializeField] private GameObject root;     // контейнер екрана завантаження
        [SerializeField] private Slider progressBar;  // індикатор прогресу (0..1)

        [Header("Config (Inspector)")]
        [SerializeField, Tooltip("Швидкість згладжування заповнення (візуально).")]
        private float smoothSpeed = 6f;
        
        private const float DefaultFakeFinishTime = 1.0f;

        private Coroutine _routine;
        private bool _isLoading;
        private string _targetScene;

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
            
            if (SceneManager.GetActiveScene().name == sceneName)
            {
                Debug.LogWarning($"[LoadingScreen] Scene '{sceneName}' is already active.");
                root?.SetActive(false);
                return;
            }
            
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"[LoadingScreen] Scene '{sceneName}' is not in Build Settings.");
                return;
            }
            
            if (_isLoading && _targetScene == sceneName)
                return;

            if (_routine != null) StopCoroutine(_routine);

            _isLoading = true;
            _targetScene = sceneName;

            root?.SetActive(true);
            if (progressBar) progressBar.value = 0f;

            float fakeDelay = fakeDelayOverride ?? DefaultFakeFinishTime;
            _routine = StartCoroutine(LoadSceneRoutine(sceneName, fakeDelay));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, float fakeDelay)
        {
            // дати Canvas відмалюватися перед стартом важкого лоаду
            Canvas.ForceUpdateCanvases();
            yield return null;

            // ✨ нове: кинули подію "почали завантаження сцени"
            EventBus.Invoke(new SceneLoadingStarted(sceneName));

            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;

            float shown = 0f;
            const float realPhaseMax = 0.75f; // до 75% — реальний прогрес
            const float unityCap     = 0.9f;  // Unity звітує 0.9 до активації

            // --- ФАЗА 1: реальний прогрес (0–75%) ---
            while (async.progress < unityCap)
            {
                float normalized = Mathf.Clamp01(async.progress / unityCap); // 0..1
                float target = normalized * realPhaseMax;                    // 0..0.75
                shown = Mathf.MoveTowards(shown, target, Time.unscaledDeltaTime * smoothSpeed);

                if (progressBar) progressBar.value = shown;
                yield return null;
            }

            // --- ФАЗА 2: штучна затримка 75–100% ---
            float timer = 0f;
            while (timer < fakeDelay)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(timer / fakeDelay);
                float target = Mathf.Lerp(realPhaseMax, 1f, t); // 0.75 → 1.0 за fakeDelay
                shown = Mathf.MoveTowards(shown, target, Time.unscaledDeltaTime * smoothSpeed);

                if (progressBar) progressBar.value = shown;
                yield return null;
            }

            // гарантувати 100% і дати один кадр завершення
            if (progressBar) progressBar.value = 1f;
            yield return null;

            // --- Активуємо сцену ---
            async.allowSceneActivation = true;
            while (!async.isDone) yield return null;

            // ✨ нове: повідомили, що сцена активована
            EventBus.Invoke(new SceneLoaded(sceneName));
            // ✨ нове: ще один кадр — даємо Awake/Start у новій сцені відпрацювати
            yield return null;
            // ✨ нове: тепер сцена гарантовано "готова" для UI/контролерів
            EventBus.Invoke(new SceneReady(sceneName));

            // короткий буфер, щоб уникнути "блимання" на дуже швидких сценах
            yield return new WaitForSecondsRealtime(0.2f);

            if (root) root.SetActive(false);
            _isLoading = false;
            _targetScene = null;
            _routine = null;
        }
    }
}
