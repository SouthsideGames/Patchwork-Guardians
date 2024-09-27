using MonsterBattleArena.Monster;
using MonsterBattleArena.MonsterCreator;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterBattleArena.UI.MonsterCreator
{
    public class MonsterPartSlot : MonoBehaviour, IPointerClickHandler
    {
        private static MonsterPartSlot Selected { get; set; }

        [SerializeField] private MonsterPartType _slotType;
        [SerializeField] private Image _icon;

        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _selectedColor;

        private Image _border;

        private void Start()
        {
            _border = GetComponent<Image>();

            MonsterCreatorManager.Current.OnMonsterPartUpdated += OnMonsterPartUpdated;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Selected != null)
            {
                Selected._border.color = _normalColor;
                
                if (Selected == this)
                {
                    // Deselect and clear the list when clicking on the same slot again
                    MonsterPartList.Current.ClearListEntries();
                    Selected = null;
                    return;
                }
            }
            
            // Populate part lists
            MonsterPartList.Current.LoadListEntries(_slotType);

            // Set this slot as selected
            _border.color = _selectedColor;
            Selected = this;
            
        }

        private void OnMonsterPartUpdated(MonsterPart part)
        {
            if (part == null)
            {
                _icon.gameObject.SetActive(false);
                return;
            }

            if (part.PartType != _slotType)
                return;

            _icon.gameObject.SetActive(true);
            _icon.sprite = part.Sprite;
            _icon.preserveAspect = true;
        }
    }
}
