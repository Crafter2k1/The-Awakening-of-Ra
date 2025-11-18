using System.Collections.Generic;
using Core.GamePlay.Levels;
using Core.GamePlay.Symboll;
using UnityEngine;
using Core.GamePlay; // якщо є GameSession

namespace Core.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        [Header("Refs")]
        public SunBeamController beam;

        [Tooltip("Позиції, де можуть спавнитись символи (по порядку).")]
        public Transform[] spawnPoints;

        [Tooltip("Всі види символів (префаби).")]
        public SymbolNode[] symbolPrefabs;

        [Header("Levels config file (JSON)")]
        public TextAsset levelsJson;

        LevelsFile _levelsFile;
        LevelData _currentLevel;

        public int currentLevelIndex = 0;

        List<SymbolNode> _spawnedSymbols = new List<SymbolNode>();
        int _lastHighlightedIndex = -1;

        void Start()
        {
            if (!beam)
            {
                Debug.LogError("GameManager: beam not set!");
                enabled = false;
                return;
            }

            if (!levelsJson)
            {
                Debug.LogError("GameManager: levelsJson (TextAsset) is not assigned!");
                enabled = false;
                return;
            }

            // load JSON
            _levelsFile = JsonUtility.FromJson<LevelsFile>(levelsJson.text);
            if (_levelsFile == null || _levelsFile.levels == null || _levelsFile.levels.Length == 0)
            {
                Debug.LogError("GameManager: no levels in JSON!");
                enabled = false;
                return;
            }

            // Select level index
            int idx = GameSession.CurrentLevelIndex;

            if (idx < 0 || idx >= _levelsFile.levels.Length)
                idx = currentLevelIndex;

            if (idx < 0 || idx >= _levelsFile.levels.Length)
                idx = 0;

            currentLevelIndex = idx;
            _currentLevel = _levelsFile.levels[idx];

            SetupLevel();
        }

        void SetupLevel()
        {
            // clear old symbols
            foreach (var s in _spawnedSymbols)
            {
                if (s != null)
                    Destroy(s.gameObject);
            }
            _spawnedSymbols.Clear();

            if (_currentLevel.symbolPrefabIndices == null || _currentLevel.symbolPrefabIndices.Length == 0)
            {
                Debug.LogError("Current level has no symbolPrefabIndices!");
                return;
            }

            int count = Mathf.Min(_currentLevel.symbolPrefabIndices.Length, spawnPoints.Length);
            Transform[] beamPoints = new Transform[count];

            for (int i = 0; i < count; i++)
            {
                int prefabIndex = _currentLevel.symbolPrefabIndices[i];

                if (prefabIndex < 0 || prefabIndex >= symbolPrefabs.Length)
                {
                    Debug.LogError($"Invalid prefab index {prefabIndex} in level {currentLevelIndex}");
                    continue;
                }

                SymbolNode prefab = symbolPrefabs[prefabIndex];
                Transform point = spawnPoints[i];

                SymbolNode instance = Instantiate(prefab, point.position, Quaternion.identity);
                instance.SetIdle();
                instance.SetHighlighted(false);

                _spawnedSymbols.Add(instance);
                beamPoints[i] = instance.transform;
            }

            _lastHighlightedIndex = -1;

            beam.Init(beamPoints, _currentLevel);
        }

        void Update()
        {
            UpdateHighlight();

            bool pressed = false;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                pressed = true;

#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                pressed = true;
#endif

            if (pressed)
                OnPlayerClick();
        }

        void UpdateHighlight()
        {
            int newIndex = -1;

            if (beam.IsInStopWindow)
                newIndex = beam.CurrentIndex;

            if (newIndex == _lastHighlightedIndex)
                return;

            // remove old highlight
            if (_lastHighlightedIndex >= 0 && _lastHighlightedIndex < _spawnedSymbols.Count)
                _spawnedSymbols[_lastHighlightedIndex].SetHighlighted(false);

            // apply new highlight
            if (newIndex >= 0 && newIndex < _spawnedSymbols.Count)
                _spawnedSymbols[newIndex].SetHighlighted(true);

            _lastHighlightedIndex = newIndex;
        }

        void OnPlayerClick()
        {
            if (!beam.IsInStopWindow)
            {
                Debug.Log("Click outside hit window");
                return;
            }

            int idx = beam.CurrentIndex;
            if (idx < 0 || idx >= _spawnedSymbols.Count)
                return;

            SymbolNode node = _spawnedSymbols[idx];

            if (!node.IsActivated)
            {
                node.Activate();
                Debug.Log($"Symbol {idx} activated!");
            }
            else
            {
                Debug.Log($"Symbol {idx} already active.");
            }
        }
    }
}
