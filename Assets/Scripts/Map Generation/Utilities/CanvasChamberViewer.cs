using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CanvasChamberViewer : MonoBehaviour
{
    public GameObject PrefabSprite;
    public PrefabPreviewCapture cameraPreview;

    public LevelRoomsSO levelRoomsSo;
    public Transform content;
    
    public Transform roomPosition;

    public Action<bool> OnSelectedChamber;

    public IEnumerator StartCanvasView()
    {
        yield return new WaitForSeconds(0.01f);
        
        for (int i = 0; i < levelRoomsSo.Chambers.Length; i++)
        {
            int index = i;

            GameObject newImage = Instantiate(PrefabSprite, content);
            Button buttonComponent = newImage.GetComponent<Button>();
            
            cameraPreview.CapturePrefabImage(buttonComponent, levelRoomsSo.Chambers[i].roomPrefab);
            
            buttonComponent.onClick.AddListener(() => SetButtonSettings(i));

            yield return new WaitForSeconds(0.01f);
        }
    }

    private GameObject SetButtonSettings(int i)
    {
        OnSelectedChamber.Invoke(true);
        return Instantiate(levelRoomsSo.Chambers[i].roomPrefab, roomPosition.position, Quaternion.identity);
    }
}
