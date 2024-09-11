using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewButtonAdder : MonoBehaviour
{
    public GameObject PrefabSprite;
    public PlacementSystem placementSystem;
    public PrefabPreviewCapture cameraPreview;

    public ObjectsDatabaseSO DatabaseSo;
    public Transform content;
    private void Start()
    {
        StartCoroutine(CallScript());
    }

    IEnumerator CallScript()
    {
        yield return new WaitForSeconds(0.01f);
        
        for (int i = 0; i < DatabaseSo.objectsData.Count; i++)
        {
            int index = i;

            GameObject newImage = Instantiate(PrefabSprite, content);
            Button buttonComponent = newImage.GetComponent<Button>();
            
            cameraPreview.CapturePrefabImage(buttonComponent, DatabaseSo.objectsData[i].Prefab);
            
            buttonComponent.onClick.AddListener(() => placementSystem.StartPlacement(DatabaseSo.objectsData[index].ID));

            yield return new WaitForSeconds(0.01f);
        }
    }
}
