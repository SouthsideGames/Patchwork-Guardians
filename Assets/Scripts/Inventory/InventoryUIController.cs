using UnityEngine;
using MonsterBattleArena.Inventory.UI;
using MonsterBattleArena.Monster;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace MonsterBattleArena.Inventory
{
    public class InventoryUIController : MonoBehaviour
    {
        [SerializeField] private InventorySlot _template;
        [SerializeField] private DetailsPanel _detailsPanel;

        private InventoryManager _manager;
        private List<InventorySlot> _slots;
        private InventoryMode _currentMode;

        public DetailsPanel DetailsPanel { get => _detailsPanel; }
        
        private int _primarySelection = -1;
        private int _secondarySelection = -1;

        public void Initialize(InventoryManager manager)
        {
            _manager = manager;
            _slots = new List<InventorySlot>();

            Backstack.RegisterBackstack(() => SceneManager.LoadScene("MainMenuScene"));

            _template.gameObject.SetActive(false);
            _detailsPanel.Initialize(this);

            SwitchMode(new NormalMode(this, _manager));
        }

        public void RefreshInventorySlots()
        {
            ClearInventorySlots();

            int index = 0;
            foreach (InventoryItem item in _manager.Items)
            {
                int idxCpy = index; // Copy index to avoid variable capturing
                InventorySlot instance = Instantiate(_template, _template.transform.parent);
                instance.Set(item.Resource.Icon, item.Level, item.Count, () => _currentMode.OnItemClicked(idxCpy));
                instance.gameObject.SetActive(true);

                _slots.Add(instance);
                index++;
            }
        }

        public void SelectInventorySlot(int index, InventorySlot.SelectMode selectMode)
        {
            switch (selectMode)
            {
                case InventorySlot.SelectMode.Primary:
                    if (_primarySelection != -1)
                        _slots[_primarySelection].Deselect();
                    _slots[index].Select(selectMode);
                    _primarySelection = index;
                    break;
                case InventorySlot.SelectMode.Secondary:
                    if (_secondarySelection != -1)
                        _slots[_secondarySelection].Deselect();
                    _slots[index].Select(selectMode);
                    _secondarySelection = index;
                    break;
            }
        }

        public void DeselectAll()
        {
            _primarySelection = -1;
            _secondarySelection = -1;

            foreach (InventorySlot slot in _slots)
            {
                slot.Deselect();
            }
        }

        private void ClearInventorySlots()
        {
            if (_slots.Count <= 0)
                return;

            for (int i = _slots.Count - 1; i >= 0; i--)
            {
                Destroy(_slots[i].gameObject);
                _slots.RemoveAt(i);
            }

            Debug.Log("Inventory slots cleared");
        }

        public void SwitchMode(InventoryMode mode)
        {
            _currentMode?.OnExit();

            _currentMode = mode;
            _currentMode?.OnEnter();
        }

        public abstract class InventoryMode
        {
            protected InventoryManager Manager { get; private set; }
            protected InventoryUIController Controller { get; private set; }

            public InventoryMode(InventoryUIController controller, InventoryManager manager)
            {
                Manager = manager;
                Controller = controller;
            }

            public abstract void OnEnter();
            public abstract void OnItemClicked(int index);
            public abstract void OnExit();
        }

        public class NormalMode : InventoryMode
        {
            public NormalMode(InventoryUIController controller, InventoryManager manager) : base(controller, manager)
            {
            }

            public override void OnEnter()
            {
                Controller.DeselectAll();
            }

            public override void OnExit()
            {
            }

            public override void OnItemClicked(int index)
            {
                InventoryItem item = Manager.GetItem(index);

                if (item.Resource is MonsterPart monsterPart)
                    Controller.DetailsPanel.Show(monsterPart, item.Level);

                Controller.SelectInventorySlot(index, InventorySlot.SelectMode.Primary);
            }
        }
    }
}
