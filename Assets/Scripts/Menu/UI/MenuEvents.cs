namespace Menu.UI
{
    public class MenuEvents
    {
        public readonly struct OpenMainMenu { }
        public readonly struct OpenSettings { }
        public readonly struct OpenLevelSelection { }
        public readonly struct BackRequested { }
        public readonly struct QuitRequested { }
        public readonly struct StartGameRequested { }
        public readonly struct LevelChosen
        {
            public readonly string LevelId;
            public LevelChosen(string levelId) => LevelId = levelId;
        }
    }
}