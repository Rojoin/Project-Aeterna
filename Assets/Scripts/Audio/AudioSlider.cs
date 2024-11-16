using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    public AK.Wwise.RTPC SetRTPC;
    public string Id;
    public Slider slider;

    private void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(SetNewSliderValue);
    }
    public void SetNewSliderValue(float sliderValue)
    {
        AkSoundEngine.SetRTPCValue(SetRTPC.Name, sliderValue);
        PlayerPrefs.SetFloat(Id, sliderValue);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(SetNewSliderValue);
    }
}
