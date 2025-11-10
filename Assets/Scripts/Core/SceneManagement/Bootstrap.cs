using UnityEngine;

namespace Core.SceneManagement
{
    public static class Bootstrap
    {
        private const string LoadingScreenResPath = "UI/LoadingScreen"; 
        private const string MainMenuScene = "MainMenuScene";
        private const float  FirstShowDelay = 2.0f; 

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (LoadingScreen.Instance == null)
            {
                var prefab = Resources.Load<LoadingScreen>(LoadingScreenResPath);
                if (prefab != null) Object.Instantiate(prefab);
                else Debug.LogError("[Bootstrap] Missing Resources/UI/LoadingScreen.prefab with LoadingScreen component.");
            }
            LoadingScreen.Instance?.LoadScene(MainMenuScene, FirstShowDelay);
        }
    }
}