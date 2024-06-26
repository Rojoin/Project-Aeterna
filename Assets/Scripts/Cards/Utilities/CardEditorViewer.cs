using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardEditor : EditorWindow
{
    private GameObject prefab;
    private GameObject instantiatedPrefab;
    private SelectableCardDisplay card;
    private CardSO cardData;
    private Camera sceneCamera;
    private GameObject canvas;
    private RenderTexture renderTexture;
    private Vector2 scrollPos;
    private CardDisplay _cardDisplay;

    [MenuItem("Window/Card Editor")]
    public static void ShowWindow()
    {
        GetWindow<CardEditor>("Card Editor");
    }

    private void OnEnable()
    {
        sceneCamera = new GameObject("SceneCamera").AddComponent<Camera>();
        sceneCamera.hideFlags = HideFlags.HideAndDontSave;
        renderTexture = new RenderTexture(512, 512, 16);
        sceneCamera.targetTexture = renderTexture;
        prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefab/Card/SelectableCardWithSprites.prefab", typeof(GameObject));
    }

    private void OnDisable()
    {
        DestroyImmediate(sceneCamera.gameObject);
        if (renderTexture != null)
        {
            renderTexture.Release();
            DestroyImmediate(renderTexture);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Card Editor", EditorStyles.boldLabel);

      
        cardData = (CardSO)EditorGUILayout.ObjectField("Card Data", cardData, typeof(CardSO), false);

        if (GUILayout.Button("Load Prefab"))
        {
            LoadPrefab();
        }

        if (canvas == null)
        {
            return;
        }
        if (cardData != null)
        {
            EditorGUILayout.Space();
            GUILayout.Label("Card Data", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();

            string newName = EditorGUILayout.TextField("Name", cardData.name);
            cardData.description = EditorGUILayout.TextField("Description", cardData.description);
            cardData.artMidground_1 =
                (Sprite)EditorGUILayout.ObjectField("Art Midground 1", cardData.artMidground_1, typeof(Sprite), false);
            cardData.artMidground_2 =
                (Sprite)EditorGUILayout.ObjectField("Art Midground 2", cardData.artMidground_2, typeof(Sprite), false);
            cardData.artMidground_3 =
                (Sprite)EditorGUILayout.ObjectField("Art Midground 3", cardData.artMidground_3, typeof(Sprite), false);
            cardData.artFrame =
                (Sprite)EditorGUILayout.ObjectField("Art Frame", cardData.artFrame, typeof(Sprite), false);


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(cardData, "Change Card Name");
                cardData.name = newName;
                EditorUtility.SetDirty(cardData);
            }

            if (GUILayout.Button("Save Changes"))
            {
                SaveChangesToScriptableObject();
            }

            EditorGUILayout.Space();
            GUILayout.Label("3D View and RectTransform Editor", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            // 3D View
            card.ShowCardImage(cardData);
            Render3DView();

            // RectTransform Editor
            EditorGUILayout.BeginVertical();
            GUILayout.Label("RectTransform Editor", EditorStyles.boldLabel);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);


            EditorGUILayout.LabelField(card.artMidground_1.name, EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition = EditorGUILayout.Vector3Field("Position", cardData.newPosition1);
            Vector2 newSizeDelta = EditorGUILayout.Vector2Field("Size Delta", cardData.newSizeDelta1);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(card.artMidground_1, "Change RectTransform");
                cardData.newPosition1 = newPosition ;
                cardData.newSizeDelta1 = newSizeDelta;
            }

            EditorGUILayout.LabelField(card.artMidground_2.name, EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition2 = EditorGUILayout.Vector3Field("Position", cardData.newPosition2);
            Vector2 newSizeDelta2 = EditorGUILayout.Vector2Field("Size Delta", cardData.newSizeDelta2);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(card.artMidground_2, "Change RectTransform");
                cardData.newPosition2 = newPosition2 ;
                cardData.newSizeDelta2 = newSizeDelta2;
            }

            EditorGUILayout.LabelField(card.artMidground_3.name, EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            Vector3 newPosition3 = EditorGUILayout.Vector3Field("Position", cardData.newPosition3);
            Vector2 newSizeDelta3 = EditorGUILayout.Vector2Field("Size Delta", cardData.newSizeDelta3);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(card.artMidground_3, "Change RectTransform");
                cardData.newSizeDelta3 = newSizeDelta3;
                cardData.newPosition3 = newPosition3;
            }

            EditorGUILayout.Space();


            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            GUILayout.EndHorizontal();
            instantiatedPrefab.transform.localScale = new Vector3(3, 3, 3);
            var rotation = instantiatedPrefab.transform.rotation;
            rotation.eulerAngles =
                EditorGUILayout.Vector3Field("Rotate Object", instantiatedPrefab.transform.rotation.eulerAngles);
            instantiatedPrefab.transform.rotation = rotation;
        }
        else
        {
        }
    }

    private void LoadPrefab()
    {
        if (instantiatedPrefab != null)
        {
            DestroyImmediate(instantiatedPrefab);
        }
       
        if (cardData == null)
        {
            CreateNewCardSO();
        }
        canvas = new GameObject("Canvas");
        canvas.layer = LayerMask.NameToLayer("ModelViewer");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceCamera;
        canvasComponent.worldCamera = sceneCamera;
        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvas.AddComponent<GraphicRaycaster>();
        canvas.hideFlags = HideFlags.HideAndDontSave;


        // Ensure the Canvas is reset to identity transformations
        canvas.transform.position = Vector3.zero;
        canvas.transform.rotation = Quaternion.identity;
        canvas.transform.localScale = Vector3.one;
        if (prefab != null)
        {
            Undo.RecordObject(this, "Instantiate Prefab");
            instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvas.transform);
            instantiatedPrefab.transform.SetParent(canvas.transform, false);
            instantiatedPrefab.hideFlags = HideFlags.HideAndDontSave;
            instantiatedPrefab.layer = LayerMask.NameToLayer("ModelViewer");
            card = instantiatedPrefab.GetComponent<SelectableCardDisplay>();
            if (card == null)
            {
                Debug.LogWarning("Failed to instantiate CardDisplay.");
            }

            RectTransform rectTransform = instantiatedPrefab.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition3D = Vector3.zero;
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = Vector3.one;
            }
            else
            {
                Debug.LogWarning("Prefab does not contain a RectTransform component.");
            }
        }
        else
        {
            Debug.LogWarning("Failed to instantiate prefab.");
        }

        sceneCamera.transform.position = new Vector3(0, 0, -10);
        sceneCamera.transform.LookAt(Vector3.zero);
        Debug.Log("Canvas created: " + canvas.name);
        if (instantiatedPrefab != null)
        {
            Debug.Log("Prefab instantiated: " + instantiatedPrefab.name);
        }

        sceneCamera.targetTexture = renderTexture;
        sceneCamera.orthographic = true;
        sceneCamera.cullingMask = LayerMask.GetMask("ModelViewer");
    }

    private void SaveChangesToScriptableObject()
    {
        if (cardData != null)
        {
            Undo.RecordObject(cardData, "Save Card Data Changes");
            EditorUtility.SetDirty(cardData);
            AssetDatabase.SaveAssets();
        }
    }

    private void Render3DView()
    {
        if (sceneCamera == null || renderTexture == null)
            return;

        sceneCamera.Render();
        GUILayout.Box(renderTexture, GUILayout.Width(256), GUILayout.Height(256));
    }
    
    private void OnDestroy()
    {
        if (instantiatedPrefab != null)
        {
            DestroyImmediate(instantiatedPrefab);
        }

        if (canvas != null)
        {
            DestroyImmediate(canvas);
        }
    }
    private void CreateNewCardSO()
    {
        cardData = ScriptableObject.CreateInstance<CardSO>();
        cardData.name = "New Card";

        string path = EditorUtility.SaveFilePanelInProject("Save New Card Data", "NewCardSO", "asset", "Enter a name for the new CardSO asset.");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(cardData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}