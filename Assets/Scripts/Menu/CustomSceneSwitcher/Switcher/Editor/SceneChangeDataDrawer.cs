using UnityEngine;
using UnityEditor;
using CustomSceneSwitcher.Switcher.Data;

namespace CustomSceneSwitcher.Switcher.Editor
{
    [CustomPropertyDrawer(typeof(SceneChangeData))]
    public class SceneChangeDataDrawer : PropertyDrawer
    {
        private const int InternalPropertiesAmount = 13;
        private const int InternalPropertiesSpaceOffset = 2;
        private const int PropertyHeightOffset = 5;
        private Rect _nextPropertyRect;
        private Rect _scenePropertyRect;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2) * InternalPropertiesAmount
                   + InternalPropertiesAmount * InternalPropertiesSpaceOffset
                   + PropertyHeightOffset;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sceneReference = property.FindPropertyRelative("sceneReference");
            
            var transitionControlPrefab = property.FindPropertyRelative("transitionControlPrefab");
            var transitionSpeed = property.FindPropertyRelative("transitionSpeed");
            var transitionMinTime = property.FindPropertyRelative("minTransitionTime");

            var loadingBarPrefab = property.FindPropertyRelative("loadingBarPrefab");
            var loadingBarVisibility = property.FindPropertyRelative("loadingBarVisibility");
                
            _nextPropertyRect = position;
            _nextPropertyRect.height = EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2);
            _nextPropertyRect.width -= 10;
            _nextPropertyRect.x += 5;

            _scenePropertyRect = _nextPropertyRect;
            _scenePropertyRect.height *= 3;
            _scenePropertyRect.y += EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2) * 2;
            _nextPropertyRect.y += EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2) * 4;

            GUIStyle boxGuiStyle = new GUIStyle(GUI.skin.window)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };

            EditorGUI.BeginProperty(position, label, property);
            {
                GUI.Box(position, label, boxGuiStyle);
                EditorGUI.PropertyField(_scenePropertyRect, sceneReference);
                
                EditorGUI.LabelField(GetNextPropertyRect(1), "Transition Configurations");
                
                EditorGUI.ObjectField(GetNextPropertyRect(1), transitionControlPrefab);
                
                if (transitionControlPrefab.objectReferenceValue == null)
                    EditorGUI.BeginDisabledGroup(true);
                
                transitionSpeed.floatValue = EditorGUI.FloatField(GetNextPropertyRect(1),"Transition Speed", transitionSpeed.floatValue);
                transitionMinTime.floatValue = EditorGUI.FloatField(GetNextPropertyRect(1), "Minimum Transition Time", transitionMinTime.floatValue);
                
                if (transitionControlPrefab.objectReferenceValue == null)
                    EditorGUI.EndDisabledGroup();

                _nextPropertyRect.y += EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2);
                EditorGUI.LabelField(GetNextPropertyRect(1), "Loading Bar Configurations");
                
                EditorGUI.ObjectField(GetNextPropertyRect(1), loadingBarPrefab);
                
                if (loadingBarPrefab.objectReferenceValue == null)
                    EditorGUI.BeginDisabledGroup(true);
                
                var visibilityIndex = (LoadingBarVisibility)EditorGUI.EnumPopup(
                    GetNextPropertyRect(1), "Loading Bar Visibility", (LoadingBarVisibility)loadingBarVisibility.intValue);
                loadingBarVisibility.enumValueIndex = (int)visibilityIndex;
                
                if (loadingBarPrefab.objectReferenceValue == null)
                    EditorGUI.EndDisabledGroup();
            }
            EditorGUI.EndProperty();

            property.serializedObject.ApplyModifiedProperties();
        }

        private Rect GetNextPropertyRect(int heightSize)
        {
            _nextPropertyRect.y += (EditorGUIUtility.singleLineHeight * (EditorGUIUtility.wideMode ? 1 : 2) 
                                    + InternalPropertiesSpaceOffset) * heightSize;
            return _nextPropertyRect;
        }
    }
}
