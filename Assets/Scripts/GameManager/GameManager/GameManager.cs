using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject firsButtonSelected;

    [SerializeField] private PlayerHud playerHud;

    private PlayerMovement playerMovement;
    private PlayerCombatController playerCombarController;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCombarController = player.GetComponent<PlayerCombatController>();
        EventSystem.current.SetSelectedGameObject(firsButtonSelected);
    }

    void Update()
    {
        if (playerHud.isHudOpen) 
        {
            playerMovement.enabled = false;
            playerCombarController.enabled = false;
        }

        else 
        {
            playerMovement.enabled = true;
            playerCombarController.enabled = true;
        }
    }
}
