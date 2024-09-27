using MonsterBattleArena.Monster;
using UnityEngine;

namespace MonsterBattleArena.BattleSystem
{
    public class DefaultDamageCalculator : IDamageCalculator
    {
        private int _minDamageThreshold;
        private int _maxCritThreshold;

        public DefaultDamageCalculator(int minDamageThreshold, int maxCritThreshold)
        {
            _minDamageThreshold = minDamageThreshold;
            _maxCritThreshold = maxCritThreshold;
        }

        public MonsterDamageData CalculateDamageAmount(BattleUnit dealer, BattleUnit taker)
        {
            MonsterDamageData damage = new MonsterDamageData()
            {
                Dealer = dealer
            };

            float hitChance = CalculateHitChance(dealer, taker);
            if (Random.Range(0.0f, 1.0f) < hitChance)
            {
                int atk = dealer.GetAttributeValue(AttributeType.Attack);
                int crit = dealer.GetAttributeValue(AttributeType.CriticalChance);
                int def = taker.GetAttributeValue(AttributeType.Defense);

                crit += dealer.GetBonusCritChance();
                crit = Mathf.Clamp(crit, 0, _maxCritThreshold);

                damage.IsCrit = Random.Range(0, 100) < crit;
                damage.Value = Mathf.Clamp((atk - def) * (damage.IsCrit ? 2 : 1), _minDamageThreshold, int.MaxValue);
            }

            return damage;
        }

        // Calculate the hit chance. Range from 0 - 1
        private float CalculateHitChance(BattleUnit dealer, BattleUnit taker)
        {
            int acc = dealer.GetAttributeValue(AttributeType.Accuracy);
            int dodge = taker.GetAttributeValue(AttributeType.Dodge);

            if (dodge <= 0)
                return 1.0f;

            return Mathf.Clamp01((float)acc / dodge * 0.5f);
        }
    }
}
