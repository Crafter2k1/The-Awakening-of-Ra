using UnityEngine;

namespace Core.SceneManagement
{
    public static class Bootstrap
    {
        private const string LoadingScreenResPath = "LoadingScreen"; 
        private const string MainMenuScene = "MainMenuScene";
        private const float  FirstShowDelay = 1.0f; 

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (LoadingScreen.Instance == null)
            {
                var prefab = Resources.Load<LoadingScreen>(LoadingScreenResPath);
                if (prefab != null) Object.Instantiate(prefab);
                else Debug.LogError("[Bootstrap] Missing Resources/LoadingScreen.prefab with LoadingScreen component.");
            }

            // 2) Нормалізувати середовище
            Time.timeScale = 1f;

            // 3) Завантажити MainMenu через LoadingScreen
            //    (У варіанті A перша сцена — порожня Init, тож просто вантажимо меню)
            LoadingScreen.Instance?.LoadScene(MainMenuScene, FirstShowDelay);
        }
    }
}