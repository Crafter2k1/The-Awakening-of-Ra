// Assets/Scripts/Core/GamePlay/UI/PausePopup.cs
using Core.EventBusSystem;
using UnityEngine;
using UnityEngine.UI;
using Menu.UI.Popups;

namespace Core.GamePlay.UI
{
    public sealed class PausePopup : BasePopup
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        protected override void Awake()
        {
            base.Awake();

            if (resumeButton)  resumeButton.onClick.AddListener(OnResumeClicked);
            if (restartButton) restartButton.onClick.AddListener(OnRestartClicked);
            if (menuButton)    menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnDestroy()
        {
            if (resumeButton)  resumeButton.onClick.RemoveListener(OnResumeClicked);
            if (restartButton) restartButton.onClick.RemoveListener(OnRestartClicked);
            if (menuButton)    menuButton.onClick.RemoveListener(OnMenuClicked);
        }

        private void OnResumeClicked()
        {
            EventBus.Invoke(new GameEvents.ResumeRequested());
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