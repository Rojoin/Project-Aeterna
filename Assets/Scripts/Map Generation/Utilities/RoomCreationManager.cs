using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomCreationManager : MonoBehaviour
{
    [SerializeField] private CanvasChamberViewer chamberViewButtonAdder;
    [SerializeField] private GameObject chamberCanvas;
    
    [SerializeField] private CanvasPropViewer PropsViewButtonAdder;
    [SerializeField] private GameObject propsCanvas;

    private void Start()
    {
        chamberViewButtonAdder.OnSelectedChamber.AddListener(SetChamberUIFalse);
        StartCoroutine(chamberViewButtonAdder.StartCanvasView());
        
        chamberCanvas.SetActive(true);
        propsCanvas.SetActive(false);
    }

    private void StartPropViewer()
    {
        
    }

    private void SetChamberUIFalse()
    {
        chamberCanvas.SetActive(false);
        propsCanvas.SetActive(true);
        
        StartCoroutine(PropsViewButtonAdder.StartCanvasView());
    }
}
