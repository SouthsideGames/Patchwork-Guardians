using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MonsterBattleArena.Monster
{
    public enum MonsterPartType
    {
        Head, Body, LeftArm, RightArm, LeftLeg, RightLeg
    }

    /*
    *   TODO: Handle guid creation when duplicated
    *   NOTE: Monster part can only one type of attribute
    */

    [CreateAssetMenu(menuName = "Monster/Monster Part")]
    public class MonsterPart : GameResource
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private MonsterPartType _type;
        [SerializeField] private Synergy _synergy;
        [SerializeField] private MonsterAbility _ability;
        [Space]
        [SerializeField, NonReorderable] private AttributeSet[] _attributeSet;

        // [SerializeField, FormerlySerializedAs("_attributes")] private MonsterAttribute[] _oldAttr;

        /// <summary>
        /// Get the total number of tiers
        /// </summary>
        public int TotalLevel { get => _attributeSet.Length; }

        public Sprite Sprite { get => _sprite; }
        public MonsterPartType PartType { get => _type; }
        public Synergy Synergy { get => _synergy; }
        public MonsterAbility Ability { get => _ability; }

        public IEnumerable<MonsterAttribute> GetAttributes(int level)
        {
            foreach (MonsterAttribute attr in _attributeSet[level].Attributes)
            {
                yield return attr;
            }
        }

        public MonsterAttribute GetAttribute(AttributeType attributeType, int level)
        {
            foreach (MonsterAttribute attr in _attributeSet[level].Attributes)
            {
                if (attr.AttributeType == attributeType)
                    return attr;
            }

            return default;
        }
    
        [System.Serializable]
        private struct AttributeSet
        {
            public MonsterAttribute[] Attributes;
        }
    }
}
