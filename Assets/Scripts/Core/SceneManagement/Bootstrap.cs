// Assets/Scripts/Core/SceneManagement/Bootstrap.cs
using UnityEngine;
using Core.Audio; // ← додай

namespace Core.SceneManagement
{
    public static class Bootstrap
    {
        private const string LoadingScreenResPath = "UI/LoadingScreen";
        private const string AudioServiceResPath  = "Systems/AudioService"; // ← нове

        private const string MainMenuScene = "MainMenuScene";
        private const float  FirstShowDelay = 2.0f;

        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            // LoadingScreen
            if (LoadingScreen.Instance == null)
            {
                var prefab = Resources.Load<LoadingScreen>(LoadingScreenResPath);
                if (prefab != null) Object.Instantiate(prefab);
                else Debug.LogError("[Bootstrap] Missing Resources/UI/LoadingScreen.prefab with LoadingScreen component.");
            }

            // AudioService (без самостворення всередині сервісу)
            if (AudioService.Instance == null)
            {
                var audioPrefab = Resources.Load<AudioService>(AudioServiceResPath);
                if (audioPrefab != null) Object.Instantiate(audioPrefab);
                else Debug.LogError("[Bootstrap] Missing Resources/Systems/AudioService.prefab with AudioService component.");
            }

            // нормалізація часу (за бажанням)
            // Time.timeScale = 1f;

            LoadingScreen.Instance?.LoadScene(MainMenuScene, FirstShowDelay);
        }
    }
}