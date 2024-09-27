using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace MonsterBattleArena.BattleSetup.UI
{
    public class DraftPickQueue : MonoBehaviour
    {
        [SerializeField] private Outline _outline;
        [SerializeField] private Image _progress;
        [SerializeField] private TMP_Text _slotNumber;

        private float _time;
        private float _elapsed;

        public float Progress { get => Mathf.Clamp01(_elapsed / _time); }

        public void Initialize(Color color)
        {
            SetColor(color);
            
            _progress.rectTransform.localScale = new Vector2(0, 1.0f);
        }

        public void SetColor(Color color)
        {
            _slotNumber.color = color;
            _outline.effectColor = color;
            _progress.color = color;
        }

        public YieldInstruction StartCountdown(float time)
        {
            _time = time;

            Tween tween = _progress.rectTransform.DOScaleX(1.0f, time)
                .SetEase(Ease.Linear) 
                .SetId(this); 

            tween.OnUpdate(() => {
                _elapsed = tween.Elapsed();
            });

            return tween.WaitForCompletion();
        }

        public void Finish()
        {
            DOTween.Kill(this);

            _progress.rectTransform.localScale = Vector2.one;
            _elapsed = 1.0f;
        }
    }
}
