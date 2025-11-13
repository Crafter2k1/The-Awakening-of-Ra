using Core.EventBusSystem;
using Core.Settings;
using Core.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio
{
    [DefaultExecutionOrder(-120)]
    public sealed class AudioService : MonoBehaviour
    {
        public static AudioService Instance { get; private set; }

        [Header("Mixer")]
        [SerializeField] private AudioMixer masterMixer;
        [SerializeField] private string musicParam = "MusicVolume";
        [SerializeField] private string sfxParam   = "SFXVolume";

        [Header("Volumes (dB)")]
        [SerializeField] private float onDb  = 0f;
        [SerializeField] private float offDb = -80f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (Instance != null) return;
            var go = new GameObject("[AudioService]");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<AudioService>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            // Стан налаштувань
            EventBus.Subscribe<SettingsLoaded>(OnSettingsLoaded);
            EventBus.Subscribe<SettingsChanged>(OnSettingsChanged);

            // Після кожної сцени просимо сервіс скинути поточний стан (для нових AudioSource)
            EventBus.Subscribe<SceneReady>(OnSceneReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SettingsLoaded>(OnSettingsLoaded);
            EventBus.Unsubscribe<SettingsChanged>(OnSettingsChanged);
            EventBus.Unsubscribe<SceneReady>(OnSceneReady);
        }

        private void OnSettingsLoaded(SettingsLoaded e)  => Apply(e.MusicOn, e.SfxOn);
        private void OnSettingsChanged(SettingsChanged e) => Apply(e.MusicOn, e.SfxOn);

        private void OnSceneReady(SceneReady _)
        {
            // Запитуємо повторну синхронізацію стану, щоб застосувати його у щойно активованій сцені
            EventBus.Invoke(new SettingsSyncRequested());
        }

        private void Apply(bool musicOn, bool sfxOn)
        {
            if (!masterMixer)
            {
                Debug.LogWarning("[AudioService] No AudioMixer assigned.");
                return;
            }
            masterMixer.SetFloat(musicParam, musicOn ? onDb : offDb);
            masterMixer.SetFloat(sfxParam,   sfxOn   ? onDb : offDb);
        }
    }
}
