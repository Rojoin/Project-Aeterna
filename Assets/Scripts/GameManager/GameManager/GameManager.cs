using System;
using Character;
using System.Collections;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerFSM player;

    [SerializeField] private GameObject firsButtonSelected;
    [SerializeField] private BoolChannelSO TogglePause;

    [SerializeField] private PlayerHud playerHud;

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
        player.ChangeFromPause(value);
    }
    
}