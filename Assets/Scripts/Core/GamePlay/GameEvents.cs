namespace Core.GamePlay
{
    public static class GameEvents
    {
        public readonly struct PauseRequested { }
        public readonly struct ResumeRequested { }

        public readonly struct RestartRequested { }
        public readonly struct NextLevelRequested { }
        public readonly struct GoToMenuRequested { }

        public readonly struct LevelCompleted { }
        public readonly struct LevelFailed { }
    }
}