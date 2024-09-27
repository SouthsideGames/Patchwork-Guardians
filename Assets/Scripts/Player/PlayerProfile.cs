using System.Collections.Generic;
using MonsterBattleArena.Monster;
using Newtonsoft.Json;

namespace MonsterBattleArena.Player
{
    /*
    *   TODO: Use OnDeserialize callback and subtitute monsters list with Dictionary for more complex query
    */

    [System.Serializable]
    public class PlayerProfile
    {
        [JsonProperty("name")] public string Name { get; private set; }

        [JsonProperty("monsters")] private List<MonsterData> _monsters;
        [JsonProperty("party")] private string[] _monsterParty;
        [JsonProperty("inventory")] private PlayerInventory _inventory;

        [JsonIgnore] public int AvailableMonsters { get => _monsters.Count; }
        [JsonIgnore] public PlayerInventory Inventory { get => _inventory; }

        public MonsterData GetMonsterData(int index)
        {
            if (index < 0 || index >= _monsters.Count)
                return null;

            return _monsters[index];
        }

        public PlayerProfile(string name)
        {
            Name = name;
            _monsters = new List<MonsterData>();
            _monsterParty = new string[5];
            _inventory = new PlayerInventory();
        }

        public void AddMonsterData(MonsterData monster)
        {
            int index = _monsters.IndexOf(monster);
            if (index > -1)
            {
                _monsters[index] = monster;
                return;
            }

            _monsters.Add(monster);
        }

        public bool RemoveMonsterData(string id)
        {
            int index = _monsters.FindIndex(x => x.Id == id);
            if (index < 0)
                return false;

            _monsters.RemoveAt(index);
            return true;
        }

        public MonsterData GetMonsterData(string id)
        {
            return _monsters.Find(x => x.Id == id);
        }

        public bool SetPartySlot(MonsterData monsterData, int index)
        {
            string id = monsterData == null ? null : monsterData.Id;

            if (IsInParty(id))
            {
                return false;
            }

            _monsterParty[index] = id;
            return true;
        }

        public string GetPartySlot(int index)
        {
            return _monsterParty[index];
        }

        public bool IsInParty(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                foreach (string s in _monsterParty)
                {
                    if (!string.IsNullOrEmpty(s) && s == id)
                        return true;
                }
            }

            return false;
        }

        public bool IsInParty(MonsterData monsterData)
        {
            if (monsterData == null)
                return false;
                
            return IsInParty(monsterData.Id);
        }

        public int GetMonsterIndex(MonsterData monsterData)
        {
            return _monsters.IndexOf(monsterData);
        }
    
    }
}
