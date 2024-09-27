using MonsterBattleArena.BattleSystem;
using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class FriendsGuard : AbilityBehaviour
    {
        [SerializeField] private float _defModifier;

        public override AbilityBehaviour Copy()
        {
            return new FriendsGuard
            {
                _defModifier = this._defModifier
            };
        }

        public override void OnEnable()
        {
            base.OnEnable();

            foreach (BattleUnit battleUnit in BattleManager.Current.PlayerUnits)
            {
                if (battleUnit == BattleUnit)
                    continue;

                battleUnit.AddAttributeModifier(AttributeType.Defense, _defModifier);
            }
        }
    }
}
