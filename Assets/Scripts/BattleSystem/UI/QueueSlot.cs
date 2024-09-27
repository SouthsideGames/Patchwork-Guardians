using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleArena.BattleSystem.UI
{
    public class QueueSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _indicator;
        [SerializeField] private Color _playerColor;
        [SerializeField] private Color _opppnentColor;

        public void Set(int number, bool isPlayer)
        {
            _text.SetText(number.ToString());
            _indicator.color = isPlayer ? _playerColor : _opppnentColor;
        }

        public void Show(System.Action onComplete, bool useTransition = false)
        {
            SetScale(1, useTransition ? 0.25f : 0.0f, onComplete);
        }

        public void Hide(System.Action onComplete, bool useTransition = true)
        {
            SetScale(0, useTransition ? 0.25f : 0.0f, onComplete);
        }

        private void SetScale(float scale, float duration, System.Action onComplete)
        {
            if (duration <= 0)
            {
                transform.localScale = Vector3.one * scale;
                onComplete?.Invoke();
                return;
            }

            transform.DOScale(scale, duration).OnComplete(() => onComplete?.Invoke());
        }
    }
}
