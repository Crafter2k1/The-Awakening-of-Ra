using System;

namespace Core.GamePlay.Levels
{
    [Serializable]
    public class LevelData
    {
        // Які символи спавнити по порядку (індекси в масиві symbolPrefabs)
        public int[] symbolPrefabIndices;

        // Параметри складності для цього рівня
        public float moveSpeed = 5f;
        public float stopDuration = 0.5f;
        public float hitWindow = 0.3f;
    }

    [Serializable]
    public class LevelsFile
    {
        public LevelData[] levels;
    }
}