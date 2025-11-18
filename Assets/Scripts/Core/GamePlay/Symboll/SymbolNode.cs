using UnityEngine;

namespace Core.GamePlay.Symboll
{
    public class SymbolNode : MonoBehaviour
    {
        [Header("Visuals")]
        public SpriteRenderer sprite;
        public Color idleColor = Color.white;
        public Color activeColor = Color.yellow;

        [Header("Highlight Settings")]
        [SerializeField] private float highlightScaleMul = 1.1f;

        private bool _activated;
        private Vector3 _baseScale;

        void Reset()
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        void Awake()
        {
            // базовий scale
            _baseScale = transform.localScale;

            // МОЖНА тут скинути стан один раз при створенні
            SetIdle();
            SetHighlighted(false);
        }

        // ❌ Start прибираємо зовсім, або залишаємо пустим
        // void Start() { }

        public void SetIdle()
        {
            _activated = false;
            if (sprite != null)
                sprite.color = idleColor;
        }

        public void Activate()
        {
            _activated = true;
            if (sprite != null)
                sprite.color = activeColor;
        }

        public bool IsActivated => _activated;

        public void SetHighlighted(bool highlighted)
        {
            transform.localScale = highlighted
                ? _baseScale * highlightScaleMul
                : _baseScale;
        }
    }
}