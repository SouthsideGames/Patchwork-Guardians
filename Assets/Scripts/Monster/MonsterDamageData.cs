namespace MonsterBattleArena.Monster
{
    public struct MonsterDamageData
    {
        public BattleUnit Dealer;
        public int Value;
        public bool IsCrit;

        public bool IsMiss { get => Value <= 0; }
    }
}
