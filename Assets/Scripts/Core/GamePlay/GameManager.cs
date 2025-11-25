using System.Collections.Generic;
using Core.EventBusSystem;
using Core.GamePlay.Levels;
using Core.GamePlay.Symboll;
using Core.GamePlay.UI;
using Core.SceneManagement;
using UnityEngine;

namespace Core.GamePlay
{
    /// <summary>
    /// Логіка рівня:
    /// 1) Спавнить символи згідно з JSON.
    /// 2) Генерує послідовність (зараз: усі символи по порядку).
    /// 3) ФАЗА ПОКАЗУ: промінь літає по послідовності, символи підсвічуються.
    /// 4) ФАЗА ВВОДУ: гравець клікає по символах у тому ж порядку.
    ///    - якщо клік правильний -> рухаємось далі;
    ///    - якщо помилковий -> LevelFailed();
    ///    - якщо вся послідовність пройдена -> LevelComplete().
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Refs")]
        public SunBeamController beam;

        [Tooltip("Позиції, де можуть спавнитись символи (по порядку).")]
        public Transform[] spawnPoints;

        [Tooltip("Всі види символів (префаби).")]
        public SymbolNode[] symbolPrefabs;

        [Header("Levels config file (JSON)")]
        public TextAsset levelsJson;   // сюди підкидаємо згенерований levels.json

        LevelsFile _levelsFile;
        LevelData _currentLevel;

        [Header("Debug / Manual")]
        [Tooltip("Який індекс рівня брати з JSON (0-based) при запуску сцени напряму.")]
        public int currentLevelIndex = 0;

        [Header("UI")]
        [SerializeField] private GameplayHud gameplayHud;

        readonly List<SymbolNode> _spawnedSymbols = new List<SymbolNode>();

        // послідовність, яку показуємо і яку гравець має повторити
        int[] _sequence;
        int _inputIndex = 0;

        bool _isShowingSequence = false;
        bool _isInputPhase = false;
        bool _levelFinished = false;
        bool _isPaused = false;

        int _lastHighlightedIndex = -1;

        private void OnEnable()
        {
            EventBus.Subscribe<GameEvents.RestartRequested>(OnRestartRequested);
            EventBus.Subscribe<GameEvents.NextLevelRequested>(OnNextLevelRequested);
            EventBus.Subscribe<GameEvents.GoToMenuRequested>(OnGoToMenuRequested);
            EventBus.Subscribe<GameEvents.PauseRequested>(OnPauseRequested);
            EventBus.Subscribe<GameEvents.ResumeRequested>(OnResumeRequested);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameEvents.RestartRequested>(OnRestartRequested);
            EventBus.Unsubscribe<GameEvents.NextLevelRequested>(OnNextLevelRequested);
            EventBus.Unsubscribe<GameEvents.GoToMenuRequested>(OnGoToMenuRequested);
            EventBus.Unsubscribe<GameEvents.PauseRequested>(OnPauseRequested);
            EventBus.Unsubscribe<GameEvents.ResumeRequested>(OnResumeRequested);
        }

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

            // розпарсити JSON
            _levelsFile = JsonUtility.FromJson<LevelsFile>(levelsJson.text);
            if (_levelsFile == null || _levelsFile.levels == null || _levelsFile.levels.Length == 0)
            {
                Debug.LogError("GameManager: no levels in JSON!");
                enabled = false;
                return;
            }

            if (currentLevelIndex < 0 || currentLevelIndex >= _levelsFile.levels.Length)
                currentLevelIndex = 0;

            _currentLevel = _levelsFile.levels[currentLevelIndex];

            if (gameplayHud != null)
                gameplayHud.SetLevel(currentLevelIndex);

            SetupLevel();
            StartShowSequence();
        }

        void SetupLevel()
        {
            // видалити старі символи, якщо були
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
            _levelFinished = false;
            _isShowingSequence = false;
            _isInputPhase = false;
            _isPaused = false;
            Time.timeScale = 1f;

            // послідовність поки що: усі символи по порядку
            _sequence = new int[_spawnedSymbols.Count];
            for (int i = 0; i < _sequence.Length; i++)
                _sequence[i] = i;
        }

        /// <summary>
        /// Запускаємо ФАЗУ ПОКАЗУ: промінь ходить по символах у порядку _sequence.
        /// </summary>
        void StartShowSequence()
        {
            if (_sequence == null || _sequence.Length == 0)
            {
                Debug.LogError("GameManager: sequence is empty, nothing to show.");
                return;
            }

            _isShowingSequence = true;
            _isInputPhase = false;
            _inputIndex = 0;
            _lastHighlightedIndex = -1;

            Transform[] seqPoints = new Transform[_sequence.Length];
            for (int i = 0; i < _sequence.Length; i++)
            {
                int idx = _sequence[i];
                seqPoints[i] = _spawnedSymbols[idx].transform;
            }

            // Використовуємо moveSpeed / stopDuration з LevelData як параметри показу
            beam.PlaySequence(
                seqPoints,
                _currentLevel.moveSpeed,
                _currentLevel.stopDuration,
                OnShowStep,
                OnShowFinished
            );
        }

        /// <summary>
        /// Викликається SunBeamController для кожного кроку показу.
        /// крок i відповідає _sequence[i].
        /// </summary>
        void OnShowStep(int stepIndex)
        {
            int symbolIndex = _sequence[stepIndex];

            // гасимо попередній highlight
            if (_lastHighlightedIndex >= 0 && _lastHighlightedIndex < _spawnedSymbols.Count)
                _spawnedSymbols[_lastHighlightedIndex].SetHighlighted(false);

            // підсвічуємо поточний
            if (symbolIndex >= 0 && symbolIndex < _spawnedSymbols.Count)
            {
                _spawnedSymbols[symbolIndex].SetHighlighted(true);
                _lastHighlightedIndex = symbolIndex;
            }
        }

        /// <summary>
        /// Викликається, коли показ усієї послідовності завершений.
        /// </summary>
        void OnShowFinished()
        {
            // гасимо останній highlight
            if (_lastHighlightedIndex >= 0 && _lastHighlightedIndex < _spawnedSymbols.Count)
                _spawnedSymbols[_lastHighlightedIndex].SetHighlighted(false);

            _lastHighlightedIndex = -1;

            _isShowingSequence = false;
            _isInputPhase = true;
            _inputIndex = 0;

            Debug.Log("GameManager: sequence show finished. Waiting for player input...");
        }

        void Update()
        {
            if (_levelFinished || _isPaused)
                return;

            if (!_isInputPhase)
                return;

            bool pressed = false;
            Vector3 screenPos = Vector3.zero;

#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                pressed = true;
                screenPos = Input.GetTouch(0).position;
            }
            if (Input.GetMouseButtonDown(0))
            {
                pressed = true;
                screenPos = Input.mousePosition;
            }
