using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [CreateAssetMenu(menuName = "Monster/Monster Ability")]
    public class MonsterAbility : GameResource
    {
        [Space]
        [SerializeReference, SubclassSelector] private AbilityBehaviour[] _behaviours;

        public AbilityBehaviour[] Behaviours { get => _behaviours; }
    }
}
