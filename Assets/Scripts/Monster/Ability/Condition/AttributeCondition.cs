using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class AttributeCondition : IAbilityCondition
    {
        public enum Comparator
        {
            LessOrEqual, GreaterOrEqual
        }

        [SerializeField] private AttributeType _attribute;
        [SerializeField] private Comparator _comparator;
        [SerializeField, Range(0, 1)] private float _value;

        public bool Evaluate(BattleUnit unit)
        {
            int attribute = unit.GetAttributeValue(_attribute);

            switch (_comparator)
            {
                case Comparator.LessOrEqual:
                    return attribute <= (attribute * _value);
                case Comparator.GreaterOrEqual:
                    return attribute >= (attribute * _value);
            }

            return false;
        }
    }
}
