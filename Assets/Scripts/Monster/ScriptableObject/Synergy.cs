using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [CreateAssetMenu(menuName = "Monster/Synergy")]
    public class Synergy : GameResource
    {
        [SerializeField] private SynergyWrapper[] _synergies;

        public SynergyWrapper[] Synergies { get => _synergies; }
        public int SynergyCount { get => _synergies.Length; }

        public MonsterAbility GetAbility(int index)
        {
            return _synergies[index].Ability;
        }
        
        [System.Serializable]
        public class SynergyWrapper
        {
            public string Name;
            public MonsterPartType[] RequiredParts;
            public MonsterAbility Ability;
        }
    }
}
