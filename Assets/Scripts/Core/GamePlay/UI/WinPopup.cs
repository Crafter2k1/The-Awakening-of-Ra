// Assets/Scripts/Core/GamePlay/UI/WinPopup.cs
using Core.EventBusSystem;
using UnityEngine;
using UnityEngine.UI;
using Menu.UI.Popups;

namespace Core.GamePlay.UI
{
    public sealed class WinPopup : BasePopup
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;

        protected override void Awake()
        {
            base.Awake();

            if (restartButton)   restartButton.onClick.AddListener(OnRestartClicked);
            if (nextLevelButton) nextLevelButton.onClick.AddListener(OnNextClicked);
        }

        private void OnDestroy()
        {
            if (restartButton)   restartButton.onClick.RemoveListener(OnRestartClicked);
            if (nextLevelButton) nextLevelButton.onClick.RemoveListener(OnNextClicked);
        }

        private void OnRestartClicked()
        {
            EventBus.Invoke(new GameEvents.RestartRequested());
        }

        private void OnNextClicked()
        {
            EventBus.Invoke(new GameEvents.NextLevelRequested());
        }
    }
}