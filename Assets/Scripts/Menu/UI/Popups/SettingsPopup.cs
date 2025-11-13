// Assets/Scripts/Menu/UI/Popups/SettingsPopup.cs
using Core.EventBusSystem;
using Core.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.UI.Popups
{
    public sealed class SettingsPopup : BasePopup
    {
        [Header("Buttons")]
        [SerializeField] private Button backButton;

        [Header("Toggles")]
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;

        private bool _lockUi; 

        protected override void Awake()
        {
            base.Awake();

            if (backButton) backButton.onClick.AddListener(OnBackClicked);

            if (musicToggle) musicToggle.onValueChanged.AddListener(OnMusicToggle);
            if (sfxToggle)   sfxToggle.onValueChanged.AddListener(OnSfxToggle);
        }

        private void OnEnable()
        {
            EventBus.Subscribe<SettingsLoaded>(OnSettings);
            EventBus.Subscribe<SettingsChanged>(OnSettings);
            
            EventBus.Invoke(new SettingsSyncRequested());
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SettingsLoaded>(OnSettings);
            EventBus.Unsubscribe<SettingsChanged>(OnSettings);
        }

        private void OnDestroy()
        {
            if (backButton)  backButton.onClick.RemoveListener(OnBackClicked);
            if (musicToggle) musicToggle.onValueChanged.RemoveListener(OnMusicToggle);
            if (sfxToggle)   sfxToggle.onValueChanged.RemoveListener(OnSfxToggle);
        }

        private void OnBackClicked() => EventBus.Invoke(new Menu.UI.MenuEvents.BackRequested());

        private void OnMusicToggle(bool isOn)
        {
            if (_lockUi) return;
            EventBus.Invoke(new MusicToggleRequested(isOn));
        }

        private void OnSfxToggle(bool isOn)
        {
            if (_lockUi) return;
            EventBus.Invoke(new SfxToggleRequested(isOn));
        }

        private void OnSettings(SettingsLoaded e)  => SyncUI(e.MusicOn, e.SfxOn);
        private void OnSettings(SettingsChanged e) => SyncUI(e.MusicOn, e.SfxOn);

        private void SyncUI(bool musicOn, bool sfxOn)
        {
            _lockUi = true;
            if (musicToggle) musicToggle.SetIsOnWithoutNotify(musicOn);
            if (sfxToggle)   sfxToggle.SetIsOnWithoutNotify(sfxOn);
            _lockUi = false;
        }
    }
}
