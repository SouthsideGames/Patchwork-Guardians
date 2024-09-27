using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace MonsterBattleArena.Editor
{
    [CustomPropertyDrawer(typeof(FuseCost))]
    public class FuseCostTableInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUIContent title = new GUIContent(GetTitle(property.propertyPath));
            EditorGUI.PropertyField(position, property, title, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUIUtility.singleLineHeight * 3;

            return base.GetPropertyHeight(property, label);
        }

        public string GetTitle(string propertyPath)
        {
            Match match = Regex.Match(propertyPath, @"[\d]+(?=\])(?!.*data\[[\d]+\])");
            if (match.Success)
            {
                return string.Format("Tier " + match.Value);
            }

            return "Fuse Cost";
        }
    }
}
