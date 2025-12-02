// Assets/Scripts/Core/SceneManagement/Bootstrap.cs
using System;
using System.Reflection;
using UnityEngine;
using Core.Audio;

namespace Core.SceneManagement
{
    public static class Bootstrap
    {
        private const string LoadingScreenResPath = "UI/LoadingScreen";
        private const string AudioServiceResPath  = "Systems/AudioService";

        private const string MainMenuScene = "MainMenuScene";
        private const float  FirstShowDelay = 2.0f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            // ✅ КЛЮЧОВЕ: запускаємось тільки якщо LoadingService це дозволяє
            if (!IsAllowedByLoadingService(MainMenuScene))
                return;

            if (LoadingScreen.Instance == null)
            {
                var prefab = Resources.Load<LoadingScreen>(LoadingScreenResPath);
                if (prefab != null) UnityEngine.Object.Instantiate(prefab);
                else Debug.LogError("[Bootstrap] Missing Resources/UI/LoadingScreen.prefab with LoadingScreen component.");
            }

            if (AudioService.Instance == null)
            {
                var audioPrefab = Resources.Load<AudioService>(AudioServiceResPath);
                if (audioPrefab != null) UnityEngine.Object.Instantiate(audioPrefab);
                else Debug.LogError("[Bootstrap] Missing Resources/Systems/AudioService.prefab with AudioService component.");
            }

            LoadingScreen.Instance?.LoadScene(MainMenuScene, FirstShowDelay);
        }

        /// <summary>
        /// НЕ торкаючись LoadingService, намагаємося зрозуміти, чи він дозволяє запуск сцени.
        /// Підтримує декілька поширених сигнатур:
        ///  - public static bool Allows(string sceneName)
        ///  - public static bool CanStart(string sceneName)
        ///  - public static string SceneName / TargetScene / PlannedScene / _sceneName (дорівнює нашому sceneName)
        ///  - опціонально public static bool IsEnabled / AllowBootstrap (повинно бути true)
        /// Якщо нічого з цього не знайдено — повертаємо false (консервативна політика).
        /// </summary>
        private static bool IsAllowedByLoadingService(string sceneName)
        {
            // Спробуємо кілька можливих повних імен типу
            var candidates = new[]
            {
                "Core.SceneManagement.LoadingService",
                "LoadingService"
            };

            Type svcType = null;
            foreach (var fullName in candidates)
            {
                svcType = Type.GetType(fullName);
                if (svcType != null) break;
            }

            if (svcType == null)
            {
                // LoadingService відсутній у збірці — забороняємо.
                return false;
            }

            try
            {
                // 1) Методи типу Allows/CanStart
                var method = svcType.GetMethod("Allows", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null)
                             ?? svcType.GetMethod("CanStart", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);

                if (method != null)
                {
                    var ok = method.Invoke(null, new object[] { sceneName });
                    if (ok is bool b1) return b1;
                }

                // 2) Додатковий вмикач (IsEnabled/AllowBootstrap) — якщо є, повинен бути true
                bool? enabledGate = TryReadBoolStatic(svcType, "IsEnabled")
                                    ?? TryReadBoolStatic(svcType, "AllowBootstrap")
                                    ?? (bool?)null;

                if (enabledGate.HasValue && !enabledGate.Value)
                    return false;

                // 3) Статичні поля/властивості з назвою сцени
                var planned = TryReadStringStatic(svcType, "SceneName")
                              ?? TryReadStringStatic(svcType, "TargetScene")
                              ?? TryReadStringStatic(svcType, "PlannedScene")
                              ?? TryReadStringStatic(svcType, "_sceneName");

                if (!string.IsNullOrEmpty(planned))
                    return string.Equals(planned, sceneName, StringComparison.Ordinal);

                // Нічого не змогли визначити — не запускаємось
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[Bootstrap] LoadingService check failed: {ex.Message}");
                return false;
            }
        }

        private static string TryReadStringStatic(Type t, string name)
        {
            var f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (f != null && f.FieldType == typeof(string))
                return (string)f.GetValue(null);

            var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (p != null && p.PropertyType == typeof(string) && p.CanRead)
                return (string)p.GetValue(null);

            return null;
        }

        private static bool? TryReadBoolStatic(Type t, string name)
        {
            var f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (f != null && f.FieldType == typeof(bool))
                return (bool)f.GetValue(null);

            var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (p != null && p.PropertyType == typeof(bool) && p.CanRead)
                return (bool)p.GetValue(null);

            return null;
        }
    }
}
