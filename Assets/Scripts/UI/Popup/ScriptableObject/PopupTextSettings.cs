using UnityEngine;
using TMPro;

namespace MonsterBattleArena.UI
{
    [System.Serializable]
    public class PopupTextSettings
    {
        [SerializeField] private TMP_Text _template;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _positiveColor = Color.green;
        [SerializeField] private Color _negativeColor = Color.red;

        public TMP_Text Template { get => _template; }
        public Color PositiveColor { get => _positiveColor; }
        public Color NegativeColor { get => _negativeColor; }
        public Color DefaultColor { get => _defaultColor; }
    }
}
