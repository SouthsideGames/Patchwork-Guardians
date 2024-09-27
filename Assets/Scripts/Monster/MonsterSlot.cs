using UnityEngine;
using DG.Tweening;

namespace MonsterBattleArena.Monster
{
    public class MonsterSlot : MonoBehaviour
    {
        [SerializeField] private MonsterRig _rig;
        [SerializeField] private GameObject _fx;

        private string _monsterId;

        public string MonsterId { get => _monsterId; }
        public MonsterRig Rig { get => _rig; }

        public void SetMonster(string monsterId)
        {
            MonsterData monsterData = PlayerProfileManager.CurrentProfile.GetMonsterData(monsterId);
            SetMonster(monsterData);
        }

        public void SetMonster(MonsterData monsterData)
        {
            _monsterId = monsterData?.Id;
            bool setActive = !string.IsNullOrEmpty(_monsterId);
            
            _rig.gameObject.SetActive(setActive);
            _fx.gameObject.SetActive(setActive);

            if (setActive)
                _rig.UpdateMonsterPart(monsterData);
        }

        public void SetFX(bool enabled, bool useTransition = true)
        {
            if (enabled)
            {
                _fx.SetActive(true);
                Sequence sequence = DOTween.Sequence();
                sequence.Append(_fx.transform.DOScaleY(1.2f, useTransition ? 0.25f : 0.0f))
                    .Append(_fx.transform.DOScaleY(1.0f, useTransition ? 0.15f : 0.0f));
            }
            else
            {
                _fx.transform.DOScaleY(0.0f, useTransition ? 0.15f : 0.0f)
                    .OnComplete(() => _fx.SetActive(false));
            }
        }
    
    }
}
