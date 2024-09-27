using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace MonsterBattleArena
{
    public class BackButton : MonoBehaviour, IPointerClickHandler
    {
        private bool _pressed;

        public void OnPointerClick(PointerEventData eventData)
        {
            // Prevent accidental double click
            if (_pressed)
                return;
            
            _pressed = true;
            
            RectTransform rt = (RectTransform)transform;
            rt.DOPunchPosition(new Vector3(-rt.rect.width * 0.15f, 0), 0.125f)
                .OnComplete(() => {
                    Backstack.PopBackstack();
                    _pressed = false;
                });
                        
        }
    }
}
