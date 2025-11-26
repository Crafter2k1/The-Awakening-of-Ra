// Assets/Scripts/Core/Audio/UiClickSfxListener.cs

using Core.EventBusSystem;
using Core.GamePlay;
using Menu.UI;
using UnityEngine;

namespace Core.Audio
{
    public sealed class UiClickSfxListener : MonoBehaviour
    {
        [Header("SFX")]
        [SerializeField] private AudioClip clickSfx;

        private void OnEnable()
        {
            // ----- MENU -----
            EventBus.Subscribe<MenuEvents.OpenMainMenu>(OnClick);
            EventBus.Subscribe<MenuEvents.OpenSettings>(OnClick);
            EventBus.Subscribe<MenuEvents.OpenLevelSelection>(OnClick);
            EventBus.Subscribe<MenuEvents.BackRequested>(OnClick);
            EventBus.Subscribe<MenuEvents.StartGameRequested>(OnClick);
            EventBus.Subscribe<MenuEvents.QuitRequested>(OnClick);
            EventBus.Subscribe<MenuEvents.LevelChosen>(OnClick);

            // ----- GAME -----
            EventBus.Subscribe<GameEvents.PauseRequested>(OnClick);
            EventBus.Subscribe<GameEvents.ResumeRequested>(OnClick);
            EventBus.Subscribe<GameEvents.RestartRequested>(OnClick);
            EventBus.Subscribe<GameEvents.NextLevelRequested>(OnClick);
            EventBus.Subscribe<GameEvents.GoToMenuRequested>(OnClick);
        }

        private void OnDisable()
        {
            // ----- MENU -----
            EventBus.Unsubscribe<MenuEvents.OpenMainMenu>(OnClick);
            EventBus.Unsubscribe<MenuEvents.OpenSettings>(OnClick);
            EventBus.Unsubscribe<MenuEvents.OpenLevelSelection>(OnClick);
            EventBus.Unsubscribe<MenuEvents.BackRequested>(OnClick);
            EventBus.Unsubscribe<MenuEvents.StartGameRequested>(OnClick);
            EventBus.Unsubscribe<MenuEvents.QuitRequested>(OnClick);
            EventBus.Unsubscribe<MenuEvents.LevelChosen>(OnClick);

            // ----- GAME -----
            EventBus.Unsubscribe<GameEvents.PauseRequested>(OnClick);
            EventBus.Unsubscribe<GameEvents.ResumeRequested>(OnClick);
            EventBus.Unsubscribe<GameEvents.RestartRequested>(OnClick);
            EventBus.Unsubscribe<GameEvents.NextLevelRequested>(OnClick);
            EventBus.Unsubscribe<GameEvents.GoToMenuRequested>(OnClick);
        }

        // Один універсальний хендлер під будь-який тип події
        private void OnClick<T>(T _)
        {
            if (!clickSfx) return;
            if (SfxPlayer.Instance != null)
                SfxPlayer.Instance.PlayOneShot(clickSfx);
        }
    }
}
