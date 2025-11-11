using System;
using Core.EventBus;
using UnityEngine;
using UnityEngine.UI;
// LevelChosen, BackRequested

namespace Menu.UI.Popups
{
    public sealed class LevelSelectionPopup : BasePopup
    {
        [Serializable]
        private struct LevelEntry
        {
            public Button button;   // кнопка рівня
            public string levelId;  // або sceneName (як тобі зручніше)
        }

        [Header("Buttons")]
        [SerializeField] private LevelEntry[] levels;
        [SerializeField] private Button backButton;

        protected override void Awake()
        {
            base.Awake();

            if (levels != null)
            {
                for (int i = 0; i < levels.Length; i++)
                {
                    var entry = levels[i]; // копія для коректного замикання
                    if (entry.button)
                        entry.button.onClick.AddListener(() => OnLevelClicked(entry.levelId));
                }
            }

            if (backButton) backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnDestroy()
        {
            if (levels != null)
            {
                for (int i = 0; i < levels.Length; i++)
                {
                    var entry = levels[i];
                    if (entry.button)
                        entry.button.onClick.RemoveAllListeners(); // або зберігати делегати і знімати точково
                }
            }

            if (backButton) backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void OnLevelClicked(string levelId)
        {
            EventBus.Invoke(new MenuEvents.LevelChosen(levelId));
            // або, якщо стартуєш без збереження id:
            // EventBus.Invoke(new StartGameRequested());
        }

        private void OnBackClicked() => EventBus.Invoke(new MenuEvents.BackRequested());
    }
}