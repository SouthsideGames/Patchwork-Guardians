using MonsterBattleArena.Monster;
using MonsterBattleArena.Inventory.ItemFuse.UI;
using UnityEngine;

namespace MonsterBattleArena.Inventory.ItemFuse
{
    public class ItemFuseManager : InstancedMonobehaviour<ItemFuseManager>
    {
        /*
        *   NOTE:
        *       Currently, it's only possible to fuse monster part
        */

        [SerializeField] private FusePanel _fusePanel;

        private FuseCostTable _fuseCostTable;
        private InventoryManager _inventory;

        private string _currentItem;
        private FuseCost _currentFuseCost;

        public void Initialize(InventoryManager inventory)
        {
            _inventory = inventory;
            _fuseCostTable = Resources.Load<FuseCostTable>("FuseCostTable");

            _fusePanel.Initialize(this, FuseCurrentItem);
        }

        public void FuseItem(string guid)
        {
            string id = ResourceGUIDUtility.GetIDFromGUID(guid);
            int level = ResourceGUIDUtility.GetLevelFromGUID(guid);
            MonsterPart part = ResourceDatabase.Load<MonsterPart>(id);
            ItemFuseArgs args = SolveItemFuse(part, level, guid);

            _currentItem = guid;
            _currentFuseCost = args.FuseCost;

            _fusePanel.Set(part, level, args);
            _fusePanel.Show(null);
        }

        private void FuseCurrentItem()
        {
            string resId = ResourceGUIDUtility.GetIDFromGUID(_currentItem);
            int level = ResourceGUIDUtility.GetLevelFromGUID(_currentItem);
            GameResource res = ResourceDatabase.Load<GameResource>(resId);
            PlayerProfileManager.CurrentProfile.Inventory.RemoveItem(_currentItem, 2);
            PlayerProfileManager.CurrentProfile.Inventory.RemoveItem(_currentFuseCost.Resource);

            PlayerProfileManager.CurrentProfile.Inventory.AddItem(res, level + 1);
            PlayerProfileManager.SaveCurrentProfile();
            _inventory.Refresh();
        }

        private ItemFuseArgs SolveItemFuse(MonsterPart part, int level, string guid)
        {
            FuseCost cost = _fuseCostTable.GetResourceCostForLevel(level);
            ItemFuseArgs args = new ItemFuseArgs();
            args.FuseCost = cost;

            // Already at max level
            args.LevelMaxed = level >= part.TotalLevel;
            
            // Check for spare item
            args.HasSpareItem = PlayerProfileManager.CurrentProfile.Inventory.GetItemCount(guid) > 1;

            // TODO: Check if have enough item for the cost see FuseCost.Count
            args.SufficientCost = PlayerProfileManager.CurrentProfile.Inventory.HasItem(cost.Resource);

            return args;
        }

        public struct ItemFuseArgs
        {
            public FuseCost FuseCost;
            public bool LevelMaxed;
            public bool SufficientCost;
            public bool HasSpareItem;
        }
    }
}
