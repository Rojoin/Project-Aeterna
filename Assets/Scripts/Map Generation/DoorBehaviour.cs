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
        other.GetComponent<CharacterController>().enabled = false;
        other.transform.position += transform.forward * 12;
        other.GetComponent<CharacterController>().enabled = true;

        Debug.Log("On player interact door");
    }
}
