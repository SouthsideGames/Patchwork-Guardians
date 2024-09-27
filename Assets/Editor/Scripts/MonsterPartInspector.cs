using UnityEditor;
using UnityEngine;
using MonsterBattleArena.Monster;

namespace MonsterBattleArena.Editor
{
    [CustomEditor(typeof(MonsterPart)), CanEditMultipleObjects]
    public class MonsterPartInspector : UnityEditor.Editor
    {
        private GUIStyle _box;
        private GUIStyle _rlElement;
        private GUIStyle _rlHeader;

        public override void OnInspectorGUI()
        {
            _box = new GUIStyle("RL Background")
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(),
                border = new RectOffset()
            };

            _rlElement = new GUIStyle("RL Element");
            _rlHeader = new GUIStyle("RL Header");

            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();
            property.NextVisible(true);
            
            do {
                if (property.propertyPath == "m_Script")
                    continue;

                if (property.propertyPath == "_attributeSet")
                {
                    DrawAttributeSet(property);
                    continue;
                }

                EditorGUILayout.PropertyField(property);
            }
            while (property.NextVisible(false));

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAttributeSet(SerializedProperty attributeSet)
        {
            GUIStyle tierLabel = new GUIStyle("AnimationTimelineTick")
            {
                alignment = TextAnchor.UpperLeft,
                padding = new RectOffset(),
                fontSize = 14,
                fontStyle = FontStyle.Bold,
            };

            GUIStyle rlButton = new GUIStyle("RL FooterButton");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(_rlHeader);
            EditorGUILayout.LabelField("Attribute Set", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(_box);
            EditorGUILayout.Space();
            for (int i = 0; i < attributeSet.arraySize; i++)
            {
                SerializedProperty attribute = attributeSet.GetArrayElementAtIndex(i);
                attribute.NextVisible(true);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Tier " + (i + 1), tierLabel);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus More@2x"), new GUIStyle("IconButton")))
                {
                    GenericMenu addMenu = CreateAddAbilityMenu(attribute);   
                    addMenu.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();

                DrawAttributeElement(attribute);
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(new GUIStyle("RL Footer"));
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x"), rlButton))
            {
                if (attributeSet.arraySize <= 0)
                    return;

                attributeSet.DeleteArrayElementAtIndex(attributeSet.arraySize - 1);
            }
            
            if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Plus@2x"), rlButton))
            {
                attributeSet.InsertArrayElementAtIndex(attributeSet.arraySize);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            
        }

        private void DrawAttributeElement(SerializedProperty attribute)
        {
            for (int i = 0; i < attribute.arraySize; i++)
            {
                SerializedProperty element = attribute.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal(_rlElement);

                EditorGUILayout.LabelField(((AttributeType)element.FindPropertyRelative("_type").enumValueIndex).ToString(), EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(element.FindPropertyRelative("_value"), GUIContent.none);
                if (GUILayout.Button(EditorGUIUtility.IconContent("d_Toolbar Minus@2x"), new GUIStyle("IconButton")))
                {
                    attribute.DeleteArrayElementAtIndex(i);
                    continue;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
        }

        private GenericMenu CreateAddAbilityMenu(SerializedProperty attr)
        {
            GenericMenu genericMenu = new GenericMenu();

            foreach (AttributeType attrType in System.Enum.GetValues(typeof(AttributeType)))
            {
                bool hasAttribute = HasAttribute(attr, attrType);
                genericMenu.AddItem(new GUIContent(attrType.ToString()), 
                    hasAttribute, 
                    hasAttribute ? null : () => AddAttribute(attr, attrType));
            }

            return genericMenu;
        }

        private void AddAttribute(SerializedProperty attr, AttributeType type)
        {
            attr.InsertArrayElementAtIndex(attr.arraySize);
            SerializedProperty property = attr.GetArrayElementAtIndex(attr.arraySize - 1);

            property.FindPropertyRelative("_type").enumValueIndex = (int)type;
            property.FindPropertyRelative("_value").intValue = 0;

            property.serializedObject.ApplyModifiedProperties();
        }

        private bool HasAttribute(SerializedProperty attr, AttributeType type)
        {
            for (int i = 0; i < attr.arraySize; i++)
            {
                SerializedProperty property = attr.GetArrayElementAtIndex(i);
                if (property.FindPropertyRelative("_type").enumValueIndex == (int)type)
                    return true;
            }

            return false;
        }
    }
}