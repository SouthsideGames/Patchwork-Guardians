using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class StatusEffectAbility : AbilityBehaviour
    {
        [SerializeField] private int _minDuration = 2;
        [SerializeField] private int _maxDuration = 2;
        [SerializeReference, SubclassSelector] private StatusEffectBehaviour[] _statusEffects;

        public override AbilityBehaviour Copy()
        {
            return new StatusEffectAbility
            {
                _minDuration = this._minDuration,
                _maxDuration = this._maxDuration,
                _statusEffects = this._statusEffects,
            };
        }

        public override void OnEnable()
        {
            base.OnEnable();
            BattleUnit.OnAttackDealDamage += OnAttack;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            BattleUnit.OnAttackDealDamage -= OnAttack;
        }

        private void OnAttack(BattleUnit target, MonsterDamageData damageData)
        {
            int duration = Random.Range(_minDuration, _maxDuration);
            target.ApplyStatusEffectBehaviour(GetHashCode().ToString(), duration, _statusEffects);
        }
    }
}
