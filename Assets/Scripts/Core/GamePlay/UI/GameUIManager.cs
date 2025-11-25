using Core.EventBusSystem;
using UnityEngine;

namespace Core.GamePlay.UI
{
    public sealed class GameUIManager : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private GameplayHud gameplayHud;
        [SerializeField] private PausePopup pausePopup;
        [SerializeField] private WinPopup winPopup;
        [SerializeField] private LosePopup losePopup;

        private void Awake()
        {
            if (pausePopup) pausePopup.HideView();
            if (winPopup)   winPopup.HideView();
            if (losePopup)  losePopup.HideView();
            if (gameplayHud) gameplayHud.ShowView();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.PauseRequested>(OnPauseRequested);
            EventBus.Subscribe<GameEvents.ResumeRequested>(OnResumeRequested);
            EventBus.Subscribe<GameEvents.LevelCompleted>(OnLevelCompleted);
            EventBus.Subscribe<GameEvents.LevelFailed>(OnLevelFailed);
            EventBus.Subscribe<GameEvents.RestartRequested>(OnRestartRequested);
            EventBus.Subscribe<GameEvents.GoToMenuRequested>(OnGoToMenuRequested);   // 👈 додано
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.PauseRequested>(OnPauseRequested);
            EventBus.Unsubscribe<GameEvents.ResumeRequested>(OnResumeRequested);
            EventBus.Unsubscribe<GameEvents.LevelCompleted>(OnLevelCompleted);
            EventBus.Unsubscribe<GameEvents.LevelFailed>(OnLevelFailed);
            EventBus.Unsubscribe<GameEvents.RestartRequested>(OnRestartRequested);
            EventBus.Unsubscribe<GameEvents.GoToMenuRequested>(OnGoToMenuRequested); // 👈 додано
        }

        void OnPauseRequested(GameEvents.PauseRequested _)
        {
            if (pausePopup) pausePopup.ShowView();
        }

        void OnResumeRequested(GameEvents.ResumeRequested _)
        {
            if (pausePopup) pausePopup.HideView();
        }

        void OnLevelCompleted(GameEvents.LevelCompleted _)
        {
            if (pausePopup) pausePopup.HideView();
            if (winPopup)   winPopup.ShowView();
        }

        void OnLevelFailed(GameEvents.LevelFailed _)
        {
            if (pausePopup) pausePopup.HideView();
            if (losePopup)  losePopup.ShowView();
        }

        void OnRestartRequested(GameEvents.RestartRequested _)
        {
            if (pausePopup) pausePopup.HideView();
            if (winPopup)   winPopup.HideView();
            if (losePopup)  losePopup.HideView();
            if (gameplayHud) gameplayHud.ShowView();
        }

        // 👇 нове — ховає PausePopup при виході в меню
        void OnGoToMenuRequested(GameEvents.GoToMenuRequested _)
        {
            if (pausePopup) pausePopup.HideView();
            if (winPopup)   winPopup.HideView();
            if (losePopup)  losePopup.HideView();
        }
    }
}
