using CustomSceneSwitcher.Switcher;
using CustomSceneSwitcher.Switcher.Data;
using InputControls;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [FormerlySerializedAs("GoToGame")] [SerializeField]
    private SceneChangeData GoToMenu;

    [SerializeField] private SceneChangeData GameScene;
    [SerializeField] private bool isPaused;
    [SerializeField] private float timeUntilShow = 0.2f;
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private Button firstButton;

    [SerializeField] private VoidChannelSO OnPauseChannel;

    private void OnEnable()
    {
        isPaused = false;
        OnPauseChannel.Subscribe(PauseLogic);
    }

    public void SetIsPaused(bool value)
    {
        isPaused = value;
    }

    public void PauseLogic()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }

        Time.timeScale = !isPaused ? 1.0f : 0f;
        InputController.IsGamePaused = isPaused;

        StartCoroutine(canvas.FadeCanvas(isPaused, timeUntilShow));
        // canvas.alpha = isPaused ? 1.0f : 0;
        // canvas.interactable = isPaused;
        // canvas.blocksRaycasts = isPaused;
    }

    public void GoMenu()
    {
        isPaused = false;

        Time.timeScale = !isPaused ? 1.0f : 0f;
        InputController.IsGamePaused = isPaused;

        canvas.alpha = isPaused ? 1.0f : 0;
        canvas.interactable = isPaused;
        canvas.blocksRaycasts = isPaused;

        SceneSwitcher.ChangeScene(GoToMenu);
    }

    public void Retry()
    {
        isPaused = false;

        Time.timeScale = !isPaused ? 1.0f : 0f;
        InputController.IsGamePaused = isPaused;

        canvas.alpha = isPaused ? 1.0f : 0;
        canvas.interactable = isPaused;
        canvas.blocksRaycasts = isPaused;

        SceneSwitcher.ChangeScene(GameScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDisable()
    {
        OnPauseChannel.Unsubscribe(PauseLogic);
    }
}