using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class Heal : StatusEffectBehaviour
    {
        [SerializeField] private int _healAmount;

        public override void OnApply()
        {
            
        }

        public override void OnRemove()
        {
            
        }

        public override void OnUpdate()
        {
            BattleUnit.Heal(_healAmount);
        }

        public override StatusEffectBehaviour Copy()
        {
            return new Heal
            {
                _healAmount = this._healAmount
            };
        }
    }
}