#else 
            if (Input.GetMouseButtonDown(0))
            {
                pressed = true;
                screenPos = Input.mousePosition;
            }
#endif

            if (pressed)
            {
                HandleInputClick(screenPos);
            }
        }

        void HandleInputClick(Vector3 screenPos)
        {
            if (Camera.main == null)
            {
                Debug.LogError("GameManager: no main camera for raycast.");
                return;
            }

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            Vector2 pos2D = new Vector2(worldPos.x, worldPos.y);

            var hit = Physics2D.Raycast(pos2D, Vector2.zero);
            if (!hit.collider)
                return;

            var node = hit.collider.GetComponent<SymbolNode>();
            if (!node)
                return;

            int clickedIndex = _spawnedSymbols.IndexOf(node);
            if (clickedIndex < 0)
                return;

            OnSymbolClicked(clickedIndex);
        }

        void OnSymbolClicked(int clickedIndex)
        {
            if (!_isInputPhase || _levelFinished || _isPaused)
                return;

            if (_inputIndex < 0 || _inputIndex >= _sequence.Length)
                return;

            int expectedIndex = _sequence[_inputIndex];

            if (clickedIndex == expectedIndex)
            {
                // правильний клік
                _spawnedSymbols[clickedIndex].Activate();
                Debug.Log($"Correct click {clickedIndex} (step {_inputIndex + 1}/{_sequence.Length})");

                _inputIndex++;

                if (_inputIndex >= _sequence.Length)
                {
                    LevelComplete();
                }
            }
            else
            {
                Debug.Log($"Wrong click! expected={expectedIndex}, got={clickedIndex}");
                LevelFailed();
            }
        }

        void LevelComplete()
        {
            _levelFinished = true;
            _isInputPhase = false;

            Debug.Log($"LEVEL COMPLETE! Level index = {currentLevelIndex}");
            EventBus.Invoke(new GameEvents.LevelCompleted());
        }

        void LevelFailed()
        {
            _levelFinished = true;
            _isInputPhase = false;

            Debug.Log($"LEVEL FAILED! Level index = {currentLevelIndex}");
            EventBus.Invoke(new GameEvents.LevelFailed());
        }

        // ---------- HANDLERS UI PODIY ----------

        void OnPauseRequested(GameEvents.PauseRequested _)
        {
            if (_levelFinished || _isPaused) return;
            _isPaused = true;
            Time.timeScale = 0f;
        }

        void OnResumeRequested(GameEvents.ResumeRequested _)
        {
            if (!_isPaused) return;
            _isPaused = false;
            Time.timeScale = 1f;
        }

        void OnRestartRequested(GameEvents.RestartRequested _)
        {
            _isPaused = false;
            Time.timeScale = 1f;

            // достатньо скинути рівень та заново показати послідовність
            SetupLevel();
            StartShowSequence();
        }

        void OnNextLevelRequested(GameEvents.NextLevelRequested _)
        {
            _isPaused = false;
            Time.timeScale = 1f;

            int nextIndex = currentLevelIndex + 1;
            if (_levelsFile != null && _levelsFile.levels != null && nextIndex < _levelsFile.levels.Length)
            {
                currentLevelIndex = nextIndex;
                _currentLevel = _levelsFile.levels[currentLevelIndex];

                if (gameplayHud != null)
                    gameplayHud.SetLevel(currentLevelIndex);

                SetupLevel();
                StartShowSequence();
            }
            else
            {
                // якщо наступного рівня нема — кидаємо в меню
                SceneFlow.GoToMenu(0.2f);
            }
        }

        void OnGoToMenuRequested(GameEvents.GoToMenuRequested _)
        {
            _isPaused = false;
            Time.timeScale = 1f;
            SceneFlow.GoToMenu(0.2f);
        }
    }
}
