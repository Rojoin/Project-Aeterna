using UnityEngine;

public class LoadSoundInfo : MonoBehaviour
{
    [SerializeField] private AudioSlider[] Sliders;
    [SerializeField] private float defaultValue;

    private void Start()
    {
        foreach (AudioSlider slider in Sliders)
        {
            if (PlayerPrefs.HasKey(slider.Id))
            {
                slider.slider.value = PlayerPrefs.GetFloat(slider.Id);
            }
            else
            {
                slider.slider.value = defaultValue;
                PlayerPrefs.SetFloat(slider.Id, defaultValue);
            }
        }
    }
}
