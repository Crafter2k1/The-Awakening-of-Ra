using Core.EventBusSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.UI.Popups
{
    public sealed class MainMenuPopup : BasePopup
    {
        [Header("Buttons")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button levelSelectButton;
        [SerializeField] private Button quitButton;

        protected override void Awake()
        {
            base.Awake();

            if (settingsButton)
                settingsButton.onClick.AddListener(OnSettingsClicked);

            if (levelSelectButton)
                levelSelectButton.onClick.AddListener(OnLevelSelectClicked);

            if (quitButton)
                quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDestroy()
        {
            if (settingsButton)
                settingsButton.onClick.RemoveListener(OnSettingsClicked);

            if (levelSelectButton)
                levelSelectButton.onClick.RemoveListener(OnLevelSelectClicked);

            if (quitButton)
                quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnSettingsClicked()
            => EventBus.Invoke(new MenuEvents.OpenSettings());
        
        private void OnLevelSelectClicked()
            => EventBus.Invoke(new MenuEvents.StartGameRequested());

        private void OnQuitClicked()
            => EventBus.Invoke(new MenuEvents.QuitRequested());
    }
}