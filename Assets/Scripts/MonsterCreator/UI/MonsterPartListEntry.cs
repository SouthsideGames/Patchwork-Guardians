using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace MonsterBattleArena.UI.MonsterCreator
{
    public class MonsterPartListEntry : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _entryText;

        public event System.Action<MonsterPartListEntry> OnClick;

        public void Set(Sprite icon, string text)
        {
            _icon.sprite = icon;
            _icon.preserveAspect = true;

            _entryText.text = string.Concat(text.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick?.Invoke(this);
        }
    }
}
