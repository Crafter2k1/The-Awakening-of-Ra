using DG.Tweening;
using UnityEngine;

namespace Menu.UI.Popups
{
    public class BasePopup : MonoBehaviour
    {
        [SerializeField] protected Transform content;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] private float animationDuration = 0.5f;

        private Tween _fadeTween;

        // ВАЖЛИВО: НІЯКОГО Initial() В AWAKE!
        // Стан початкової видимості задаєш у префабі/сцені.
        protected virtual void Awake()
        {
            Debug.Log($"[Popup] Awake {name} (content.activeSelf={content?.gameObject.activeSelf}, alpha={canvasGroup?.alpha})", this);

            if (!canvasGroup)
                Debug.LogWarning($"[Popup] {name} CanvasGroup НЕ заданий", this);
            if (!content)
                Debug.LogWarning($"[Popup] {name} Content НЕ заданий", this);
        }

        protected virtual void OnDisable()
        {
            if (_fadeTween != null && _fadeTween.IsActive())
                _fadeTween.Kill();
        }

        public virtual void ShowView()
        {
            Debug.Log($"[Popup] ShowView {name}", this);

            if (_fadeTween != null && _fadeTween.IsActive())
                _fadeTween.Kill();

            if (content) content.gameObject.SetActive(true);

            if (canvasGroup)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                // миттєво піднімаємо alpha до 1, або хочеш — залиш tween
                _fadeTween = canvasGroup.DOFade(1f, animationDuration)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
                    .OnComplete(() => canvasGroup.alpha = 1f);
            }
        }

        public virtual void HideView()
        {
            Debug.Log($"[Popup] HideView {name}", this);

            if (_fadeTween != null && _fadeTween.IsActive())
                _fadeTween.Kill();

            if (canvasGroup)
            {
                _fadeTween = canvasGroup.DOFade(0f, animationDuration)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        if (content) content.gameObject.SetActive(false);
                        canvasGroup.interactable = false;
                        canvasGroup.blocksRaycasts = false;
                    });
            }
            else
            {
                if (content) content.gameObject.SetActive(false);
            }
        }
    }
}
