using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleArena.Monster
{
    public class StatusEffectController
    {
        /*
        *   NOTE:
        *       Currently battle units can only have one status effect applied
        *       with Apply(StatusEffect) but we can apply multiple status effect 
        *       behaviours manually with Apply(string, int, StatusEffectBehaviour[]) 
        */

        private BattleUnit _battleUnit;
        private List<StatusEffectWrapper> _wrappers;
        private Stack<int> _toRemove;

        public StatusEffectController(BattleUnit battleUnit)
        {
            _battleUnit = battleUnit;
            _wrappers = new List<StatusEffectWrapper>();
            _toRemove = new Stack<int>();
        }

        public void Apply(StatusEffect statusEffect)
        {
            if (statusEffect == null)
            {
                Debug.LogError("Status effect can't be null!");
                return;
            }

            if (_wrappers.Count > 0)
            {
                Debug.Log("Battle unit already has 1 active status effect!");
                return;
            }

            Apply(statusEffect.Id, statusEffect.Duration, statusEffect.Behaviour);
        }

        public void Apply(string id, int duration, StatusEffectBehaviour[] behaviours)
        {
            _wrappers.Add(new StatusEffectWrapper(id, duration, _battleUnit, behaviours));
        }

        public void Update()
        {
            for (int i = 0; i < _wrappers.Count; i++)
            {
                _wrappers[i].Update();
                if (_wrappers[i].DurationEnded())
                {
                    _toRemove.Push(i);
                }
            }

            while (_toRemove.Count > 0)
            {
                int index = _toRemove.Pop();
                _wrappers[index].Remove();
                _wrappers.RemoveAt(index);
            }
        }

        private class StatusEffectWrapper
        {
            public string Id { get; private set; }
            private int _duration;
            private int _turnPassed;
            private StatusEffectBehaviour[] _behaviours;

            public StatusEffectWrapper(string id, int duration, BattleUnit unit, StatusEffectBehaviour[] behaviours)
            {
                Id = id;
                _duration = duration;
                
                CopyBehaviours(behaviours);
                BehaviourHandler(x => x.Init(unit));
                BehaviourHandler(x => x.OnApply());
            }

            public void Update()
            {
                BehaviourHandler(x => x.OnUpdate());
                _turnPassed++;
            }

            public void Remove()
            {
                BehaviourHandler(x => x.OnRemove());
            }

            public bool DurationEnded()
            {
                // -1 means the status effect lasts indefinitely
                if (_duration == -1)
                    return false;

                return _turnPassed >= _duration;
            }

            private void CopyBehaviours(StatusEffectBehaviour[] behaviours)
            {
                _behaviours = new StatusEffectBehaviour[behaviours.Length];
                for (int i = 0; i < _behaviours.Length; i++)
                {
                    _behaviours[i] = behaviours[i].Copy();
                }
            }

            private void BehaviourHandler(System.Action<StatusEffectBehaviour> action)
            {
                foreach (StatusEffectBehaviour behaviour in _behaviours)
                {
                    action.Invoke(behaviour);
                }
            }
        }
    }
}
