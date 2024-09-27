using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleArena.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] private Color _fillColor;
        [Space]
        [SerializeField, Range(0.0f, 1.0f)] private float _value = 0.5f;
        
        private void OnValidate()
        {
            SetFillColor(_fillColor);
            SetValue(_value);
        }

        public void SetFillColor(Color color)
        {
            _fillColor = color;

            if (_fill != null)
                _fill.color = _fillColor;
        }
        
        public void SetValue(float value)
        {
            _value = Mathf.Clamp01(value);

            if (_fill != null)
                _fill.rectTransform.localScale = new Vector2(_value, 1);
        }
    }
}
