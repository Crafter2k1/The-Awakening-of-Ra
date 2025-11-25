// LevelsConfig.cs
using System;

namespace Core.GamePlay.Levels
{
    [Serializable]
    public class LevelData
    {
        // Які символи спавнити (індекси в масиві symbolPrefabs)
        public int[] symbolPrefabIndices;

        // В який spawnPoint ставити кожен символ (індекси в масиві spawnPoints у GameManager)
        public int[] spawnPointIndices;

        // 👇 НОВЕ: у якому порядку промінь буде їх показувати / гравець має клікати
        // значення – індекси в списку _spawnedSymbols (0..N-1)
        public int[] sequenceIndices;

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