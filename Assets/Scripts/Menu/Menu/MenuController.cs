using System;
using System.Collections;
using CustomSceneSwitcher.Switcher;
using CustomSceneSwitcher.Switcher.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private SceneChangeData GoToGame;
        [SerializeField] private Button startGameButton;
        //[SerializeField] private Button optionsButton;
        //[SerializeField] private Button backOptionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button backCreditsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private CanvasGroup menuCanvas;
        //[SerializeField] private CanvasGroup optionsCanvas;
        [SerializeField] private CanvasGroup creditsCanvas;
        [SerializeField] private TextMeshProUGUI versionNumber;
        private bool isOptionsActive;
        private bool isHowToPlayActive;

        private void Awake()
        {
            Cursor.visible = true;
            versionNumber.text = $"{Application.version}";
            startGameButton.onClick.AddListener(StartGame);
            //optionsButton.onClick.AddListener(OptionsToggle);
            //backOptionsButton.onClick.AddListener(OptionsToggle);
            creditsButton.onClick.AddListener(HowToToggle);
            backCreditsButton.onClick.AddListener(HowToToggle);
            exitButton.onClick.AddListener(ExitGame);
            isOptionsActive = false;
            isHowToPlayActive = false;
        }

        private IEnumerator Start()
        {
            yield return null;
            yield return null;
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(startGameButton.gameObject);
            startGameButton.Select();
            yield break;
        }

        private void OnDestroy()
        {
            startGameButton.onClick.RemoveListener(StartGame);
            //optionsButton.onClick.RemoveListener(OptionsToggle);
            //backOptionsButton.onClick.RemoveListener(OptionsToggle);
            creditsButton.onClick.RemoveListener(HowToToggle);
            backCreditsButton.onClick.RemoveListener(HowToToggle);
            exitButton.onClick.RemoveListener(ExitGame);
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        private void OptionsToggle()
        {
            isOptionsActive = !isOptionsActive;
            //SetCanvasVisibility(optionsCanvas, isOptionsActive);
            menuCanvas.blocksRaycasts = !isOptionsActive;
            if (!isOptionsActive)
            {
                EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(startGameButton.gameObject);
            }
        }

        private void HowToToggle()
        {
            isHowToPlayActive = !isHowToPlayActive;
            SetCanvasVisibility(creditsCanvas, isHowToPlayActive);
            menuCanvas.blocksRaycasts = !isHowToPlayActive;
            if (!isHowToPlayActive)
            {
                EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(startGameButton.gameObject);
            }
        }

        private void StartGame()
        {
            SceneSwitcher.ChangeScene(GoToGame);
        }

        private void SetCanvasVisibility(CanvasGroup canvas, bool state)
        {
            canvas.alpha = state ? 1 : 0;
            canvas.interactable = state;
            canvas.blocksRaycasts = state;
            if (state)
            {
                Button currentbutton = canvas.GetComponentInChildren<Button>();
                if (currentbutton)
                {
                    EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(currentbutton.gameObject);
                }
            }
        }
    }
}