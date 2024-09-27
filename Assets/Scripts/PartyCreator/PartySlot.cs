using MonsterBattleArena.Monster;
using UnityEngine;

namespace MonsterBattleArena.PartyCreator
{
    public class PartySlot : MonoBehaviour
    {
        [SerializeField] private MonsterRig _rig;
        [SerializeField] private GameObject _fx;

        private string _monsterId;

        public string MonsterId { get => _monsterId; }

        public void Initialize(string monsterId)
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
    }
}
