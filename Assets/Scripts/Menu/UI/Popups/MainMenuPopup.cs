using Core.EventBus;
using UnityEngine;
using UnityEngine.UI;
// OpenSettings, OpenLevelSelection, QuitRequested, StartGameRequested (якщо треба)

namespace Menu.UI.Popups
{
    public sealed class MainMenuPopup : BasePopup
    {
        [Header("Buttons")]
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button levelSelectButton;

        // опційні (якщо є у твоєму UI)
        [SerializeField] private Button startButton; // напряму старт гри без вибору рівня
        [SerializeField] private Button quitButton;

        protected override void Awake()
        {
            base.Awake();

            if (settingsButton)     settingsButton.onClick.AddListener(OnSettingsClicked);
            if (levelSelectButton)  levelSelectButton.onClick.AddListener(OnLevelSelectClicked);
            if (startButton)        startButton.onClick.AddListener(OnStartClicked);
            if (quitButton)         quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void OnDestroy()
        {
            if (settingsButton)     settingsButton.onClick.RemoveListener(OnSettingsClicked);
            if (levelSelectButton)  levelSelectButton.onClick.RemoveListener(OnLevelSelectClicked);
            if (startButton)        startButton.onClick.RemoveListener(OnStartClicked);
            if (quitButton)         quitButton.onClick.RemoveListener(OnQuitClicked);
        }

        private void OnSettingsClicked()    => EventBus.Invoke(new MenuEvents.OpenSettings());
        private void OnLevelSelectClicked() => EventBus.Invoke(new MenuEvents.OpenLevelSelection());
        private void OnStartClicked()       => EventBus.Invoke(new MenuEvents.StartGameRequested());  
        private void OnQuitClicked()        => EventBus.Invoke(new MenuEvents.QuitRequested());
    }
}