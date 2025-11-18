#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Core.GamePlay.Levels;
using UnityEditor;
using UnityEngine;
// LevelData, LevelsFile

namespace Editor
{
    public class LevelsJsonGeneratorWindow : EditorWindow
    {
        [Serializable]
        private class RangeRule
        {
            public int fromLevel = 1;  // включно
            public int toLevel = 1;    // включно
            public int symbolsCount = 3;
        }

        [Header("Основні налаштування")]
        private int _totalLevels = 10;
        private int _symbolTypesCount = 3; // скільки у тебе symbolPrefabs у GameManager

        [Header("Файл")]
        private string _outputPath = "Assets/Levels/levels.json";

        [Header("Правила кількості символів")]
        private List<RangeRule> _rules = new List<RangeRule>()
        {
            new RangeRule { fromLevel = 1, toLevel = 10, symbolsCount = 3 }
        };

        [MenuItem("Tools/Levels/Generate Levels JSON")]
        public static void Open()
        {
            GetWindow<LevelsJsonGeneratorWindow>("Levels JSON Generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Генератор levels.json", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _totalLevels = EditorGUILayout.IntField("Кількість рівнів", _totalLevels);
            _totalLevels = Mathf.Max(1, _totalLevels);

            _symbolTypesCount = EditorGUILayout.IntField("Кількість типів символів", _symbolTypesCount);
            _symbolTypesCount = Mathf.Max(1, _symbolTypesCount);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Шлях до JSON (Assets-relative)", EditorStyles.boldLabel);
            _outputPath = EditorGUILayout.TextField("Output path", _outputPath);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Правила: з якого по який рівень — скільки символів", EditorStyles.boldLabel);

            // малюємо список правил
            if (_rules == null)
                _rules = new List<RangeRule>();

            int removeIndex = -1;

            for (int i = 0; i < _rules.Count; i++)
            {
                var r = _rules[i];
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Правило {i + 1}", EditorStyles.miniBoldLabel);
                r.fromLevel = EditorGUILayout.IntField("З рівня (включно)", r.fromLevel);
                r.toLevel   = EditorGUILayout.IntField("По рівень (включно)", r.toLevel);
                r.symbolsCount = EditorGUILayout.IntField("Кількість символів", r.symbolsCount);

                r.fromLevel = Mathf.Max(1, r.fromLevel);
                r.toLevel   = Mathf.Max(r.fromLevel, r.toLevel);
                r.symbolsCount = Mathf.Max(1, r.symbolsCount);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Видалити правило", GUILayout.MaxWidth(160)))
                    removeIndex = i;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (removeIndex >= 0 && removeIndex < _rules.Count)
            {
                _rules.RemoveAt(removeIndex);
            }

            if (GUILayout.Button("Додати правило"))
            {
                _rules.Add(new RangeRule
                {
                    fromLevel = 1,
                    toLevel = Mathf.Max(1, _totalLevels),
                    symbolsCount = 3
                });
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Згенерувати JSON", GUILayout.Height(40)))
            {
                GenerateJson();
            }
        }

        private void GenerateJson()
        {
            if (_totalLevels <= 0)
            {
                Debug.LogError("[LevelsJsonGenerator] totalLevels <= 0");
                return;
            }

            if (_symbolTypesCount <= 0)
            {
                Debug.LogError("[LevelsJsonGenerator] symbolTypesCount <= 0");
                return;
            }

            if (string.IsNullOrWhiteSpace(_outputPath))
            {
                Debug.LogError("[LevelsJsonGenerator] outputPath is empty");
                return;
            }

            // нормалізуємо шлях
            string fullPath = _outputPath;
            if (!fullPath.StartsWith("Assets"))
            {
                fullPath = "Assets/" + fullPath.TrimStart('/');
            }

            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var levelsFile = new LevelsFile
            {
                levels = new LevelData[_totalLevels]
            };

            for (int levelIndex = 0; levelIndex < _totalLevels; levelIndex++)
            {
                int levelNumber = levelIndex + 1; // для людини — рівні з 1

                int symbolsCount = GetSymbolsCountForLevel(levelNumber);
                if (symbolsCount <= 0)
                    symbolsCount = 1;

                var levelData = new LevelData
                {
                    symbolPrefabIndices = new int[symbolsCount],
                    // тут можна за замовчуванням якісь базові параметри складності
                    moveSpeed = 5f,
                    stopDuration = 0.5f,
                    hitWindow = 0.3f
                };

                // Заповнюємо індекси символів — циклічно по кількості типів
                for (int i = 0; i < symbolsCount; i++)
                {
                    levelData.symbolPrefabIndices[i] = i % _symbolTypesCount;
                }

                levelsFile.levels[levelIndex] = levelData;
            }

            string json = JsonUtility.ToJson(levelsFile, true);
            File.WriteAllText(fullPath, json);
            AssetDatabase.Refresh();

            Debug.Log($"[LevelsJsonGenerator] JSON з рівнями згенерований: {fullPath}");
        }

        private int GetSymbolsCountForLevel(int levelNumber)
        {
            // шукаємо перше правило, яке підходить по діапазону
            foreach (var rule in _rules)
            {
                if (levelNumber >= rule.fromLevel && levelNumber <= rule.toLevel)
                    return rule.symbolsCount;
            }

            // якщо нічого не підходить — 1 символ за замовчуванням
            return 1;
        }
    }
}
#endif
