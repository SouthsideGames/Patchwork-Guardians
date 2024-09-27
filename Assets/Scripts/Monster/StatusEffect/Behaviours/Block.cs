using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class Block : StatusEffectBehaviour
    {
        public override StatusEffectBehaviour Copy()
        {
            return new Block();
        }

        public override void OnApply()
        {
            BattleUnit.OnBeforeHeal += BlockHealingEffect;
        }

        public override void OnRemove()
        {
            BattleUnit.OnBeforeHeal -= BlockHealingEffect;
        }

        private int BlockHealingEffect(int heal)
        {
            Debug.Log("Healing effect blocked");
            return 0;
        }
    }
}
