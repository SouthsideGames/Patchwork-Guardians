using MonsterBattleArena.Util;
using UnityEditor;
using UnityEngine;

namespace MonsterBattleArena
{
    [CustomPropertyDrawer(typeof(GUIDAttribute))]
    public class GUIDPropertyDrawer : PropertyDrawer
    {
        private bool _initialized = false;
        private bool _typeMismatch = false;

        private void Initialize(SerializedProperty property)
        {
            SerializedObject serializedObject = property.serializedObject;
            GameResource target = (GameResource)serializedObject.targetObject;

            if (target != null)
            {
                if (string.IsNullOrEmpty(property.stringValue))
                {
                    property.stringValue = ResourceGUIDUtility.CreateGUID(target);
                    serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    foreach (GameResource asset in Resources.FindObjectsOfTypeAll<GameResource>())
                    {
                        if (asset != serializedObject.targetObject && asset.Id == property.stringValue)
                        {
                            property.stringValue = ResourceGUIDUtility.CreateGUID(target);
                            serializedObject.ApplyModifiedProperties();

                            Debug.Log("Duplicate guid removed.");
                        }
                    }
                }
            }
            else
            {
                _typeMismatch = true;
            }

            _initialized = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, "GUID Attribute should only used with property of type String!", MessageType.Error);
                return;
            }

            if (!_initialized)
                Initialize(property);

            if (_typeMismatch)
                EditorGUI.HelpBox(position, "GUID Attribute should only be used within a class derived from GameAsset!", MessageType.Error);
            else
                EditorGUI.LabelField(position, string.Format("ID: {0}", property.stringValue), EditorStyles.boldLabel);
        }
    }
}
