using MonsterBattleArena.UI;
using MonsterBattleArena.Monster;
using TMPro;
using UnityEngine;

namespace MonsterBattleArena.BattleSystem.UI
{
    public class BattleUnitStatus : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _slotNumber;
        [SerializeField] private ProgressBar _healthBar;
        [SerializeField] private ProgressBar _manaBar;

        public void Initialize(BattleUnit battleUnit)
        {
            _name.SetText(battleUnit.Name);
            _slotNumber.SetText(battleUnit.UnitIndex.ToString());
            _healthBar.SetValue(1);
            _manaBar.SetValue(1);

            battleUnit.OnCurrentHealthChanged += () => {
                _healthBar.SetValue(battleUnit.CurrentHealthPercent);
            };
        }
    }
}
