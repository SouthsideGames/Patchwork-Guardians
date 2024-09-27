using MonsterBattleArena.Inventory.UI;
using MonsterBattleArena.Monster;
using MonsterBattleArena.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace MonsterBattleArena.Inventory.ItemFuse.UI
{
    public class FusePanel : UIPanel
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _fuseButton;
        [SerializeField] private RectTransform _progressFill;
        [Space]
        [SerializeField] private InventorySlot _item;
        [SerializeField] private InventorySlot _sacrifice;
        [SerializeField] private InventorySlot _cost;
        [SerializeField] private InventorySlot _result;
        [SerializeField] private TMP_Text _warningText;
        [Space]
        [SerializeField] private Sprite _xMark;

        private ItemFuseManager _manager;
        private System.Action _onFuseItem;

        public void Initialize(ItemFuseManager manager, System.Action onFuseItem)
        {
            _manager = manager;
            _onFuseItem = onFuseItem;
            _closeButton.onClick.AddListener(() => Hide(null));
            _fuseButton.onClick.AddListener(() => Fuse());

            Set(null, 0, default);
            Hide(null, false);
        }

        public void Set(MonsterPart part, int level, ItemFuseManager.ItemFuseArgs args)
        {
            if (part == null)
            {
                _item.Set(_xMark, level, 0);
                _item.ToggleCountText(false);

                _sacrifice.Set(_xMark, level, 0);
                _sacrifice.ToggleCountText(false);

                _cost.Set(_xMark, level, 0);
                _cost.ToggleCountText(false);
                
                _result.Set(_xMark, level, 0);
                _result.ToggleCountText(false);

                _warningText.gameObject.SetActive(false);
                _progressFill.localScale = new Vector2(0, 1);
                _fuseButton.interactable = false;

                return;
            }

            _item.Set(part.Icon, level, 1);
            _item.ToggleCountText(true);
            if (args.LevelMaxed)
            {
                _warningText.text = "Already at max level!";
                _warningText.gameObject.SetActive(true);
                return;
            }

            if (!args.HasSpareItem)
            {
                _warningText.text = "You don't have spare item to sacrifice!";
                _warningText.gameObject.SetActive(true);
                return;
            }
            else
            {
                _sacrifice.Set(part.Icon, level, 1);
                _sacrifice.ToggleCountText(true);
            }

            if (!args.SufficientCost)
            {
                _warningText.text = "Insufficient cost!";
                _warningText.gameObject.SetActive(true);
                return;
            }
            else
            {
                GameResource costItem = ResourceDatabase.Load<GameResource>(args.FuseCost.Resource);
                _cost.Set(costItem.Icon, 0, args.FuseCost.Count);
                _cost.ToggleCountText(true);
            }

            _result.Set(part.Icon, level + 1, 1);
            _result.ToggleCountText(true);
            _fuseButton.interactable = true;
        }

        protected override void OnDisable()
        {
            Set(null, 0, default);
        }

        private void Fuse()
        {
            _progressFill.DOScaleX(1.0f, 1.0f).OnComplete(() => 
            {
                _onFuseItem?.Invoke();
                Hide(null);
            });;
        }
    }
}
