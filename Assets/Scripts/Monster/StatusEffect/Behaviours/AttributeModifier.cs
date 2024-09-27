using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class AttributeModifier : StatusEffectBehaviour
    {
        [SerializeField] private bool _applyEachTurn = false;
        [SerializeField] private AttributeType _attribute;
        [SerializeField] private float _modifier;

        private float _totalApplied;

        public override StatusEffectBehaviour Copy()
        {
            return new AttributeModifier
            {
                _applyEachTurn = this._applyEachTurn,
                _attribute = this._attribute,
                _modifier = this._modifier  
            };
        }

        public override void OnApply()
        {
            if (!_applyEachTurn)
                ApplyModifier();
        }

        public override void OnRemove()
        {
            BattleUnit.AddAttributeModifier(_attribute, _totalApplied);
        }

        public override void OnUpdate()
        {
            if (_applyEachTurn)
                ApplyModifier();
        }

        private void ApplyModifier()
        {
            BattleUnit.AddAttributeModifier(_attribute, _modifier);
            _totalApplied += _modifier;
        }
    }
}
