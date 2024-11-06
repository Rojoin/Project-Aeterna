using InputControls;
using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
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
        InputController.IsGamePaused = !isPaused;

        canvas.alpha = isPaused ? 1.0f : 0;
        canvas.interactable = isPaused;
        canvas.blocksRaycasts = isPaused;
    }

    private void OnDisable()
    {
        OnPauseChannel.Unsubscribe(PauseLogic);
    }
}
