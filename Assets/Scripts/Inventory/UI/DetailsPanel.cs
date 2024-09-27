using MonsterBattleArena.Inventory.ItemFuse;
using MonsterBattleArena.Monster;
using MonsterBattleArena.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleArena.Inventory.UI
{
    public class DetailsPanel : UIPanel
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _fuseButton;
        [SerializeField] private MonsterPreview _preview;
        [SerializeField] private StatsPanel _statsPanel;

        private InventoryUIController _controller;
        private MonsterData _previewData;
        private MonsterPart _currentPart;
        private int _currentPartLevel;

        public void Initialize(InventoryUIController controller)
        {
            _controller = controller;

            _previewData = new MonsterData("Preview");
            _statsPanel.SetMonstertData(_previewData);

            _closeButton.onClick.AddListener(Hide);
            _fuseButton.onClick.AddListener(FuseCurrentPart);

            Hide(null, false);
        }

        public void Show(MonsterPart part, int level)
        {
            _currentPart = part;
            _currentPartLevel = level;
            _previewData.SetMonsterPart(part, level);
            _preview.Set(_previewData);
            _statsPanel.UpdateAllTexts();

            Show(null);
        }

        public void Hide()
        {
            Hide(() => {
                _controller.DeselectAll();
                ResetPreviewAndStats();
            });
        }

        private void FuseCurrentPart()
        {
            Hide(() => 
            {
                ResetPreviewAndStats();

                string id = ResourceGUIDUtility.AppendLevelToGUID(_currentPart.Id, _currentPartLevel);
                ItemFuseManager.Current.FuseItem(id);
            });
        }

        private void ResetPreviewAndStats()
        {
            _previewData.RemoveMonsterPart(MonsterPartType.Head);
            _previewData.RemoveMonsterPart(MonsterPartType.Body);
            _previewData.RemoveMonsterPart(MonsterPartType.LeftArm);
            _previewData.RemoveMonsterPart(MonsterPartType.RightArm);
            _previewData.RemoveMonsterPart(MonsterPartType.LeftLeg);
            _previewData.RemoveMonsterPart(MonsterPartType.RightLeg);
            _preview.Reset();

            _statsPanel.UpdateAllTexts();
        }
    }
}
