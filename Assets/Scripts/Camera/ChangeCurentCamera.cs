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
    private bool isGlobalCamera = false;
    public Transform MapCamera;
    private void OnEnable()
    {
        onChangeCameraChannel.Subscribe(ChangeCamera);
        onResetSceneChannel.Subscribe(ResetScene);
    }

    private void OnDisable()
    {
        onChangeCameraChannel.Unsubscribe(ChangeCamera);
        onResetSceneChannel.Unsubscribe(ResetScene);
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
}
