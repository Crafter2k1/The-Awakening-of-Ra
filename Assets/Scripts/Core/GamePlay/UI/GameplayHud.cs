// Assets/Scripts/Core/GamePlay/UI/GameplayHud.cs
using Core.EventBusSystem;
using UnityEngine;
using UnityEngine.UI;
using Menu.UI.Popups;
using TMPro; // BasePopup

namespace Core.GamePlay.UI
{
    public sealed class GameplayHud : BasePopup
    {
        [Header("Buttons")]
        [SerializeField] private Button pauseButton;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI levelText; // або TMP_Text, якщо хочеш — тоді поміняй тип

        protected override void Awake()
        {
            base.Awake();

            if (pauseButton)
                pauseButton.onClick.AddListener(OnPauseClicked);
        }

        private void OnDestroy()
        {
            if (pauseButton)
                pauseButton.onClick.RemoveListener(OnPauseClicked);
        }

        private void OnPauseClicked()
        {
            EventBus.Invoke(new GameEvents.PauseRequested());
        }

        public void SetLevel(int levelNumber)
        {
            if (!levelText) return;
            levelText.text = $"Level {levelNumber}";
        }
    }
}