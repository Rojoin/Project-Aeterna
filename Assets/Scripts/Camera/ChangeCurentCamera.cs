using InputControls;
using UnityEngine;

public class ChangeCurentCamera : MonoBehaviour
{
    public VoidChannelSO onChangeCameraChannel;
    public VoidChannelSO onResetSceneChannel;
    public VoidChannelSO onExitChannel;
    public Skybox skybox;
    private Material skyboxMaterial;
    private float skyboxMaterialRangeMin = 38;
    private float skyboxMaterialRange = 35;
    private float skyboxMaterialRangeMax = 232;
    public float rotationSpeed = 10f;

    private bool increasing = true;
    private bool isGlobalCamera = false;
    public Transform MapCamera;
    public Transform MainCamera;
    private static readonly int Rotation = Shader.PropertyToID("_Rotation");

    private void OnEnable()
    {
        onChangeCameraChannel.Subscribe(ChangeCamera);
        onResetSceneChannel.Subscribe(ResetScene);
        onExitChannel.Subscribe(Exit);
     
        skyboxMaterialRange = skyboxMaterialRangeMin;
    }



    private void OnDisable()
    {
        onChangeCameraChannel.Unsubscribe(ChangeCamera);
        onResetSceneChannel.Unsubscribe(ResetScene);
        onExitChannel.Unsubscribe(Exit);
    }

    private void ChangeCamera()
    {
        isGlobalCamera = !isGlobalCamera;
        MapCamera.gameObject.SetActive(isGlobalCamera);
        MainCamera.gameObject.SetActive(!isGlobalCamera);
        InputController.IsGamePaused = true;
    }

    private void ResetScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Exit()
    {
        Application.Quit();
        Debug.Log($"Close Application");
    }
}