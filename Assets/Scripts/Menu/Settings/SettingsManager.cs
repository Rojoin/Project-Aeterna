using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Settings
{
    public class SettingsManager : MonoBehaviour
    {
        private Resolution[] _resolutions;
        private List<Resolution> filteredResolutions;

        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown fullscreenDropdown;
        private float currentRefreshRate;
        private readonly float minAspectRatio = 1.6f;
        private FullScreenMode currentFullScreen = FullScreenMode.FullScreenWindow;
        private Resolution currentResolution;

        [SerializeField] private Toggle shouldGridReset;
        private int currentResolutionIndex = 0;

        private void Awake()
        {
            _resolutions = Screen.resolutions;
            filteredResolutions = new List<Resolution>();

            resolutionDropdown.ClearOptions();
            currentRefreshRate = Screen.currentResolution.refreshRate;

            foreach (Resolution res in _resolutions)
            {
                if ((res.width / (float)res.height) > minAspectRatio)
                {
                    filteredResolutions.Add(res);
                }
            }

            List<string> options = new List<string>();
            for (int i = 0; i < filteredResolutions.Count; i++)
            {
                string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " +
                                          filteredResolutions[i].refreshRate + " Hz";
                options.Add(resolutionOption);

                if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }

            //shouldGridReset.isOn = playerStats.shouldGridControllerReset;
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            shouldGridReset.onValueChanged.AddListener(ChangeToggleReset);
        }

        private void ChangeToggleReset(bool value)
        {
            //playerStats.shouldGridControllerReset = value;
        }

        private void OnDisable()
        {
            resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
            shouldGridReset.onValueChanged.RemoveListener(ChangeToggleReset);
        }

        private void SetResolution(int resolutionIndex)
        {
            currentResolution = filteredResolutions[resolutionIndex];
            Screen.SetResolution(currentResolution.width, currentResolution.height, currentFullScreen);
            Debug.Log($"Resolution set to {currentResolution.width} x {currentResolution.height}.");
        }

        public void SetFullScreenMode(FullScreenMode fullScreenMode)
        {
            currentFullScreen = fullScreenMode;
            Screen.fullScreenMode = currentFullScreen;
        }
    }
    
}