#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChamberEditor : EditorWindow
{
    private ChamberSO chamberSo;
    
    
    [MenuItem("Window/Chamber Editor")]
    public static void ShowWindow()
    {
        GetWindow<ChamberEditor>("Chamber Editor");
    }
    
}

#endif