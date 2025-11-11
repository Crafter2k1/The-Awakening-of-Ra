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

        protected virtual void Awake() => Initial();

        protected virtual void OnDisable()
        {
            if (_fadeTween != null && _fadeTween.IsActive())
                _fadeTween.Kill();
        }

        protected virtual void Initial()
        {
            if (canvasGroup)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            if (content) content.gameObject.SetActive(false);
        }

        public virtual void ShowView()
        {
            if (_fadeTween != null && _fadeTween.IsActive())
                _fadeTween.Kill();

            if (content) content.gameObject.SetActive(true);

            if (canvasGroup)
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                _fadeTween = canvasGroup.DOFade(1f, animationDuration)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true)
                    .OnComplete(() => canvasGroup.alpha = 1f);
            }
        }

        public virtual void HideView()
        {
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
