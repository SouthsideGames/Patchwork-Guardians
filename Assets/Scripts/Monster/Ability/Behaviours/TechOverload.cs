using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class TechOverload : AbilityBehaviour
    {
        [SerializeField, Range(0, 100)] private int _abilityDisableChance = 20;
        [SerializeField] private int _duration = 2;

        private List<Wrapper> _wrappers;
        private Queue<Wrapper> _toRemove;

        public override AbilityBehaviour Copy()
        {
            return new TechOverload
            {
                _abilityDisableChance = this._abilityDisableChance,
                _duration = this._duration
            };
        }

        protected override void OnInit()
        {
            base.OnInit();
            _wrappers = new List<Wrapper>();
            _toRemove = new Queue<Wrapper>();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            BattleUnit.OnAttackDealDamage += DisableTargetAbility;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            BattleUnit.OnAttackDealDamage -= DisableTargetAbility;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            foreach (Wrapper wrapper in _wrappers)
            {
                if (wrapper.RoundPassed > _duration)
                {
                    _toRemove.Enqueue(wrapper);
                }

                wrapper.RoundPassed++;
            }
            
            while (_toRemove.Count > 0)
            {
                Wrapper wrapper = _toRemove.Dequeue();
                wrapper.Target.Ability.SetAbilityEnabled(wrapper.Ability, true);

                _wrappers.Remove(wrapper);
            }
        }

        private void DisableTargetAbility(BattleUnit target, MonsterDamageData dmg)
        {
            int perc = Random.Range(0, 100);
            if (perc <= _abilityDisableChance)
            {
                Wrapper wrapper = new Wrapper
                {
                    Target = target,
                    Ability = target.Ability.GetRandomAbility(),
                    RoundPassed = 0
                };

                if (string.IsNullOrEmpty(wrapper.Ability))
                    return;
                
                wrapper.Target.Ability.SetAbilityEnabled(wrapper.Ability, false);
                _wrappers.Add(wrapper);
            }
        }

        private class Wrapper
        {
            public BattleUnit Target;
            public string Ability;
            public int RoundPassed;
        }
    }
}
