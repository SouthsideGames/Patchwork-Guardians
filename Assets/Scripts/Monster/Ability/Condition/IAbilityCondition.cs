using UnityEngine;

namespace MonsterBattleArena.Monster
{
    public interface IAbilityCondition
    {
        bool Evaluate(BattleUnit unit);
    }
}
