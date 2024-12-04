using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public BoolChannelSO onChangeCameraChannel;


    public GameObject DeathCamera;
    public List<GameObject> CommonCameras;

    private void OnEnable()
    {
        onChangeCameraChannel?.Subscribe(ChangeCamera);
        foreach (GameObject camera in CommonCameras)
        {
            camera.SetActive(true);
        }

        DeathCamera.SetActive(false);
    }


    private void OnDisable()
    {
        onChangeCameraChannel?.Unsubscribe(ChangeCamera);
    }

    private void ChangeCamera(bool value)
    {
        if (value)
        {
            foreach (GameObject go in CommonCameras)
            {
                go.SetActive(false);
            }

            DeathCamera.SetActive(true);
        }
    }
}