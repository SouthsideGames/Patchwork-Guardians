using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class PassiveHealing : AbilityBehaviour
    {
        [SerializeField] private int _healAmount; 

        public override AbilityBehaviour Copy()
        {
            return new PassiveHealing
            {
                _healAmount = this._healAmount
            };
        }

        public override void OnUpdate()
        {
            BattleUnit.Heal(_healAmount);
        }
    }
}
