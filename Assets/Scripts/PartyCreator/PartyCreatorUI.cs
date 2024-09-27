using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MonsterBattleArena.PreviewCarousel.UI;
using MonsterBattleArena.UI;
using MonsterBattleArena.Monster;
using TMPro;

namespace MonsterBattleArena.PartyCreator.UI
{
    public class PartyCreatorUI : MonoBehaviour
    {
        private const float BUTTONS_DELAY = 0.0255f;

        [SerializeField] private Transform _partySlotsUIContainer;

        [Header("Monster Selection")]
        [SerializeField] private CanvasGroup _monsterSelectionGroup;
        [SerializeField] private RectTransform _sidePanelContainer;
        [SerializeField] private RectTransform _carouselControlContainer;
        [SerializeField] private Button _createMonsterButton;
        [SerializeField] private Button _editButton;
        [SerializeField] private Button _addToPartyButton;
        [SerializeField] private CarouselControlButton _prevMonster;
        [SerializeField] private CarouselControlButton _nextMonster;
        [SerializeField] private StatsPanel _statsPanel;
        [SerializeField] private TMP_Text _selectedPartyIndex;
        [SerializeField] private CarouselControlText _carouselControlText;

        private PartyCreatorManager _manager;

        public void Initialize(PartyCreatorManager manager)
        {
            _manager = manager;

            _manager.OnCarouselEntrySelected += OnCarouselEntrySelected;

            _prevMonster.OnClick += () => {
                if (_manager.PreviousMonster())
                    HideButton(_editButton);
            };

            _nextMonster.OnClick += () => {
                if (_manager.NextMonster())
                    HideButton(_editButton);
            };

            _createMonsterButton.onClick.AddListener(() => _manager.LoadMonsterCreatorScene());
            _editButton.onClick.AddListener(() => _manager.EditCurrentSelectedMonster());
            _addToPartyButton.onClick.AddListener(() => _manager.SetCurrentSelectedToPartyIndex());

            ShowPartySetupUI(null, false);
            HideButton(_editButton, false);
        }

        public void InitializePartySlotUI(MonsterSlot partySlot, int index)
        {
            PartySlotUI slot = _partySlotsUIContainer.GetChild(index).GetComponent<PartySlotUI>();
            slot.Initialize(partySlot);

            slot.SetAddButtonListener(() => {
                _manager.SelectedPartyIndex = index;
                _manager.LoadPreviewCarouselScreen();

                _selectedPartyIndex.text = (index + 1).ToString();
            });

            slot.SetEditButtonListener(() => {
                _manager.EditMonster(partySlot.MonsterId, false);
            });

            slot.SetRemoveButtonListener(() => {
                _manager.ClearPartySlot(index);
            });

            _manager.OnPartySlotChanged += (idx) => {
                if (idx != index)
                    return;

                if (!slot.IsVisible)
                    return;
                    
                slot.Hide(false);
                slot.Show();
            };
        }

        private void OnCarouselEntrySelected(int index)
        {
            MonsterData monsterData = PlayerProfileManager.CurrentProfile.GetMonsterData(index);

            if (monsterData != null)
                ShowButton(_editButton);

            UpdateStatsAndText(monsterData);
        }

        private void UpdateStatsAndText(MonsterData monsterData)
        {
            bool inParty = PlayerProfileManager.CurrentProfile.IsInParty(monsterData);

            _carouselControlText.UpdateText(monsterData?.Name, inParty);

            if (monsterData != null)
                _statsPanel.SetMonstertData(monsterData);
        }

        public void ShowPartySetupUI(System.Action onHidePrevUICompleted = null, bool useTransition = true)
        {
            IEnumerator exec()
            {
                yield return HideMonsterSelectionUI(useTransition);
                onHidePrevUICompleted?.Invoke();

                foreach (PartySlotUI slot in _partySlotsUIContainer.GetComponentsInChildren<PartySlotUI>())
                {
                    slot.Show(useTransition);
                }
            }

            StartCoroutine(exec());
        }

        private YieldInstruction HidePartySetupUI(bool useTransition = true)
        {
            IEnumerator exec()
            {
                YieldInstruction delay = new WaitForSeconds(BUTTONS_DELAY);
                foreach (PartySlotUI slot in _partySlotsUIContainer.GetComponentsInChildren<PartySlotUI>())
                {
                    slot.Hide(useTransition);
                    yield return delay;
                }
            }
            
            return StartCoroutine(exec());
        }

        public void ShowMonsterSelectionUI(System.Action onHidePrevUICompleted = null, bool useTransition = true)
        {
            float duration = useTransition ? 0.25f : 0.0f;

            IEnumerator exec()
            {
                LayoutRebuilder.MarkLayoutForRebuild(_sidePanelContainer);

                yield return HidePartySetupUI();
                onHidePrevUICompleted?.Invoke();

                _monsterSelectionGroup.interactable = true;
                _monsterSelectionGroup.blocksRaycasts = true;

                RectTransform createMonsterButton = (RectTransform)_createMonsterButton.transform;
                createMonsterButton.DOAnchorPosX(-createMonsterButton.anchoredPosition.x, duration);
                _carouselControlContainer.DOAnchorPosY(-_carouselControlContainer.anchoredPosition.y, duration);
                _sidePanelContainer.DOAnchorPosX(0, duration);
                _monsterSelectionGroup.DOFade(1.0f, duration);

                if (PlayerProfileManager.CurrentProfile.AvailableMonsters > 0)
                    ShowButton(_editButton, useTransition);
            }

            StartCoroutine(exec());
        }

        private YieldInstruction HideMonsterSelectionUI(bool useTransition = true)
        {
            float duration = useTransition ? 0.25f : 0.0f;

            IEnumerator exec()
            {
                RectTransform createMonsterButton = (RectTransform)_createMonsterButton.transform;
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_monsterSelectionGroup.transform);

                yield return new WaitForEndOfFrame();

                createMonsterButton.DOAnchorPosX(-createMonsterButton.anchoredPosition.x, duration);
                _sidePanelContainer.DOAnchorPosX(_sidePanelContainer.rect.width + (_sidePanelContainer.offsetMin.x * 2), duration);
                _carouselControlContainer.DOAnchorPosY(-_carouselControlContainer.anchoredPosition.y, duration);
                _monsterSelectionGroup.DOFade(0.0f, duration);

                HideButton(_editButton, useTransition);

                yield return new WaitForSeconds(duration);

                _monsterSelectionGroup.interactable = false;
                _monsterSelectionGroup.blocksRaycasts = false;
            }

            return StartCoroutine(exec());
        }
    
        private void ShowButton(Button button, bool useTransition = true)
        {
            if (button.gameObject.activeSelf)
                return;

            Transform t = button.transform;
            button.gameObject.SetActive(true);

            if (!useTransition)
            {
                t.transform.localScale = Vector3.one;
                button.interactable = true;
                return;
            }            

            Sequence sequence = DOTween.Sequence();
            sequence.Append(t.DOScale(1.1f, 0.15f))
                .Append(t.DOScale(1.0f, 0.1f))
                .OnComplete(() => { button.interactable = true; });
        }

        private void HideButton(Button button, bool useTransition = true)
        {
            Transform t = button.transform;
            if (!useTransition)
            {
                t.transform.localScale = Vector3.zero;
                button.interactable = true;
                button.gameObject.SetActive(false); 
                return;
            }

            Sequence sequence = DOTween.Sequence();
            sequence.Append(t.DOScale(0.0f, 0.15f))
                .OnComplete(() => { 
                    button.interactable = false;
                    button.gameObject.SetActive(false); 
                });
        }
    }
}
