using DG.Tweening;
using UnityEngine;

namespace MonsterBattleArena.BattleSetup.UI
{
    public class SelectionIndicator : MonoBehaviour
    {
        [SerializeField] private RectTransform _slotNumberBg;

        private Sequence _currentSequence;

        public void ShowSlotNumber()
        {
            if (_currentSequence != null)
                _currentSequence.Kill();

            _slotNumberBg.gameObject.SetActive(true);
            _slotNumberBg.localScale = Vector2.zero;

            _currentSequence = DOTween.Sequence();
            _currentSequence.Append(_slotNumberBg.DOScaleX(1, 0.15f))
                .Join(_slotNumberBg.DOScaleY(1.1f, 0.25f))
                .Append(_slotNumberBg.DOScaleY(1.0f, 0.15f));
        }

        public void HideSlotNumber()
        {
            _slotNumberBg.localScale = Vector2.zero;
            _slotNumberBg.gameObject.SetActive(false);
        }
    }
}
