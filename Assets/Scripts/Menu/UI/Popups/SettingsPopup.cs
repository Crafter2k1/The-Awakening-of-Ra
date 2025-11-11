using Core.EventBusSystem;
using UnityEngine;
using UnityEngine.UI;


namespace Menu.UI.Popups
{
    public sealed class SettingsPopup : BasePopup
    {
        [Header("Buttons")]
        [SerializeField] private Button backButton;

        // приклад додаткових контролів (опційно):
        // [SerializeField] private Slider musicVolume;
        // [SerializeField] private Toggle vsyncToggle;

        protected override void Awake()
        {
            base.Awake();
            if (backButton) backButton.onClick.AddListener(OnBackClicked);

            // приклад прив’язки своїх подій:
            // if (musicVolume) musicVolume.onValueChanged.AddListener(v => EventBus.Invoke(new MusicVolumeChanged(v)));
            // if (vsyncToggle) vsyncToggle.onValueChanged.AddListener(on => EventBus.Invoke(new VSyncToggled(on)));
        }

        private void OnDestroy()
        {
            if (backButton) backButton.onClick.RemoveListener(OnBackClicked);
            // if (musicVolume) musicVolume.onValueChanged.RemoveAllListeners();
            // if (vsyncToggle) vsyncToggle.onValueChanged.RemoveAllListeners();
        }

        private void OnBackClicked() => EventBus.Invoke(new MenuEvents.BackRequested());
    }
}