using UnityEngine;
using UnityEngine.Events;

public class DoorBehaviour : MonoBehaviour
{
    public UnityEvent<RoomDirection> OnPlayerInteractDoor;
    public RoomDirection roomDirection;

    private void OnTriggerEnter(Collider other)
    {
        OnPlayerInteractDoor.Invoke(roomDirection);
    }
}