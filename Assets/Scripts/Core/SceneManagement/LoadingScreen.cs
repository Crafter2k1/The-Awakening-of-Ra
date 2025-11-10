using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core.SceneManagement
{
    [DefaultExecutionOrder(-100)] // створюється раніше за більшість менеджерів
    public sealed class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }

        [Header("UI")] [SerializeField] private GameObject root; // контейнер екрана завантаження
        [SerializeField] private Slider progressBar; // слайдер прогресу (0..1)

        [Header("Config")] [SerializeField, Tooltip("Мінімальний час показу екрана (сек).")]
        private float minShowTime = 1.0f;

        [SerializeField, Tooltip("Швидкість згладжування прогресу.")]
        private float smoothSpeed = 6f;

        private Coroutine _routine;

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (root != null) root.SetActive(false);
            if (progressBar != null) progressBar.value = 0f;
        }

        /// <summary>
        /// Публічний API для завантаження сцени з екраном завантаження.
        /// </summary>
        /// <param name="sceneName">Ім'я сцени з Build Settings.</param>
        /// <param name="minDelayOverride">Опціонально: перекрити мінімальний час показу.</param>
        public void LoadScene(string sceneName, float? minDelayOverride = null)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("[LoadingScreen] Scene name is null or empty.");
                return;
            }

            // якщо вже щось вантажимо — перезапускаємо
            if (_routine != null) StopCoroutine(_routine);

            root?.SetActive(true);
            if (progressBar != null) progressBar.value = 0f;

            _routine = StartCoroutine(LoadSceneRoutine(sceneName, minDelayOverride ?? minShowTime));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, float minDelay)
        {
            // TODO: тут можна кинути івент "SceneLoadingStarted"
            // EventBus.Publish(new SceneLoadingStarted(sceneName));

            var async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;

            float shown = 0f; // що показуємо на слайдері
            float elapsed = 0f; // час, що минув від старту показу
            const float preActivateCap = 0.9f; // Unity доходить ~до 0.9 перед активацією

            // 1) Поки сцена готується (progress до ~0.9), згладжено тягнемо показаний прогрес
            while (!async.isDone)
            {
                // реальний прогрес у діапазоні [0..0.9] до активації
                float target = Mathf.Clamp01(async.progress / preActivateCap);
                shown = Mathf.MoveTowards(shown, target, Time.unscaledDeltaTime * smoothSpeed);

                if (progressBar != null)
                    progressBar.value = shown;

                elapsed += Time.unscaledDeltaTime;

                // Вихід із циклу, коли:
                //  - ми досягли майже 1.0 по відображенню
                //  - і виконано мінімальний час показу
                if (shown >= 0.999f && elapsed >= minDelay)
                    break;

                yield return null;
            }

            // 2) Дорисувати до 1.0 (про всяк випадок, якщо не встигли)
            while (shown < 1f)
            {
                shown = Mathf.MoveTowards(shown, 1f, Time.unscaledDeltaTime * smoothSpeed);
                if (progressBar != null)
                    progressBar.value = shown;
                yield return null;
            }

            // Маленький кадр “завершення” перед активацією
            yield return null;

            // Дозволяємо активацію і чекаємо перемикання
            async.allowSceneActivation = true;
            while (!async.isDone)
                yield return null;

            // TODO: тут можна кинути івент "SceneLoaded(sceneName)"
            // EventBus.Publish(new SceneLoaded(sceneName));

            // Ховаємо екран
            if (root != null) root.SetActive(false);

            // Наступний кадр — безпечно повідомити "SceneRea
        }
    }
}
