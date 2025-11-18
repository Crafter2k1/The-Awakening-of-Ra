using UnityEngine;

namespace Core.GamePlay.Symboll
{
    public class SymbolNode : MonoBehaviour
    {
        public SpriteRenderer sprite;
        public Color idleColor = Color.white;
        public Color activeColor = Color.yellow;

        [Header("Highlight")]
        [SerializeField] private float highlightScaleMul = 1.1f;

        bool _activated;
        Vector3 _baseScale;

        void Reset()
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }

        void Awake()
        {
            // запам'ятовуємо стартовий scale префаба
            _baseScale = transform.localScale;
        }

        void Start()
        {
            SetIdle();
            SetHighlighted(false);
        }

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