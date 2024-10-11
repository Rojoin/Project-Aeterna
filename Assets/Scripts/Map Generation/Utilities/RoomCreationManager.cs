using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomCreationManager : MonoBehaviour
{
    [Header("Chamber Settings")] [SerializeField]
    private LevelRoomsSO levelRoomsSO;

    [SerializeField] private BaseChamberSo BaseChamberSo;

    [Header("Canvas Settings")] [SerializeField]
    private CanvasChamberViewer chamberViewButtonAdder;

    [SerializeField] private GameObject chamberCanvas;

    [SerializeField] private CanvasPropViewer PropsViewButtonAdder;
    [SerializeField] private GameObject propsCanvas;

    [Header("Others")] [SerializeField] private GameObject EndChamberPersonalization;
    [SerializeField] private ObjectPlacer objectPlacer;

    private int chamberId = 0;

    private void Start()
    {
        EndChamberPersonalization.GetComponent<Button>().onClick.AddListener(SetChamberLevel);
        chamberViewButtonAdder.OnSelectedChamber.AddListener(SetChamberUIFalse);

        StartCoroutine(chamberViewButtonAdder.StartCanvasView());

        EndChamberPersonalization.SetActive(false);
        chamberCanvas.SetActive(true);
        propsCanvas.SetActive(false);
    }

    private void SetChamberUIFalse(int ChamberID)
    {
        this.chamberId = ChamberID;

        EndChamberPersonalization.SetActive(true);
        chamberCanvas.SetActive(false);
        propsCanvas.SetActive(true);

        StartCoroutine(PropsViewButtonAdder.StartCanvasView());
    }

    private void SetChamberLevel()
    {
        List<Props> propsList = new List<Props>();
        List<Props> enemyList = new List<Props>();
        if (objectPlacer.placedGameObjects != null || objectPlacer.placedGameObjects.Count != 0)
        {
            foreach (Props currentProp in objectPlacer.placedGameObjects)
            {
                propsList.Add(new Props(currentProp.prop, currentProp.propPosition));
            }
        }

        if (objectPlacer.placedGameObjects != null || objectPlacer.placedGameObjects.Count != 0)
        {
            foreach (Props currentProp in objectPlacer.placedEnemies)
            {
                enemyList.Add(new Props(currentProp.prop, currentProp.propPosition));
            }
        }

        levelRoomsSO.Chambers.Add(new LevelRoomPropsSo(BaseChamberSo.Chambers[chamberId], propsList, enemyList));
    }
}