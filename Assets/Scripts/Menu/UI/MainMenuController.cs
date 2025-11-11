using UnityEngine;
using Core.EventBus;
using Core.SceneManagement;
using Menu.UI.Popups;

namespace Menu.UI
{

    public sealed class MainMenuController : MonoBehaviour
    {
        [Header("Popups")]
        [SerializeField] private MainMenuPopup mainMenuPopup;
        [SerializeField] private SettingsPopup settingsPopup;
        [SerializeField] private LevelSelectionPopup levelSelectionPopup;

        [Header("Config")]
        [SerializeField, Tooltip("Штучна доводка 75→100% при переході в гру")]
        private float startGameFakeFinish = 1.5f;

        private enum Panel { None, Main, Settings, Levels }
        private Panel _current = Panel.None;
        private bool _transitioning;

        private void OnEnable()
        {
            // Відкриття екранів
            EventBus.Subscribe<MenuEvents.OpenMainMenu>(OnOpenMain);
            EventBus.Subscribe<MenuEvents.OpenSettings>(OnOpenSettings);
            EventBus.Subscribe<MenuEvents.OpenLevelSelection>(OnOpenLevels);
            EventBus.Subscribe<MenuEvents.BackRequested>(OnBack);

            // Дії
            EventBus.Subscribe<MenuEvents.QuitRequested>(OnQuit);
            EventBus.Subscribe<MenuEvents.StartGameRequested>(OnStartGame);
            EventBus.Subscribe<MenuEvents.LevelChosen>(OnLevelChosen);

            // Коли меню-сцена готова — показати головне меню
            EventBus.Subscribe<SceneReady>(OnSceneReady);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<MenuEvents.OpenMainMenu>(OnOpenMain);
            EventBus.Unsubscribe<MenuEvents.OpenSettings>(OnOpenSettings);
            EventBus.Unsubscribe<MenuEvents.OpenLevelSelection>(OnOpenLevels);
            EventBus.Unsubscribe<MenuEvents.BackRequested>(OnBack);

            EventBus.Unsubscribe<MenuEvents.QuitRequested>(OnQuit);
            EventBus.Unsubscribe<MenuEvents.StartGameRequested>(OnStartGame);
            EventBus.Unsubscribe<MenuEvents.LevelChosen>(OnLevelChosen);

            EventBus.Unsubscribe<SceneReady>(OnSceneReady);
        }

        // ---------- Event handlers ----------
        private void OnSceneReady(SceneReady e)
        {
            if (e.Name == SceneFlow.Menu)
                ShowMain();
        }

        private void OnOpenMain(MenuEvents.OpenMainMenu _)=> ShowMain();
        private void OnOpenSettings(MenuEvents.OpenSettings _)=> ShowSettings();
        private void OnOpenLevels(MenuEvents.OpenLevelSelection _)=> ShowLevels();
        private void OnBack(MenuEvents.BackRequested _)=> ShowMain();

        private void OnQuit(MenuEvents.QuitRequested _)
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void OnStartGame(MenuEvents.StartGameRequested _)
        {
            SceneFlow.GoToGame(startGameFakeFinish);
        }

        private void OnLevelChosen(MenuEvents.LevelChosen e)
        {
            // TODO: зберегти e.LevelId у своєму GameData/Session, якщо потрібно
            SceneFlow.GoToGame(startGameFakeFinish);
        }

        // ---------- UI switching ----------
        private void HideAll()
        {
            if (mainMenuPopup) mainMenuPopup.HideView();
            if (settingsPopup) settingsPopup.HideView();
            if (levelSelectionPopup) levelSelectionPopup.HideView();
        }

        private void ShowMain()
        {
            if (_transitioning || _current == Panel.Main) return;
            _transitioning = true;
            HideAll();
            mainMenuPopup?.ShowView();
            _current = Panel.Main;
            _transitioning = false;
        }

        private void ShowSettings()
        {
            if (_transitioning || _current == Panel.Settings) return;
            _transitioning = true;
            HideAll();
            settingsPopup?.ShowView();
            _current = Panel.Settings;
            _transitioning = false;
        }

        private void ShowLevels()
        {
            if (_transitioning || _current == Panel.Levels) return;
            _transitioning = true;
            HideAll();
            levelSelectionPopup?.ShowView();
            _current = Panel.Levels;
            _transitioning = false;
        }
    }

    // ===== Події меню =====
    // Поклади це в окремий файл MenuEvents.cs, якщо ще не створив.
  
}
