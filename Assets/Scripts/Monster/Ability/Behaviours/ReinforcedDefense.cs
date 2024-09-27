using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class ReinforcedDefense : AbilityBehaviour
    {
        [SerializeField, Range(0, 1)] private float _damageReduction;

        public override AbilityBehaviour Copy()
        {
            return new ReinforcedDefense
            {
                _damageReduction = this._damageReduction
            };
        }

        public override void OnEnable()
        {
            base.OnEnable();
            BattleUnit.OnBeforeTakeDamage += ReduceDamge;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            BattleUnit.OnBeforeTakeDamage -= ReduceDamge;
        }

        private int ReduceDamge(int damage)
        {
            int current = Mathf.RoundToInt(damage - (damage * _damageReduction));
            return Mathf.Clamp(current, 0, int.MaxValue);
        }
    }
}
