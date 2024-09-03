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
    public float rotationSpeed = 10f;

    private bool increasing = true;
    private bool isGlobalCamera = false;
    public Transform MapCamera;

    private void OnEnable()
    {
        onChangeCameraChannel.Subscribe(ChangeCamera);
        onResetSceneChannel.Subscribe(ResetScene);
        onExitChannel.Subscribe(Exit);
     
        skyboxMaterialRange = skyboxMaterialRangeMin;
    }

    public void Update()
    {
        if (increasing)
        {
            skyboxMaterialRange += rotationSpeed * Time.deltaTime;
            if (skyboxMaterialRange >= skyboxMaterialRangeMax)
            {
                skyboxMaterialRange = skyboxMaterialRangeMax;
                increasing = false;
            }
        }
        else
        {
            skyboxMaterialRange -= rotationSpeed * Time.deltaTime;
            if (skyboxMaterialRange <= skyboxMaterialRangeMin)
            {
                skyboxMaterialRange = skyboxMaterialRangeMin;
                increasing = true;
            }
        }

        // Set the skybox rotation
        RenderSettings.skybox.SetFloat("_Rotation", skyboxMaterialRange);
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
    }

    private void Exit()
    {
        Application.Quit();
        Debug.Log($"Close Application");
    }
}