using UnityEngine;
using UnityEngine.UI;

namespace MonsterBattleArena.BattleSystem.UI
{
    public class GameSpeedControl : MonoBehaviour
    {
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _selectedColor = Color.white;
        [SerializeField] private Button _buttonSpeed0;
        [SerializeField] private Button _buttonSpeed1;
        [SerializeField] private Button _buttonSpeed2;

        private BattleManager _battleManager;
        private Button _currentSelected;

        public void Initialize(BattleManager battleManager)
        {
            _battleManager = battleManager;

            _buttonSpeed0.onClick.AddListener(() => SetGameSpeed(GameSpeed.X1));
            _buttonSpeed1.onClick.AddListener(() => SetGameSpeed(GameSpeed.X2));
            _buttonSpeed2.onClick.AddListener(() => SetGameSpeed(GameSpeed.X3));

            SetGameSpeed(GameSpeed.X1);
        }

        private void SetGameSpeed(GameSpeed gameSpeed)
        {
            Button button = GetButton(gameSpeed);
            button.GetComponent<Image>().color = _selectedColor;

            if (_currentSelected != null)
            {
                _currentSelected.GetComponent<Image>().color = _normalColor;
            }

            _currentSelected = button;
            _battleManager.SetGameSpeed(gameSpeed);
        }

        private Button GetButton(GameSpeed gameSpeed)
        {
            switch (gameSpeed)
            {
                default:
                case GameSpeed.X1:
                    return _buttonSpeed0;
                case GameSpeed.X2:
                    return _buttonSpeed1;
                case GameSpeed.X3:
                    return _buttonSpeed2;
            }
        }
    }
}
