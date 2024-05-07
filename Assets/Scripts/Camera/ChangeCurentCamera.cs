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
    private bool isGlobalCamera = false;
    public Transform MapCamera;
    private void OnEnable()
    {
        onChangeCameraChannel.Subscribe(ChangeCamera);
        onResetSceneChannel.Subscribe(ResetScene);
        onExitChannel.Subscribe(Exit);
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
    }
}
