using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MonsterBattleArena.Monster;

namespace MonsterBattleArena.PartyCreator.UI
{
    public class PartySlotUI : MonoBehaviour
    {
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _editButton;
        [SerializeField] private Button _removeButton;
        [SerializeField] private RectTransform _slotNumber;

        [SerializeField] private float _transitionDuration = 0.15f;

        private bool _isVisible;
        private MonsterSlot _partySlot;

        public bool IsVisible { get => _isVisible; }

        public void Initialize(MonsterSlot partySlot)
        {
            _partySlot = partySlot;

            Vector2 offset = new Vector2(Screen.width, Screen.height) * 0.5f;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(partySlot.transform.position);
            ((RectTransform)transform).anchoredPosition = screenPos - offset;

            Hide(false);
            Show();
        }

        public void SetEditButtonListener(System.Action onClick)
        {
            _editButton.onClick.RemoveAllListeners();
            _editButton.onClick.AddListener(() => onClick?.Invoke());
        }

        public void SetAddButtonListener(System.Action onClick)
        {
            _addButton.onClick.RemoveAllListeners();
            _addButton.onClick.AddListener(() => onClick?.Invoke());
        }

        public void SetRemoveButtonListener(System.Action onClick)
        {
            _removeButton.onClick.RemoveAllListeners();
            _removeButton.onClick.AddListener(() => onClick?.Invoke());
        }

        public void Show(bool useTransition = true)
        {
            ShowButton(useTransition);

            _isVisible = true;
            if (!useTransition)
            {
                _slotNumber.localScale = Vector3.one;
                return;
            }

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_slotNumber.DOScaleX(1.0f, _transitionDuration))
                .Join(_slotNumber.DOScaleY(1.1f, _transitionDuration))
                .Append(_slotNumber.DOScaleY(1.0f, 0.1f));
        }

        public void Hide(bool useTransition = true)
        {
            HideButtons(useTransition);

            if (!useTransition)
            {
                _slotNumber.localScale = Vector3.zero;
                _isVisible = false;
                return;
            }

            _slotNumber.DOScale(0.0f, _transitionDuration)
                .OnComplete(() => { _isVisible = false; });
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
            sequence.Append(t.DOScale(1.1f, _transitionDuration))
                .Append(t.DOScale(1.0f, 0.1f))
                .OnComplete(() => { button.interactable = true; });

            return;
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
            sequence.Append(t.DOScale(0.0f, _transitionDuration))
                .OnComplete(() => { 
                    button.interactable = false;
                    button.gameObject.SetActive(false); 
                });
        }

        private void ShowButton(bool useTransition = true)
        {
            if (_partySlot == null || string.IsNullOrEmpty(_partySlot.MonsterId))
            {
                ShowButton(_addButton, useTransition);
            }
            else
            {
                ShowButton(_editButton, useTransition);
                ShowButton(_removeButton, useTransition);
            }
        }

        private void HideButtons(bool useTransition = true)
        {
            HideButton(_addButton, useTransition);
            HideButton(_editButton, useTransition);
            HideButton(_removeButton, useTransition);
        }
    }
}
