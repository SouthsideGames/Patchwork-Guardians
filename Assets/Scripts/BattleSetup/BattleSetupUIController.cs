using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using MonsterBattleArena.BattleSetup.UI;
using System.Collections;

namespace MonsterBattleArena.BattleSetup
{
    public class BattleSetupUIController : MonoBehaviour
    {
        [SerializeField] private DraftPickQueue[] _battleSlots;
        [SerializeField] private SelectionIndicator[] _monsterSlots;
        [SerializeField] private Button _selectButton;
        [Header("Cue Text")]
        [SerializeField] private TMP_Text _cueText;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _activeColor;

        private bool _skipCurrentPhase;
        
        public void Initialize(BattleSetupManager manager)
        {
            foreach (DraftPickQueue battleSlot in _battleSlots)
            {
                battleSlot.Initialize(_normalColor);
            }

            foreach (SelectionIndicator monsterSlot in _monsterSlots)
            {
                monsterSlot.HideSlotNumber();
            }

            _selectButton.onClick.AddListener(() => {
                _skipCurrentPhase = true;
            });
        }

        public void SetMonsterSlotUI(int index, Vector2 screenPos)
        {
            if (index >= _monsterSlots.Length)
                return;
            
            SelectionIndicator ui = _monsterSlots[index];
            RectTransform rt = (RectTransform)ui.transform;
            if (rt.anchoredPosition == screenPos)
                return;

            rt.anchoredPosition = screenPos;
            ui.ShowSlotNumber();
        }

        public YieldInstruction ShowCue(string text)
        {
            Sequence sequence = DOTween.Sequence();

            _cueText.rectTransform.localScale = Vector2.one * 2.0f;
            _cueText.rectTransform.anchoredPosition = Vector2.zero;
            _cueText.alpha = 0.0f;

            _cueText.SetText(text);

            return sequence.Append(_cueText.rectTransform.DOScale(1.0f, 0.25f))
                .Join(_cueText.DOFade(1.0f, 0.15f))
                .AppendInterval(0.5f)
                .Append(_cueText.rectTransform.DOAnchorPosY(_cueText.rectTransform.rect.height * 2, 0.25f))
                .Join(_cueText.DOFade(0.0f, 0.15f))
                .WaitForCompletion();;    
        }

        public YieldInstruction StartSlotCountdown(int index, float time)
        {
            IEnumerator exec()
            {
                DraftPickQueue battleSlot = _battleSlots[index];
                battleSlot.SetColor(_activeColor);
                battleSlot.StartCountdown(time);

                yield return new WaitUntil(() => (battleSlot.Progress >= 1.0f) || _skipCurrentPhase);

                battleSlot.Finish();
                _skipCurrentPhase = false;
            }

            return StartCoroutine(exec());
        }
    
        public void SetSelectButtonVisisble(bool visible)
        {
            // TODO: Add tween animation
            _selectButton.gameObject.SetActive(visible);
        }
    }
}
