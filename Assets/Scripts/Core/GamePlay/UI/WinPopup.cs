// Assets/Scripts/Core/GamePlay/UI/WinPopup.cs
using Core.EventBusSystem;
using UnityEngine;
using UnityEngine.UI;
using Menu.UI.Popups;

namespace Core.GamePlay.UI
{
    public sealed class WinPopup : BasePopup
    {
        [Header("Buttons")]
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button menuButton;

        protected override void Awake()
        {
            base.Awake();

            if (nextLevelButton)
                nextLevelButton.onClick.AddListener(OnNextClicked);

            if (menuButton)
                menuButton.onClick.AddListener(OnMenuClicked);
        }

        private void OnDestroy()
        {
            if (nextLevelButton)
                nextLevelButton.onClick.RemoveListener(OnNextClicked);

            if (menuButton)
                menuButton.onClick.RemoveListener(OnMenuClicked);
        }

        private void OnNextClicked()
        {
            // ✨ ховаємо попап перемоги перед переходом на наступний рівень
            HideView();
            EventBus.Invoke(new GameEvents.NextLevelRequested());
        }

        private void OnMenuClicked()
        {
            // ✨ ховаємо попап перемоги перед виходом у меню
            HideView();
            EventBus.Invoke(new GameEvents.GoToMenuRequested());
        }
    }
}