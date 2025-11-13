// Assets/Scripts/Core/Settings/SettingsService.cs
using Core.EventBusSystem;
using UnityEngine;

namespace Core.Settings
{
    [DefaultExecutionOrder(-150)]
    public sealed class SettingsService : MonoBehaviour
    {
        public static SettingsService Instance { get; private set; }

        private const string KeyMusic = "settings.music_on";
        private const string KeySfx   = "settings.sfx_on";
        private const int    DefaultOn = 1;

        private bool _musicOn = true;
        private bool _sfxOn   = true;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            // забезпечуємо існування сервісу до завантаження будь-якої сцени
            if (Instance != null) return;
            var go = new GameObject("[SettingsService]");
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<SettingsService>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
            // одразу розсилаємо стан, щоб інші підтягнулись
            EventBus.Invoke(new SettingsLoaded(_musicOn, _sfxOn));
            EventBus.Invoke(new SettingsChanged(_musicOn, _sfxOn));
        }

        private void OnEnable()
        {
            EventBus.Subscribe<MusicToggleRequested>(OnMusicToggleRequested);
            EventBus.Subscribe<SfxToggleRequested>(OnSfxToggleRequested);
            EventBus.Subscribe<SettingsSyncRequested>(OnSyncRequested);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<MusicToggleRequested>(OnMusicToggleRequested);
            EventBus.Unsubscribe<SfxToggleRequested>(OnSfxToggleRequested);
            EventBus.Unsubscribe<SettingsSyncRequested>(OnSyncRequested);
        }

        private void OnMusicToggleRequested(MusicToggleRequested e)
        {
            if (_musicOn == e.IsOn) return;
            _musicOn = e.IsOn;
            Save();
            EventBus.Invoke(new SettingsChanged(_musicOn, _sfxOn));
        }

        private void OnSfxToggleRequested(SfxToggleRequested e)
        {
            if (_sfxOn == e.IsOn) return;
            _sfxOn = e.IsOn;
            Save();
            EventBus.Invoke(new SettingsChanged(_musicOn, _sfxOn));
        }

        private void OnSyncRequested(SettingsSyncRequested _)
        {
            EventBus.Invoke(new SettingsChanged(_musicOn, _sfxOn));
        }

        private void Load()
        {
            _musicOn = PlayerPrefs.GetInt(KeyMusic, DefaultOn) != 0;
            _sfxOn   = PlayerPrefs.GetInt(KeySfx,   DefaultOn) != 0;
        }

        private void Save()
        {
            PlayerPrefs.SetInt(KeyMusic, _musicOn ? 1 : 0);
            PlayerPrefs.SetInt(KeySfx,   _sfxOn   ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
