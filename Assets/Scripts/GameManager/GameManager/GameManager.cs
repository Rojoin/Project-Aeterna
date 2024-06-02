using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject firsButtonSelected;

    private PlayerMovement playerMovement;
    private PlayerCombatController playerCombarController;

    public bool paused = false;

    private void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCombarController = player.GetComponent<PlayerCombatController>();
        EventSystem.current.SetSelectedGameObject(firsButtonSelected);
    }

    void Update()
    {
        if (paused) 
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
