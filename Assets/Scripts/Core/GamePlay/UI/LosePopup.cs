// Assets/Scripts/Core/GamePlay/UI/LosePopup.cs
using Core.EventBusSystem;
using UnityEngine;
using UnityEngine.UI;
using Menu.UI.Popups;

namespace Core.GamePlay.UI
{
    public sealed class LosePopup : BasePopup
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        protected override void Awake()
        {
            base.Awake();

            if (restartButton) restartButton.onClick.AddListener(OnRestartClicked);
            if (menuButton)    menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnDestroy()
        {
            if (restartButton) restartButton.onClick.RemoveListener(OnRestartClicked);
            if (menuButton)    menuButton.onClick.RemoveListener(OnMenuClicked);
        }

        private void OnRestartClicked()
        {
            EventBus.Invoke(new GameEvents.RestartRequested());
        }

        private void OnMenuClicked()
        {
            EventBus.Invoke(new GameEvents.GoToMenuRequested());
        }
    }
}