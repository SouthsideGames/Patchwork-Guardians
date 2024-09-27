using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [CreateAssetMenu(menuName = "Monster/Status Effect")]
    public class StatusEffect : GameResource
    {
        // How long does the status effect should last
        [SerializeField] private int _duration;

        [SerializeReference, SubclassSelector] private StatusEffectBehaviour[] _behaviours = System.Array.Empty<StatusEffectBehaviour>();

        public int Duration { get => _duration; }
        public StatusEffectBehaviour[] Behaviour { get => _behaviours; }
    }
}
