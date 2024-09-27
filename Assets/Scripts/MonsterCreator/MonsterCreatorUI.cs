using UnityEngine;
using UnityEngine.UI;
using MonsterBattleArena.MonsterCreator;
using TMPro;

namespace MonsterBattleArena.UI.MonsterCreator
{
    // TODO: Centralize monster creator UI dependencies here

    public class MonsterCreatorUI : MonoBehaviour
    {
        [SerializeField] private StatsPanel _statsPanel;
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private Button _saveButton;

        private MonsterCreatorManager _monsterCreator;

        public void Initialize(MonsterCreatorManager manager)
        {
            _monsterCreator = manager;

            _nameField.SetTextWithoutNotify(_monsterCreator.CurrentMonsterData.Name);
            _nameField.onValueChanged.AddListener((value) => {
                _monsterCreator.CurrentMonsterData.Name = value;
            });

            _saveButton.onClick.AddListener(() => {
                PlayerProfileManager.CurrentProfile.AddMonsterData(_monsterCreator.CurrentMonsterData);
                PlayerProfileManager.SaveCurrentProfile();
            });
            
            _statsPanel.SetMonstertData(_monsterCreator.CurrentMonsterData);

            _monsterCreator.OnMonsterPartUpdated += (monsterPart) => {
                _statsPanel.UpdateAllTexts();
            };
        }
    }
}