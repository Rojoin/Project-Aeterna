using System;
#if UNITY_EDITOR
using UnityEditor;

namespace CustomSceneSwitcher.Switcher.External
{
    public class ExtendedEditorWindow : EditorWindow
    {
        protected SerializedProperty CurrentProperty;
        protected SerializedObject SerializedObject;

        protected void DrawProperties(SerializedProperty property, bool drawChildren)
        {
            string lastPropPath = String.Empty;
            foreach (SerializedProperty prop in property)
            {
                if (prop.isArray && prop.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (prop.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(prop, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if(!string.IsNullOrEmpty(lastPropPath) && prop.propertyPath.Contains(lastPropPath))
                        continue;

                    lastPropPath = prop.propertyPath;
                    EditorGUILayout.PropertyField(prop, drawChildren);
                }
            }
        }
        
    }
}
#endif