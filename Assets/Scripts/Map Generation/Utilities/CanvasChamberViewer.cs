using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasChamberViewer : MonoBehaviour
{
    public GameObject PrefabSprite;
    public PrefabPreviewCapture cameraPreview;

    public BaseChamberSo levelRoomsSo;
    public Transform content;

    [SerializeField] private Vector3 roomPosition;
    [SerializeField] private Vector3 roomRotation;

    private Dictionary<Button, int> buttonsListeners = new Dictionary<Button, int>();

    public UnityEvent<int> OnSelectedChamber;

    public IEnumerator StartCanvasView()
    {
        yield return new WaitForSeconds(0.01f);

        for (int i = 0; i < levelRoomsSo.Chambers.Count; i++)
        {
            GameObject newImage = Instantiate(PrefabSprite, content);
            Button buttonComponent = newImage.GetComponent<Button>();
            
            buttonsListeners.Add(buttonComponent, i);

            GameObject PrefabToCapture = levelRoomsSo.Chambers[i].roomPrefab;
            PrefabToCapture.transform.position = roomPosition;
            PrefabToCapture.transform.rotation = Quaternion.Euler(roomRotation);
            cameraPreview.CapturePrefabImage(buttonComponent, PrefabToCapture, roomPosition, roomRotation);

            buttonComponent.onClick.AddListener(() => SetButtonSettings(buttonsListeners[buttonComponent]));

            yield return new WaitForSeconds(0.01f);
        }
    }

    private void SetButtonSettings(int i)
    {
        OnSelectedChamber.Invoke(i);
        Instantiate(levelRoomsSo.Chambers[i].roomPrefab, new Vector3(0, 1.5f, 0), Quaternion.identity);
    }
}