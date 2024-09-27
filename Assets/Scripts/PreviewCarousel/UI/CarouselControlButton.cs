using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MonsterBattleArena.PreviewCarousel.UI
{
    public class CarouselControlButton : MonoBehaviour, IPointerClickHandler
    {
        private enum Direction { Left = -1, Right = 1 }

        [SerializeField] private float _animationDuration = 0.15f;
        [SerializeField] private Direction _dir;

        private bool _inTransition = false;

        public event System.Action OnClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_inTransition)
                return;
            
            _inTransition = true;

            OnClick?.Invoke();

            RectTransform rt = (RectTransform)transform;
            rt.DOPunchAnchorPos(Vector2.right * rt.rect.width * 0.15f * (int)_dir, _animationDuration)
                .OnComplete(() => { 
                    _inTransition = false;
                });
        }
    }
}
