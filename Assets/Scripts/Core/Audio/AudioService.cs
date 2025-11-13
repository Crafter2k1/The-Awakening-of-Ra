// Assets/Scripts/Core/Audio/AudioService.cs
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

        // ❌ повністю видалено RuntimeInitializeOnLoadMethod + самостворення!

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // fallback: якщо міксер не підв’язаний у префабі — спробуємо дістати з Resources
            if (!masterMixer)
                masterMixer = Resources.Load<AudioMixer>("Audio/MasterMixer");

            if (!masterMixer)
                Debug.LogError("[AudioService] No AudioMixer assigned/found. Assign in prefab or put at Resources/Audio/MasterMixer.mixer");
        }

        private void OnEnable()
        {
            EventBus.Subscribe<SettingsLoaded>(OnSettingsLoaded);
            EventBus.Subscribe<SettingsChanged>(OnSettingsChanged);
            EventBus.Subscribe<SceneReady>(OnSceneReady);

            // одразу синхронізуємо стан (раптом пропустили ранні події)
            EventBus.Invoke(new SettingsSyncRequested());
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
