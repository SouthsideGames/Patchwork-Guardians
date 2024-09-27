using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class ConditionalAbility : AbilityBehaviour
    {
        [SerializeReference, SubclassSelector] private IAbilityCondition _condition;
        [SerializeReference, SubclassSelector] private AbilityBehaviour _behaviour;

        public override AbilityBehaviour Copy()
        {
            return new ConditionalAbility
            {
                _condition = this._condition,
                _behaviour = this._behaviour
            };
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _behaviour.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _behaviour.OnEnable();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (_condition.Evaluate(BattleUnit))
            {
                _behaviour.OnUpdate();
            }
        }
    }
}
