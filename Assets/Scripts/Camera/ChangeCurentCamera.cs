using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
    private bool isGlobalCamera = false;
    public Transform MapCamera;
    private void OnEnable()
    {
        onChangeCameraChannel.Subscribe(ChangeCamera);
        onResetSceneChannel.Subscribe(ResetScene);
        onExitChannel.Subscribe(Exit);
        skyboxMaterial = skybox.material;
        skyboxMaterialRange = skyboxMaterialRangeMin;
    }

    public void Update()
    {
        if (skyboxMaterialRange < skyboxMaterialRangeMax)
        {
   
        }
        else if
        {
            
        }
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
    }
    private void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }  private void Exit()
    {
       Application.Quit();
       Debug.Log($"Close Application");
    }
}
