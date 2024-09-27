using MonsterBattleArena.Util;
using MonsterBattleArena.Player;
using UnityEngine;
using MonsterBattleArena.Monster;

namespace MonsterBattleArena
{
    public static class PlayerProfileManager
    {
        private const string DefaultProfilePath = "Profiles";
        private const string DefaultProfile = "default";

        private static PlayerProfile _CurrentProfile;
        public static PlayerProfile CurrentProfile 
        { 
            get
            {
                if (_CurrentProfile == null)
                    LoadDefaultProfile();
                
                return _CurrentProfile;
            } 
        }

        public static void SaveCurrentProfile()
        {
            if (CurrentProfile == null)
            {
                return;
            }

            SerializationUtility.Serialize(_CurrentProfile, _CurrentProfile.Name, DefaultProfilePath);
        }

        public static void CreateNewProfile(string name)
        {
            _CurrentProfile = new PlayerProfile(name);
            
            // Populate inventory with default items
            foreach (MonsterPartType type in System.Enum.GetValues(typeof(MonsterPartType)))
            {
                foreach (MonsterPart part in ResourceDatabase.LoadAll<MonsterPart>(type))
                {
                    _CurrentProfile.Inventory.AddItemRange(part, 0, 5);
                }
            }

            foreach (Item item in ResourceDatabase.LoadAll<Item>())
            {
                _CurrentProfile.Inventory.AddItemRange(item, 0, 100);
            }

            SaveCurrentProfile();
        }

        public static void LoadDefaultProfile()
        {
            if (!LoadProfile(DefaultProfile))
            {
                Debug.Log("Creating default profile.");
                CreateNewProfile(DefaultProfile);
            }
        }

        public static bool LoadProfile(string profile)
        {
            if (SerializationUtility.IsFileExists(profile, DefaultProfilePath))
            {
                _CurrentProfile = SerializationUtility.Deserialize<PlayerProfile>(profile, DefaultProfilePath);
                return true;
            }

            Debug.LogWarning("Profile " + profile + " does not exists!");
            return false;
        }
    }
}