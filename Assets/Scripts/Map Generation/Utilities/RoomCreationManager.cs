using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreationManager : MonoBehaviour
{
    [SerializeField]private LevelRoomsSO levelRoomsSO;
    
    [SerializeField] private CanvasChamberViewer chamberViewButtonAdder;
    [SerializeField] private GameObject chamberCanvas;

    [SerializeField] private CanvasPropViewer PropsViewButtonAdder;
    [SerializeField] private GameObject propsCanvas;
    
    [SerializeField]private Button EndChamberPersonalization;

    private int chamberId = 0;

    private void Start()
    {
        EndChamberPersonalization.onClick.AddListener(SetChamberLevel);
        chamberViewButtonAdder.OnSelectedChamber.AddListener(SetChamberUIFalse);
        StartCoroutine(chamberViewButtonAdder.StartCanvasView());

        chamberCanvas.SetActive(true);
        propsCanvas.SetActive(false);
    }

    private void SetChamberUIFalse(int ChamberID)
    {
        this.chamberId = ChamberID;
        
        chamberCanvas.SetActive(false);
        propsCanvas.SetActive(true);
        
        StartCoroutine(PropsViewButtonAdder.StartCanvasView());
    }

    private void SetChamberLevel()
    {
        LevelRoomPropsSo So = LevelRoomPropsSo.CreateInstance<LevelRoomPropsSo>();
        Debug.Log(chamberId);
        string fileName = levelRoomsSO.Chambers[chamberId].roomPrefab.name + "PropsSO.asset";
        string path = "Assets/SO/Chambers/" + fileName;

        while (AssetDatabase.Contains(path))
        {
            
        }
        
        AssetDatabase.CreateAsset(So, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = So;
        
        So.levelRoom = levelRoomsSO.Chambers[chamberId];
    }
}