using System.Collections.Generic;
using Newtonsoft.Json;

namespace MonsterBattleArena.Player
{
    [System.Serializable]
    public class PlayerInventory
    {
        /*
        *   NOTE:
        *       Item's guid
        *       {0000}_{00000000}_{00}
        */

        [JsonProperty("items")] private Dictionary<string, int> _items;

        public IEnumerable<PlayerInventoryItem> GetItems()
        {
            foreach (KeyValuePair<string, int> item in _items)
            {
                yield return new PlayerInventoryItem
                {
                    Guid = ResourceGUIDUtility.GetIDFromGUID(item.Key),
                    Level = ResourceGUIDUtility.GetLevelFromGUID(item.Key),
                    Count = item.Value
                };
            }
        }

        public IEnumerable<PlayerInventoryItem> GetItems<T>() where T : GameResource
        {
            int hash = GameResourceUtility.GetResourceTypeHash<T>();

            foreach (KeyValuePair<string, int> item in _items)
            {
                int type = ResourceGUIDUtility.GetTypeHashFromGUID(item.Key);
                if (type == hash)
                {
                    yield return new PlayerInventoryItem
                    {
                        Guid = ResourceGUIDUtility.GetIDFromGUID(item.Key),
                        Level = ResourceGUIDUtility.GetLevelFromGUID(item.Key),
                        Count = item.Value
                    };
                }
            }
        }

        public PlayerInventory()
        {
            _items = new Dictionary<string, int>();
        }

        /// <summary>
        /// Add an item to inventory
        /// </summary>
        /// <param name="item">item id</param>
        /// <param name="level">item level</param>
        public void AddItem(GameResource item, int level)
        {
            AddItemRange(item, level, 1);
        }
        
        /// <summary>
        /// Add an item with specific amount
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <param name="count"></param>
        public void AddItemRange(GameResource item, int level, int count)
        {
            if (count < 1)
                return;

            string key = ResourceGUIDUtility.AppendLevelToGUID(item.Id, level);
            if (HasItem(key))
                _items[key] += count;
            else
                _items.Add(key, count);
        }

        /// <summary>
        /// Remove an item
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        public void RemoveItem(string id, int count = 1)
        {
            if (ResourceGUIDUtility.GetLevelFromGUID(id) <= 0)
            {
                id = ResourceGUIDUtility.AppendLevelToGUID(id, 0);
            }

            if (!HasItem(id) || count < 1)
            {
                return;
            }

            _items[id] -= count;
            if (_items[id] <= 0)
                _items.Remove(id);
        }
    
        /// <summary>
        /// Check if player has item with id in inventory
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasItem(string id)
        {
            return GetItemCount(id) > 0;
        }
    
        /// <summary>
        /// Get how many items player have
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetItemCount(string id)
        {
            if (string.IsNullOrEmpty(id))
                return 0;

            if (ResourceGUIDUtility.GetLevelFromGUID(id) <= 0)
            {
                id = ResourceGUIDUtility.AppendLevelToGUID(id, 0);
            }
                
            if (_items.ContainsKey(id))
            {
                return _items[id];
            }

            return 0;
        }
    }
}
