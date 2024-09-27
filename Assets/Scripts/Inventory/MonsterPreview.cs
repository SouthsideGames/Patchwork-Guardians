using MonsterBattleArena.Monster;
using UnityEngine;

namespace MonsterBattleArena.Inventory
{
    public class MonsterPreview : MonoBehaviour
    {
        [SerializeField] private MonsterRig _rig;

        public void Set(MonsterData monsterData)
        {
            _rig.UpdateMonsterPart(monsterData);
        }

        public void Reset()
        {
            _rig.UpdateMonsterPart(null);
        }
    }
}
