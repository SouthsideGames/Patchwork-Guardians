using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class PassiveAttributeModifier : AbilityBehaviour
    {
        [SerializeReference, SubclassSelector] private IAbilityCondition _condition;

        [SerializeField] private bool _activateOnEnable;
        [SerializeField] private AttributeType _attributeType;
        [SerializeField] private float _modifier;

        private bool _abilityEnabledManually;

        public override AbilityBehaviour Copy()
        {
            return new PassiveAttributeModifier
            {
                _condition = this._condition,
                _activateOnEnable = this._activateOnEnable,
                _attributeType = this._attributeType,
                _modifier = this._modifier
            };
        }

        protected override void OnInit()
        {
            base.OnInit();

            if (!_activateOnEnable && _condition == null)
            {
                Debug.LogWarningFormat("The activateOnEnable flag on {0} is disabled but the condition is null!", Ability.Name);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (_activateOnEnable)
                TriggerAbility();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            if (_activateOnEnable)
                DisableAbility();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!_abilityEnabledManually && _condition?.Evaluate(BattleUnit) == true)
            {
                _abilityEnabledManually = true;
                TriggerAbility();
            }
        }

        public void TriggerAbility()
        {
            BattleUnit.AddAttributeModifier(_attributeType, _modifier);
        }

        public void DisableAbility()
        {
            BattleUnit.AddAttributeModifier(_attributeType, -_modifier);
        }
    }
}
