using UnityEditor;
using UnityEngine;

public class PrefabViewer : EditorWindow
{
    private GameObject prefab;
    private GameObject instantiatedPrefab;

    [MenuItem("Window/Prefab Viewer")]
    public static void ShowWindow()
    {
        GetWindow<PrefabViewer>("Prefab Viewer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Prefab Viewer", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        if (GUILayout.Button("Load Prefab"))
        {
            LoadPrefab();
        }

        if (instantiatedPrefab != null)
        {
            EditorGUILayout.Space();
            GUILayout.Label("Prefab Parts", EditorStyles.boldLabel);

            foreach (Transform child in instantiatedPrefab.transform)
            {
                EditorGUILayout.ObjectField(child.gameObject.name, child.gameObject, typeof(GameObject), true);
            }

            if (GUILayout.Button("Save Changes"))
            {
                SavePrefabChanges();
            }
        }
    }

    private void LoadPrefab()
    {
        if (instantiatedPrefab != null)
        {
            DestroyImmediate(instantiatedPrefab);
        }

        if (prefab != null)
        {
            instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instantiatedPrefab.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    private void SavePrefabChanges()
    {
        if (instantiatedPrefab != null && prefab != null)
        {
            PrefabUtility.SaveAsPrefabAsset(instantiatedPrefab, AssetDatabase.GetAssetPath(prefab));
            AssetDatabase.Refresh();
        }
    }

    private void OnDestroy()
    {
        if (instantiatedPrefab != null)
        {
            DestroyImmediate(instantiatedPrefab);
        }
    }
}