using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class PrefabPreviewCapture : MonoBehaviour
{
    [SerializeField] private Camera previewCamera;
    [SerializeField] private int renderTextureSize = 256;

    private RenderTexture renderTexture;
    private GameObject lastInstance;

    void Start()
    {
        renderTexture = new RenderTexture(renderTextureSize, renderTextureSize, 16);
        previewCamera.targetTexture = renderTexture;
    }
    public void CapturePrefabImage(Button targetButton, GameObject prefabToCapture, Vector3 position, Vector3 rotation)
    {
        lastInstance = Instantiate(prefabToCapture, Vector3.zero, quaternion.identity);
        
        lastInstance.transform.position = previewCamera.transform.position + position;
        
        lastInstance.transform.rotation = Quaternion.Euler(rotation); 
        previewCamera.Render();
        
        Texture2D capturedTexture = new Texture2D(renderTextureSize, renderTextureSize, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        capturedTexture.ReadPixels(new Rect(0, 0, renderTextureSize, renderTextureSize), 0, 0);
        capturedTexture.Apply();
        
        Sprite capturedSprite = Sprite.Create(capturedTexture, new Rect(0, 0, renderTextureSize, renderTextureSize), new Vector2(0.5f, 0.5f));
        targetButton.image.sprite = capturedSprite;
        RenderTexture.active = null;
        
        ClearPreviousInstance();
    }
    
    private void ClearPreviousInstance()
    {
        if (lastInstance != null)
        {
            Destroy(lastInstance);
            lastInstance = null;
        }
    }
}