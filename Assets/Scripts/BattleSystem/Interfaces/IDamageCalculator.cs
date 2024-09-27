using MonsterBattleArena.Monster;

namespace MonsterBattleArena.BattleSystem
{
    public interface IDamageCalculator
    {
        MonsterDamageData CalculateDamageAmount(BattleUnit dealer, BattleUnit taker);
    }
}
