using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterBattleArena.Monster
{
    public class AbilityController
    {
        private Dictionary<string, AbilityWrapper> _abilities;

        public AbilityController()
        {
            _abilities = new Dictionary<string, AbilityWrapper>();
        }

        public void Update()
        {
            foreach (AbilityWrapper wrapper in _abilities.Values)
            {
                wrapper.Update();
            }
        }
    
        public void RegisterAbility(BattleUnit unit, MonsterAbility ability)
        {
            AbilityWrapper wrapper = new AbilityWrapper(ability, unit);
            _abilities.Add(wrapper.Id, wrapper);
        }

        public void SetAbilityEnabled(string id, bool enabled)
        {
            if (_abilities.TryGetValue(id, out AbilityWrapper wrapper))
            {
                wrapper.SetEnabled(enabled);
                return;
            }

            Debug.Log("Unable to find ability with id: " + id);
        }

        public string GetRandomAbility()
        {
            int index = Random.Range(0, _abilities.Count);
            return _abilities.ElementAt(index).Key;
        }

        private class AbilityWrapper
        {
            private bool _enabled;
            private AbilityBehaviour[] _behaviours;

            public string Id { get; private set; }

            public bool Enabled { get => _enabled; }

            public AbilityWrapper(MonsterAbility ability, BattleUnit battleUnit)
            {
                Id = ability.Id;

                CopyBehaviours(ability.Behaviours);
                BehavioutHandle(x => x.Init(ability, battleUnit));
                SetEnabled(true);
            }

            public void SetEnabled(bool enabled)
            {
                _enabled = enabled;
                BehavioutHandle(x => x.Enabled = enabled);
            }

            public void Update()
            {
                if (!Enabled)
                    return;

                BehavioutHandle(x => x.OnUpdate());
            }

            private void CopyBehaviours(AbilityBehaviour[] behaviours)
            {
                _behaviours = new AbilityBehaviour[behaviours.Length];
                for (int i = 0; i < behaviours.Length; i++)
                {
                    _behaviours[i] = behaviours[i].Copy();
                }
            }

            private void BehavioutHandle(System.Action<AbilityBehaviour> a)
            {
                foreach (AbilityBehaviour ability in _behaviours)
                    a?.Invoke(ability);
            }
        }
    }
}
