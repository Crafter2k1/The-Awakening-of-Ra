// Assets/Scripts/Core/Audio/SfxPlayer.cs
using UnityEngine;

namespace Core.Audio
{
    [DefaultExecutionOrder(-110)]
    public sealed class SfxPlayer : MonoBehaviour
    {
        public static SfxPlayer Instance { get; private set; }

        [SerializeField] private AudioSource audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            // важливо: вистави тут Output на SFX групу в AudioMixer через інспектор
        }

        public void PlayOneShot(AudioClip clip)
        {
            if (!clip || audioSource == null) return;
            audioSource.PlayOneShot(clip);
        }
    }
}