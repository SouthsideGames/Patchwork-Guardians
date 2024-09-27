using TMPro;
using UnityEngine;

namespace MonsterBattleArena.PartyCreator.UI
{
    public class CarouselControlText : MonoBehaviour
    {
        [SerializeField] private TMP_Text _monsterName;
        [SerializeField] private TMP_Text _partyStatus;

        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _inPartyColor;

        public void UpdateText(string monsterName, bool inParty)
        {
            _monsterName.SetText(monsterName);

            _partyStatus.SetText(inParty ? "In Party" : "Not in party");
            _partyStatus.color = inParty ? _inPartyColor : _normalColor;
        }
    }
}
