using UnityEngine;
using MonsterBattleArena.Monster;
using MonsterBattleArena.UI.MonsterCreator;
using MonsterBattleArena.Util;

namespace MonsterBattleArena.MonsterCreator
{
    public class MonsterCreatorManager : InstancedMonobehaviour<MonsterCreatorManager>
    {
        public const string KEY_MONSTER_TO_EDIT = "monster_to_edit";

        [SerializeField] private MonsterRig _rig;
        [SerializeField] private MonsterCreatorUI _ui;

        private MonsterData _monsterData;

        public event System.Action<MonsterPart> OnMonsterPartUpdated;

        public MonsterData CurrentMonsterData { get => _monsterData; }

        private void Start()
        {
            string monsterToEdit = PlayerPrefs.GetString(KEY_MONSTER_TO_EDIT, string.Empty);

            if (!string.IsNullOrEmpty(monsterToEdit))
                _monsterData = PlayerProfileManager.CurrentProfile.GetMonsterData(monsterToEdit);
            else
                _monsterData = new MonsterData(RandomNameGenerator.GetMonsterName());

            _ui.Initialize(this);

            // Update the sprites for each parts
            SetMonsterPart(_monsterData.GetMonsterPart(MonsterPartType.Head));
            SetMonsterPart(_monsterData.GetMonsterPart(MonsterPartType.Body));
            SetMonsterPart(_monsterData.GetMonsterPart(MonsterPartType.LeftArm));
            SetMonsterPart(_monsterData.GetMonsterPart(MonsterPartType.RightArm));
            SetMonsterPart(_monsterData.GetMonsterPart(MonsterPartType.LeftLeg));
            SetMonsterPart(_monsterData.GetMonsterPart(MonsterPartType.RightLeg));

            PlayerPrefs.DeleteKey(KEY_MONSTER_TO_EDIT);
        }

        public void ApplyMonsterPart(MonsterPart monsterPart, int level)
        {
            SetMonsterPart(monsterPart, level);
            _rig.UpdateMonsterPart(_monsterData);

            OnMonsterPartUpdated?.Invoke(monsterPart);
        }

        private void SetMonsterPart(MonsterPart monsterPart, int level)
        {
            if (monsterPart == null)
                return;
                
            // Set the monster part to the monster data
            _monsterData.SetMonsterPart(monsterPart, level);
        }

        private void SetMonsterPart(MonsterData.PartData partData)
        {
            MonsterPart part = ResourceDatabase.Load<MonsterPart>(partData.Id);
            ApplyMonsterPart(part, partData.Level);
        }

    }
}
