using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DoorBehaviour : MonoBehaviour
{
    public UnityEvent<Transform> OnPlayerInteractDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterController>(out var ch))
        {
            ch.enabled = false;
            other.transform.position += transform.forward * 12;
            ch.enabled = true;
            Debug.Log("On player interact door");
        }
    }
}