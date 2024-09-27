using MonsterBattleArena.BattleSystem;
using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class Overcharged : StatusEffectBehaviour
    {
        [SerializeField] private int _turnSkipped = 3;
        [SerializeField] private float _damageMultiplier = 0.5f;

        private int _turnPassed;

        public override StatusEffectBehaviour Copy()
        {
            return new Overcharged
            {
                _turnSkipped = this._turnSkipped,
                _damageMultiplier = this._damageMultiplier
            };
        }

        public override void OnApply()
        {
            BattleUnit.Immobilized = true;
        }

        public override void OnRemove()
        {
            
        }

        public override void OnUpdate()
        {
            // TODO: Overcharged popup

            if (!BattleUnit.Immobilized)
            {
                /*
                *   At the end of the round, if the battle unit is not immobilized
                *   then the unit was performing an attack, so set the immobilized back to true
                */
                BattleUnit.Immobilized = true;
            }
            else
            {
                PopupText.PopupConfig config = new PopupText.PopupConfig
                {
                    TextType = PopupText.TextType.Default,
                    MaxAngle = 15
                };
                
                PopupText.Show("Overcharged!", BattleUnit.CalculatePopupPos(), config);
            }

            if (_turnPassed == _turnSkipped)
            {
                // Disable immobilized. so in the next round the battle unit will be able to attack
                BattleUnit.Immobilized = false;
                _turnPassed = 0;
            }

            _turnPassed++;
        }
    }
}
