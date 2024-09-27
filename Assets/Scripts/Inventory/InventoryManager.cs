using System.Collections.Generic;
using MonsterBattleArena.Inventory.ItemFuse;
using UnityEngine;

namespace MonsterBattleArena.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private ItemFuseManager _itemFuse;
        [SerializeField] private InventoryUIController _ui;

        private List<InventoryItem> _items;

        public IEnumerable<InventoryItem> Items { get => _items; }

        private void Start()
        {
            _ui.Initialize(this);
            _itemFuse.Initialize(this);
            
            LoadInventoryItems();
        }

        private void LoadInventoryItems()
        {
            _items = new List<InventoryItem>();

            foreach (Player.PlayerInventoryItem item in PlayerProfileManager.CurrentProfile.Inventory.GetItems())
            {
                GameResource resource = ResourceDatabase.Load<GameResource>(item.Guid);
                _items.Add(new InventoryItem
                {
                    Resource = resource,
                    Level = item.Level,
                    Count = item.Count
                });
            }

            _ui.RefreshInventorySlots();
        }

        public void Refresh()
        {
            _items.Clear();
            LoadInventoryItems();   
        }

        public InventoryItem GetItem(int index)
        {
            return _items[index];
        }
    }
}
