using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class DamageOverTime : StatusEffectBehaviour
    {
        [SerializeField] private int _minDamage;
        [SerializeField] private int _maxDamage;

        public override StatusEffectBehaviour Copy()
        {
            return new DamageOverTime
            {
                _minDamage = this._minDamage,
                _maxDamage = this._maxDamage
            };
        }

        public override void OnApply()
        {
            
        }

        public override void OnRemove()
        {
            
        }

        public override void OnUpdate()
        {
            int damage = Random.Range(_minDamage, _maxDamage + 1);
            BattleUnit.TakeDamage(new MonsterDamageData { Dealer = BattleUnit, Value = damage });
        }
    }
}
