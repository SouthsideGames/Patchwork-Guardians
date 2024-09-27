using System.Collections.Generic;
using MonsterBattleArena.Monster;
using MonsterBattleArena.MonsterCreator;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleArena.UI.MonsterCreator
{
    public class MonsterPartList : InstancedMonobehaviour<MonsterPartList>
    {
        [SerializeField] private StatsPanel _statsPanel;
        [SerializeField] private MonsterPartListEntry _template;
        [SerializeField] private Button _equipButton;

        private MonsterPartType _currentInspectedType;

        // The currently selected list entry
        private MonsterPartListEntry _current;

        // The buffer for instantiated entries
        private Stack<GameObject> _entries = new Stack<GameObject>();

        private void Start()
        {
            _equipButton.gameObject.SetActive(false);
            _template.gameObject.SetActive(false);
        }

        /// <summary>
        /// Clear the list entries
        /// </summary>
        public void ClearListEntries()
        {
            _statsPanel.UpdateAllTexts();
            ResetEquipButton();

            while (_entries.Count > 0)
            {
                Destroy(_entries.Pop());
            }
        }

        /// <summary>
        /// Load the entries with specific part type
        /// </summary>
        /// <param name="partType"></param>
        public void LoadListEntries(MonsterPartType partType)
        {
            _currentInspectedType = partType;
            ClearListEntries();

            foreach (Player.PlayerInventoryItem item in PlayerProfileManager.CurrentProfile.Inventory.GetItems<MonsterPart>())
            {
                MonsterPart part = ResourceDatabase.Load<MonsterPart>(item.Guid);
                if (part.PartType != partType)
                    continue;

                for (int i = 0; i < item.Count; i++)
                {
                    CreateListEntry(part);
                }
            }
        }

        public void Refresh()
        {
            LoadListEntries(_currentInspectedType);
        }

        private void SetEntryClickEvent(MonsterPartListEntry entry, MonsterPart part, int level)
        {
            entry.OnClick += (ctx) => {
                if (ctx == _current) 
                {
                    _current = null;

                    _statsPanel.UpdateAllTexts();
                    ResetEquipButton();
                }
                else
                {
                    _current = ctx;

                    _statsPanel.Compare(part, level);

                    // Set the click event for the equip button
                    _equipButton.onClick.RemoveAllListeners();
                    _equipButton.onClick.AddListener(() => {

                        // Check current slot
                        MonsterData.PartData partData = MonsterCreatorManager.Current.CurrentMonsterData.GetMonsterPart(part.PartType);
                        if (!partData.IsNull)
                        {
                            MonsterPart equippedPart = ResourceDatabase.Load<MonsterPart>(partData.Id);
                            PlayerProfileManager.CurrentProfile.Inventory.AddItem(equippedPart, partData.Level); 
                        }
                        
                        string id = ResourceGUIDUtility.AppendLevelToGUID(part.Id, level);
                        PlayerProfileManager.CurrentProfile.Inventory.RemoveItem(id);
                        MonsterCreatorManager.Current.ApplyMonsterPart(part, level);
                        Refresh();
                    });

                    // Bring the equip button below the selected entry when selected
                    _equipButton.transform.SetParent(ctx.transform);
                    _equipButton.gameObject.SetActive(true);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_template.transform.parent);
            };
        }

        private void CreateListEntry(MonsterPart monsterPart)
        {
            MonsterPartListEntry entry = Instantiate(_template, _template.transform.parent);
            entry.Set(monsterPart.Sprite, monsterPart.name);
            entry.gameObject.SetActive(true);

            _entries.Push(entry.gameObject);

            // TODO: Assign the part's level
            SetEntryClickEvent(entry, monsterPart, 0);
        }

        private void ResetEquipButton()
        {
            _equipButton.gameObject.SetActive(false);
            _equipButton.transform.SetParent(_template.transform.parent);
        }
    }
}
