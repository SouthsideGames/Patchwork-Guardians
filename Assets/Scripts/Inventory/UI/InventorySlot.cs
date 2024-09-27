using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MonsterBattleArena.Inventory.UI
{
    public class InventorySlot : MonoBehaviour, IPointerClickHandler
    {
        public enum SelectMode
        {
            Primary,
            Secondary
        }

        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private RectTransform _tierIndicator;

        private Outline _outline;
        private System.Action _onClick;

        public void Set(Sprite icon, int tier, int count, System.Action onClick = null)
        {
            _icon.sprite = icon;
            _count.SetText(count.ToString());
            _onClick = onClick;

            SetTier(tier);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _onClick?.Invoke();
        }

        public void Select(SelectMode selectMode)
        {
            if (_outline == null)
                _outline = GetComponent<Outline>();

            switch (selectMode)
            {
                case SelectMode.Primary:
                    _outline.effectColor = UISettings.SlotSelectedPrimary;
                    break;
                case SelectMode.Secondary:
                    _outline.effectColor = UISettings.SlotSelectedSecondary;
                    break;
            }
        }

        public void Deselect()
        {
            if (_outline == null)
                _outline = GetComponent<Outline>();

            _outline.effectColor = UISettings.SlotUnselected;
        }
    
        public void SetTier(int tier)
        {
            _tierIndicator.gameObject.SetActive(false);

            Transform parent = _tierIndicator.parent;
            for (int x = parent.childCount - 1; x >= 1; x--)
            {
                Destroy(parent.GetChild(x).gameObject);
            }

            for (int i = 0; i < tier; i++)
            {
                Instantiate(_tierIndicator, _tierIndicator.parent).gameObject.SetActive(true);
            }
        }
    
        public void ToggleCountText(bool active)
        {
            _count.gameObject.SetActive(active);
        }
    }
}
