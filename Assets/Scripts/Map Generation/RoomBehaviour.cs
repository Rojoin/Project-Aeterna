using System;
using UnityEngine;
using UnityEngine.Events;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject doorsUp;
    public GameObject doorsDown;
    public GameObject doorsRight;
    public GameObject doorsLeft;

    public UnityEvent<RoomDirection> OnInteractDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == doorsUp)
        {
            OnInteractDoor.Invoke(RoomDirection.UP);
        }
        else if (other.gameObject == doorsDown)
        {
            OnInteractDoor.Invoke(RoomDirection.DOWN);

        }
        else if (other.gameObject == doorsRight)
        {
            OnInteractDoor.Invoke(RoomDirection.RIGHT);

        }
        else if (other.gameObject == doorsLeft)
        {
            OnInteractDoor.Invoke(RoomDirection.LEFT);
        }
    }
}
