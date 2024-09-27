using UnityEngine;
using TMPro;
using MonsterBattleArena.Monster;

namespace MonsterBattleArena.UI
{
    public class StatsPanel : MonoBehaviour
    {
        private const string ICN_UP_ARROW = "up_icon";
        private const string ICN_DOWN_ARROW = "down_icon";
        private const string COMP_STR_FORMAT = "{0} > <color=#{1}>{2}<sprite name=\"{3}\"/></color>";
        private const string COMP_STR_NODIFF_FORMAT = "{0} > {1}";

        [SerializeField] private TMP_Text _monsterName;
        [SerializeField] private TMP_Text _intelligence;
        [SerializeField] private TMP_Text _speed;
        [SerializeField] private TMP_Text _attack;
        [SerializeField] private TMP_Text _health;
        [SerializeField] private TMP_Text _defense;
        [SerializeField] private TMP_Text _dodge;
        [SerializeField] private TMP_Text _accuracy;
        [SerializeField] private TMP_Text _criticalChance;
        [Space]
        [SerializeField] private Color _plusColor;
        [SerializeField] private Color _minusColor;

        private MonsterData _monsterData;

        /// <summary>
        /// Sets the monster data
        /// </summary>
        /// <param name="monsterData"></param>
        public void SetMonstertData(MonsterData monsterData)
        {
            _monsterData = monsterData;
            UpdateAllTexts();
        }

        /// <summary>
        /// Update all stat texts
        /// </summary>
        public void UpdateAllTexts()
        {
            if (_monsterData == null)
            {
                if (_monsterName != null)
                    _monsterName.SetText("-");

                _intelligence.SetText("-");
                _speed.SetText("-");
                _attack.SetText("-");
                _health.SetText("-");
                _defense.SetText("-");
                return;
            }

            if (_monsterName != null)
                _monsterName.SetText(_monsterData.Name);

            SetAttributeText(_monsterData, AttributeType.Intelligence);
            SetAttributeText(_monsterData, AttributeType.Speed);
            SetAttributeText(_monsterData, AttributeType.Attack);
            SetAttributeText(_monsterData, AttributeType.Health);
            SetAttributeText(_monsterData, AttributeType.Defense);
            SetAttributeText(_monsterData, AttributeType.Dodge);
            SetAttributeText(_monsterData, AttributeType.Accuracy);
            SetAttributeText(_monsterData, AttributeType.CriticalChance);
        }

        /// <summary>
        /// Compare part with part attached to the current monster
        /// </summary>
        /// <param name="part">to compare</param>
        public void Compare(MonsterPart part, int level)
        {
            MonsterData.PartData partData = _monsterData.GetMonsterPart(part.PartType);

            if (!string.IsNullOrEmpty(partData.Id))
            {
                if (part.Id == partData.Id)
                {
                    UpdateAllTexts();
                    return;
                }

                MonsterPart currentPartAsset = ResourceDatabase.Load<MonsterPart>(partData.Id);

                foreach (MonsterAttribute attr in part.GetAttributes(level))
                {
                    int currentTotal = MonsterDataUtility.GetTotalAttributeValue(_monsterData, attr.AttributeType);
                    int currentPartValue = currentPartAsset.GetAttribute(attr.AttributeType, partData.Level).Value;
                    int appliedValue = currentTotal - currentPartValue + attr.Value;

                    SetComparisonText(currentTotal, appliedValue, attr.AttributeType);
                }
            }
        }

        private void SetComparisonText(int currentAttr, int nextAttr, AttributeType attrType)
        {
            TMP_Text target = GetTextComponent(attrType);
            int diff = Mathf.Abs(currentAttr - nextAttr);

            if (diff > 0)
            {
                string icon = nextAttr > currentAttr ? ICN_UP_ARROW : ICN_DOWN_ARROW;
                string color = nextAttr > currentAttr ? ColorUtility.ToHtmlStringRGB(_plusColor) : ColorUtility.ToHtmlStringRGB(_minusColor);

                target.SetText(string.Format(COMP_STR_FORMAT, currentAttr, color, nextAttr, icon));
            }
            else
            {
                target.SetText(string.Format(COMP_STR_NODIFF_FORMAT, currentAttr, nextAttr));
            }
        }

        private void SetAttributeText(MonsterData monsterData, AttributeType attributeType)
        {
            int attrValue = MonsterDataUtility.GetTotalAttributeValue(monsterData, attributeType);
            GetTextComponent(attributeType).SetText(attrValue.ToString());
        }

        private TMP_Text GetTextComponent(AttributeType attrType)
        {
            switch (attrType)
            {
                case AttributeType.Intelligence:
                    return _intelligence;
                case AttributeType.Speed:
                    return _speed;
                case AttributeType.Attack:
                    return _attack;
                case AttributeType.Defense:
                    return _defense;
                case AttributeType.Health:
                    return _health;
                case AttributeType.Dodge:
                    return _dodge;
                case AttributeType.Accuracy:
                    return _accuracy;
                case AttributeType.CriticalChance:
                    return _criticalChance;
            }

            return null;
        }
    }
}