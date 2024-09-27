using MonsterBattleArena.UI;
using UnityEngine;

namespace MonsterBattleArena
{
    [CreateAssetMenu(menuName = "UI Settings", fileName = "UISettings")]
    public class UISettings : ScriptableObject
    {
        private static UISettings _Instance;

        [SerializeField] private Color _primaryColor;
        [SerializeField] private Color _secondaryColor;
        [SerializeField] private Color _tertiaryColor;

        [SerializeField] private Color _primaryTextColor;
        [SerializeField] private Color _secondaryTextColor;
        [SerializeField] private Color _tertiaryTextColor;

        [Header("Popup")]
        [SerializeField] private Color _popupBackgroundColor = new Color(0, 0, 0, 0.45f);
        [SerializeField] private float _popupTransitionDuration = 0.15f;

        [Header("Inventory")]
        [SerializeField] private Color _slotUnselected;
        [SerializeField] private Color _slotSelectedPrimary;
        [SerializeField] private Color _slotSelectedSecondary;

        [SerializeField] private PopupTextSettings _popupText;

        private static UISettings GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = Resources.Load<UISettings>("UISettings");
                if (_Instance == null)
                {
                    Debug.LogError("Unable to find UISettings asset. Make sure there's UISettings aseet inside Resources folder.");
                }
            }

            return _Instance;
        }

        public static Color PrimaryColor { get => GetInstance()._primaryColor; }
        public static Color SecondaryColor { get => GetInstance()._secondaryColor; }
        public static Color TertiaryColor { get => GetInstance()._tertiaryColor; }
        public static Color PrimaryTextColor { get => GetInstance()._primaryTextColor; }
        public static Color SecondaryTextColor { get => GetInstance()._secondaryTextColor; }
        public static Color TertiaryTextColor { get => GetInstance()._tertiaryTextColor; }
        public static Color PopupBackgroundColor { get => GetInstance()._popupBackgroundColor; }
        public static float PopupTransitionDuration { get => GetInstance()._popupTransitionDuration; }
        public static Color SlotUnselected { get => GetInstance()._slotUnselected; }
        public static Color SlotSelectedPrimary { get => GetInstance()._slotSelectedPrimary; }
        public static Color SlotSelectedSecondary { get => GetInstance()._slotSelectedSecondary; }
        
        public static PopupTextSettings PopupText { get => GetInstance()._popupText; }
    }
}
