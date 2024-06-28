using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DoorBehaviour : MonoBehaviour
{
    private PlayerHudInputs playerHudInputs;

    public int timer = 2;

    public UnityEvent<Transform> OnPlayerInteractDoor;
    private void Start()
    {
        playerHudInputs = FindObjectOfType<PlayerHudInputs>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out var ch))
        {
            ch.enabled = false;
            other.transform.position += transform.forward * 12;
            ch.enabled = true;
            StartCoroutine(ShowMenu());
            Debug.Log("On player interact door");
        }
    }

    private IEnumerator ShowMenu() 
    {
        yield return new WaitForSeconds(timer);

        playerHudInputs.ShowSelectableCardMenu();
    }
}