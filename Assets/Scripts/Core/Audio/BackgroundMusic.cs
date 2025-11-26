using System;
using Core.EventBusSystem;
using Core.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class BackgroundMusic : MonoBehaviour
    {
        public static BackgroundMusic Instance { get; private set; }

        [Header("Mixer")]
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        [Header("Music per scene")]
        [SerializeField] private SceneMusic[] sceneTracks;

        [Serializable]
        private struct SceneMusic
        {
            public string sceneName;   // "MainMenuScene", "GameScene" і т.д.
            public AudioClip clip;
        }

        private AudioSource _source;
        private string _currentScene;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _source = GetComponent<AudioSource>();
            _source.loop = true;
            _source.playOnAwake = false;

            if (musicMixerGroup)
                _source.outputAudioMixerGroup = musicMixerGroup;
        }

        private void OnEnable()
        {
            EventBus.Subscribe<SceneLoaded>(OnSceneLoaded);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SceneLoaded>(OnSceneLoaded);
        }

        private void OnSceneLoaded(SceneLoaded e)
        {
            PlayForScene(e.Name);
        }

        private void PlayForScene(string sceneName)
        {
            if (_currentScene == sceneName)
                return;

            var clip = FindClipForScene(sceneName);
            if (clip == null)
                return;

            _currentScene = sceneName;
            _source.clip = clip;
            _source.Play();
        }

        private AudioClip FindClipForScene(string sceneName)
        {
            for (int i = 0; i < sceneTracks.Length; i++)
            {
                if (sceneTracks[i].sceneName == sceneName)
                    return sceneTracks[i].clip;
            }

            return null;
        }
    }
}
