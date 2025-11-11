namespace Core.SceneManagement
{
    public static class SceneFlow
    {
        public const string Menu = "MainMenuScene";
        public const string Game = "GameScene";

        public static void GoToMenu(float? fake = null) => LoadingScreen.Instance?.LoadScene(Menu, fake);
        public static void GoToGame(float? fake = null) => LoadingScreen.Instance?.LoadScene(Game, fake);
    }
}