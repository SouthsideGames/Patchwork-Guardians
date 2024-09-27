using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class Overcharge : StatusEffectBehaviour
    {
        [SerializeField] private float _damageMultiplier = 0.5f;
        [SerializeField] private int _selfDamageAmount = 10;
        [SerializeField] private bool _incrementalDamage = false;
        [SerializeField] private float _incrementalPercent = 0.1f;

        private int _turnPassed;

        public override StatusEffectBehaviour Copy()
        {
            return new Overcharge
            {
                _damageMultiplier = this._damageMultiplier,
                _selfDamageAmount = this._selfDamageAmount,
                _incrementalDamage = this._incrementalDamage,
                _incrementalPercent = this._incrementalPercent
            };
        }

        public override void OnApply()
        {
            BattleUnit.AddAttributeModifier(AttributeType.Attack, _damageMultiplier);
        }

        public override void OnRemove()
        {
            BattleUnit.AddAttributeModifier(AttributeType.Attack, -_damageMultiplier);
        }

        public override void OnUpdate()
        {
            int damage = _selfDamageAmount;
            if (_incrementalDamage)
            {
                damage += Mathf.RoundToInt(_selfDamageAmount * _incrementalPercent * _turnPassed);
            }

            BattleUnit.TakeDamage(new MonsterDamageData { Value = damage, Dealer = BattleUnit });

            _turnPassed++;
        }
    }
}
