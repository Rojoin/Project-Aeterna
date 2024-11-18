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
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button backOptionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button backCreditsButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private CanvasGroup menuCanvas;
        [SerializeField] private CanvasGroup optionsCanvas;
        [SerializeField] private CanvasGroup creditsCanvas;
        [SerializeField] private TextMeshProUGUI versionNumber;
        [SerializeField] private BoolChannelSO onControllerChange;
        [SerializeField] private EventSystem eventSystem;
        private bool isOptionsActive;
        private bool isHowToPlayActive;

        private void Awake()
        {
            Cursor.visible = true;
            versionNumber.text = $"{Application.version}";
            startGameButton.onClick.AddListener(StartGame);
            optionsButton.onClick.AddListener(OptionsToggle);
            backOptionsButton.onClick.AddListener(OptionsToggle);
            creditsButton.onClick.AddListener(HowToToggle);
            backCreditsButton.onClick.AddListener(HowToToggle);
            exitButton.onClick.AddListener(ExitGame);
            onControllerChange.Subscribe(SetButton);
            isOptionsActive = false;
            isHowToPlayActive = false;
        }

        private void SetButton(bool value)
        {
            if (!value)
            {
                return;
            }

            if (!isOptionsActive && !isHowToPlayActive)
            {
                EventSystem.current.SetSelectedGameObject(startGameButton.gameObject);
            }
            else if (!isOptionsActive && isHowToPlayActive)
            {
                EventSystem.current.SetSelectedGameObject(backCreditsButton.gameObject);
            }
            else if (isOptionsActive)
            {
                EventSystem.current.SetSelectedGameObject(backOptionsButton.gameObject);
            }
        }

        private IEnumerator Start()
        {
            yield return null;
            yield return null;
            EventSystem.current.SetSelectedGameObject(startGameButton.gameObject);
            startGameButton.Select();
            yield break;
        }

        private void OnDestroy()
        {
            startGameButton.onClick.RemoveListener(StartGame);
            optionsButton.onClick.RemoveListener(OptionsToggle);
            backOptionsButton.onClick.RemoveListener(OptionsToggle);
            creditsButton.onClick.RemoveListener(HowToToggle);
            backCreditsButton.onClick.RemoveListener(HowToToggle);
            exitButton.onClick.RemoveListener(ExitGame);
            onControllerChange.Unsubscribe(SetButton);
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        private void OptionsToggle()
        {
            isOptionsActive = !isOptionsActive;
            SetCanvasVisibility(optionsCanvas, isOptionsActive);
            // menuCanvas.blocksRaycasts = !isOptionsActive;
            if (!isOptionsActive)
            {
                eventSystem.SetSelectedGameObject(startGameButton.gameObject);
            }
            else
            {
                eventSystem.SetSelectedGameObject(backOptionsButton.gameObject);
                backOptionsButton.Select();
            }
        }

        private void HowToToggle()
        {
            isHowToPlayActive = !isHowToPlayActive;
            SetCanvasVisibility(creditsCanvas, isHowToPlayActive);
            // menuCanvas.blocksRaycasts = !isHowToPlayActive;
            if (!isHowToPlayActive)
            {
                eventSystem.SetSelectedGameObject(startGameButton.gameObject);
            }
            else
            {
                eventSystem.SetSelectedGameObject(backCreditsButton.gameObject);
                backCreditsButton.Select();
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
        }
    }
}