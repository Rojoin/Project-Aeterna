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
        other.transform.position = transform.position + transform.forward * 15;
        Debug.Log("On player interact door");
    }
}
