using CustomSceneSwitcher.Switcher;
using CustomSceneSwitcher.Switcher.Data;
using InputControls;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private SceneChangeData GoToGame;
    [SerializeField] private bool isPaused;
    [SerializeField] private CanvasGroup canvas;

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

        Time.timeScale = !isPaused ? 1.0f : 0f;
        InputController.IsGamePaused = isPaused;

        canvas.alpha = isPaused ? 1.0f : 0;
        canvas.interactable = isPaused;
        canvas.blocksRaycasts = isPaused;
    }

    public void GoMenu()
    {
        isPaused = false;

        Time.timeScale = !isPaused ? 1.0f : 0f;
        InputController.IsGamePaused = isPaused;

        canvas.alpha = isPaused ? 1.0f : 0;
        canvas.interactable = isPaused;
        canvas.blocksRaycasts = isPaused;

        SceneSwitcher.ChangeScene(GoToGame);
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
