// Assets/Scripts/Core/GamePlay/GameSession.cs
namespace Core.GamePlay
{
    public static class GameSession
    {
        /// <summary>
        /// Поточний індекс рівня із JSON (0-based).
        /// </summary>
        public static int CurrentLevelIndex { get; set; } = 0;

        /// <summary>
        /// Опціонально: айді з меню (якщо юзаєш рядкові LevelId).
        /// </summary>
        public static string CurrentLevelId { get; set; } = "0";
    }
}