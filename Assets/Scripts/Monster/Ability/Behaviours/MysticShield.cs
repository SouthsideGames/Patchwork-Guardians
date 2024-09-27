using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class MysticShield : AbilityBehaviour
    {
        [SerializeField, Range(0, 1)] private float _damageReflected = 0.2f;

        private int _damageDealt;

        public override AbilityBehaviour Copy()
        {
            return new MysticShield
            {
                _damageReflected = this._damageReflected
            };
        }

        public override void OnEnable()
        {
            base.OnEnable();
            BattleUnit.OnBeforeTakeDamage += GetDamage;
            BattleUnit.OnTakeDamage += ReflectDamage;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            BattleUnit.OnBeforeTakeDamage -= GetDamage;
            BattleUnit.OnTakeDamage -= ReflectDamage;
        }

        private int GetDamage(int dmg)
        {
            _damageDealt = dmg;
            return dmg;
        }

        private void ReflectDamage(BattleUnit target)
        {
            if (target == BattleUnit)
                return;

            int dmg = Mathf.RoundToInt(_damageDealt * _damageReflected);
            target.TakeDamage(new MonsterDamageData { Dealer = BattleUnit, Value = dmg });
        }
    }
}
