using System;
using Character;
using System.Collections;
using System.Collections.Generic;
using InputControls;
using StateMachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerFSM player;
    [SerializeField] private InputController _inputController;

    [SerializeField] private BoolChannelSO TogglePause;

    [SerializeField] private PlayerHud playerHud;

    [SerializeField] private GameObject firsButtonSelected;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(firsButtonSelected);
        TogglePause.Subscribe(OnPauseToggle);
    }

    private void OnDisable()
    {
        TogglePause.Unsubscribe(OnPauseToggle);
    }

    private void OnPauseToggle(bool value)
    {
        InputController.IsGamePaused = value;
        player.ChangeFromPause(value);
    }

    public GameObject GetSelectedButton()
    {
        return EventSystem.current.currentSelectedGameObject;
    }
}