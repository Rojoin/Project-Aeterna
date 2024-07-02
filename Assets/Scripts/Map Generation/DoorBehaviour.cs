using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class DoorBehaviour : MonoBehaviour
{
    public UnityEvent<RoomDirection> OnPlayerInteractDoor;
    public RoomDirection doorDirection;

    private void OnTriggerEnter(Collider other)
    {
        OnPlayerInteractDoor.Invoke(doorDirection);
    }
}