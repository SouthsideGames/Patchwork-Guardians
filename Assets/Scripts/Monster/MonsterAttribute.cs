using Newtonsoft.Json;
using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public struct MonsterAttribute
    {
        [JsonProperty("type")] [SerializeField] private AttributeType _type;
        [JsonProperty("value")] [SerializeField] private int _value;

        public AttributeType AttributeType { get => _type; }
        public int Value { get => _value; }

        public MonsterAttribute(AttributeType type)
        {
            _type = type;
            _value = 0;
        }
    }
}
