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
        StartCoroutine(chamberViewButtonAdder.StartCanvasView());
        
        chamberCanvas.SetActive(true);
        propsCanvas.SetActive(false);
    }

    private void StartPropViewer()
    {
        
    }
}
