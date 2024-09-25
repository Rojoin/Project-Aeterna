using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreationManager : MonoBehaviour
{
    [Header("Chamber Settings")]
    [SerializeField] private LevelRoomsSO levelRoomsSO;
    [SerializeField] private BaseChamberSo BaseChamberSo;

    [Header("Canvas Settings")]
    [SerializeField] private CanvasChamberViewer chamberViewButtonAdder;
    [SerializeField] private GameObject chamberCanvas;

    [SerializeField] private CanvasPropViewer PropsViewButtonAdder;
    [SerializeField] private GameObject propsCanvas;

    [Header("Others")]
    [SerializeField] private Button EndChamberPersonalization;
    [SerializeField] private ObjectPlacer objectPlacer;

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
        List<Props> propsList = new List<Props>();
        foreach (Props currentProp in objectPlacer.placedGameObjects)
        {
            propsList.Add(new Props(currentProp.prop, currentProp.propPosition));
        }

        levelRoomsSO.Chambers.Add(new LevelRoomPropsSo(BaseChamberSo.Chambers[chamberId], propsList));
    }
}