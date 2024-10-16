using UnityEngine;
using UnityEngine.Events;

public class DoorBehaviour : MonoBehaviour
{
    public UnityEvent<RoomDirection> OnPlayerInteractDoor;
    public RoomDirection doorDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerInteractDoor?.Invoke(doorDirection);
        }
    }
}