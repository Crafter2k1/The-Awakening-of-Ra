using UnityEngine;

namespace Core.GamePlay
{
    public static class GameSession
    {
        private const string LevelIndexKey = "progress.level_index";

        /// <summary>
        /// Поточний (останній досягнутий / розблокований) рівень, 0-based.
        /// Зберігається в PlayerPrefs.
        /// </summary>
        public static int CurrentLevelIndex
        {
            get => PlayerPrefs.GetInt(LevelIndexKey, 0);
            set
            {
                if (value < 0) value = 0;
                PlayerPrefs.SetInt(LevelIndexKey, value);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Якщо хочеш ще зберігати string-id (але поки що не чіпаємо).
        /// </summary>
        public static string CurrentLevelId { get; set; } = "0";
    }
}