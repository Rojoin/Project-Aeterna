using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CustomSceneSwitcher.Switcher;

namespace CustomSceneSwitcher.Examples.Scripts
{
    public class LoadingBar : LoadingBarBase
    {
        [Header("UI References")] 
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image progressImage;

        private void Awake()
        {
            Hide();
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void SetProgress(float progress)
        {
            if(progressText)
                progressText.text = $"{(progress * 100):F0}%";
            
            if(progressImage)
                progressImage.fillAmount = progress;
        }
    }
}
