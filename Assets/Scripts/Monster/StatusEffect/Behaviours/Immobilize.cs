namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class Immobilize : StatusEffectBehaviour
    {
        public override StatusEffectBehaviour Copy()
        {
            return new Immobilize
            {

            };   
        }

        public override void OnApply()
        {
            BattleUnit.Immobilized = true;
        }

        public override void OnRemove()
        {
            BattleUnit.Immobilized = false;
        }

        public override void OnUpdate()
        {
            
        }
    }
}
