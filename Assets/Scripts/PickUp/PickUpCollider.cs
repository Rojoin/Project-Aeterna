using UnityEngine;
using UnityEngine.Events;

public class PickUpCollider : MonoBehaviour, IInteractable
{
    public UnityEvent onPlayerChooseCard;
    public UnityEvent onPlayerHealthPickUp;
    public BoolChannelSO showTextBox;
    

    public void ToggleDialogBox(bool interact)
    {
        showTextBox.RaiseEvent(interact);
    }
    public void Interact(int interact)
    {
        switch (interact)
        {
            case 0:
                onPlayerChooseCard.Invoke();
                break;
            case 1:
                onPlayerHealthPickUp.Invoke();
                break;
            default:
                break;
        }
        gameObject.SetActive(false);
    }
}

public interface IInteractable
{ 
    void Interact(int interact);
    void ToggleDialogBox(bool value);
}
