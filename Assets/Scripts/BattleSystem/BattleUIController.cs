using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MonsterBattleArena.BattleSystem;
using MonsterBattleArena.BattleSystem.UI;
using MonsterBattleArena.Monster;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleArena
{
    public class BattleUIController : MonoBehaviour
    {
        [SerializeField] private BattleUnitStatus _battleUnitStatus;
        [SerializeField] private QueueSlot _queueTemplate;
        [SerializeField] private Image _turnIndicator;
        [SerializeField] private GameSpeedControl _gameSpeedControl;
        [SerializeField] private TMP_Text _popupText;

        private List<QueueSlot> _queue;

        /* 
        *   NOTE:
        *       Reference to BattleManager for initialization, currently the ui is being  
        *       controlled by the battle manager
        */
        public void Initialize(BattleManager battleManager)
        {
            _popupText.gameObject.SetActive(false);

            _battleUnitStatus.gameObject.SetActive(false);
            _queueTemplate.gameObject.SetActive(false);
            _gameSpeedControl.Initialize(battleManager);

            InitializeTurnIndicator();

            _queue = new List<QueueSlot>();
        }

        public void InitializeUnitStatus(BattleUnit battleUnit, MonsterSlot slot)
        {
            BattleUnitStatus status = Instantiate(_battleUnitStatus, _battleUnitStatus.transform.parent);
            RectTransform rt = (RectTransform)status.transform;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(slot.transform.position + (Vector3.up * slot.Rig.GetBounds().size.y));
            rt.anchoredPosition = screenPos + (Vector2.up * (rt.sizeDelta.y * 1.5f));

            status.Initialize(battleUnit);

            status.gameObject.SetActive(true);
        }

        private void InitializeTurnIndicator()
        {
            _turnIndicator.gameObject.SetActive(false);

            RectTransform rt = _turnIndicator.rectTransform;
            rt.position += Vector3.up * rt.rect.height;
        }

        public void SetQueue(IEnumerable<BattleManager.QueueEntry> queue)
        {
            ClearQueue();

            foreach (BattleManager.QueueEntry entry in queue)
            {
                QueueSlot slot = Instantiate(_queueTemplate, _queueTemplate.transform.parent);
                slot.gameObject.SetActive(true);
                slot.Set(entry.BattleUnit.UnitIndex, entry.IsPlayer);

                slot.transform.localScale = Vector2.zero;
                slot.transform.DOScale(1.0f, 0.15f);

                _queue.Add(slot);
            }
        }

        public YieldInstruction ClearQueue()
        {
            IEnumerator exec()
            {
                HideTurnIndicator();
                
                YieldInstruction delay = new WaitForSeconds(0.055f);
                for (int i = _queue.Count - 1; i >= 0; i--)
                {
                    QueueSlot slot = _queue[i];
                    slot.Hide(() => Destroy(slot.gameObject, 0.2f));

                    yield return delay;
                }

                _queue.Clear();
            }

            return StartCoroutine(exec());
        }
    
        public YieldInstruction ShowTurnIndicator(int index)
        {
            IEnumerator exec()
            {
                yield return HideTurnIndicator();

                RectTransform target = (RectTransform)_queue[index].transform;
                Vector3 targetPos = target.transform.position + (Vector3.up * target.rect.height);

                RectTransform indicator = _turnIndicator.rectTransform;
                indicator.position = new Vector3(targetPos.x, indicator.position.y, 0);

                _turnIndicator.gameObject.SetActive(true);
                yield return DOTween.Sequence()
                    .Append(indicator.DOMoveY(targetPos.y, 0.15f))
                    .Join(_turnIndicator.DOFade(1.0f, 0.15f))
                    .WaitForCompletion();
            }

            return StartCoroutine(exec());
        }

        public YieldInstruction HideTurnIndicator()
        {
            RectTransform rt = _turnIndicator.rectTransform;

            return DOTween.Sequence()
                .Append(rt.DOMoveY(rt.position.y + rt.rect.height, 0.15f))
                .Join(_turnIndicator.DOFade(0.0f, 0.15f))
                .WaitForCompletion();
        }
    }
}
