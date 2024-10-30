using UnityEngine;
using UnityEngine.Events;

public class DoorBehaviour : MonoBehaviour ,IInteractable
{
    public UnityEvent<RoomDirection> OnPlayerInteractDoor;
    public RoomDirection doorDirection;
    public BoolChannelSO toggleDialogBox;
    
    public void Interact(int interact)
    {
        OnPlayerInteractDoor?.Invoke(doorDirection);
    }

    public void ToggleDialogBox(bool value)
    {
        toggleDialogBox.RaiseEvent(value);
    }
}