using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera _cmvc;
    public  float ShakeIntensity = 8.5f;
    public  float timer;
    public  float ShakeTime = 0.2f;

    private void Awake()
    {
        _cmvc = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        StopShake();
    }

    public void ShakeCamera()
    {
        CinemachineBasicMultiChannelPerlin _cbeep = _cmvc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbeep.m_AmplitudeGain = ShakeIntensity;
        timer = ShakeTime;
    }

    void StopShake()
    {
        CinemachineBasicMultiChannelPerlin _cbeep = _cmvc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cbeep.m_AmplitudeGain = 0f;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StopShake();
            }
        }
    }
}