using UnityEngine;

namespace MonsterBattleArena
{
    [CreateAssetMenu(menuName = "Fuse Cost Table")]
    public class FuseCostTable : ScriptableObject
    {
        [SerializeField] private FuseCost[] _fuseCosts;

        public FuseCost GetResourceCostForLevel(int level)
        {
            if (level < 0 || level >= _fuseCosts.Length)
            {
                Debug.LogWarning("Invalid tier");
                return default;
            }
            
            return _fuseCosts[level];
        }
    }
}
