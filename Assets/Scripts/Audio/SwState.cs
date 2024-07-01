using UnityEngine;

public class SwState : MonoBehaviour
{
    public string stateGroup;
    
    public void SetState(string newState)
    {
        AkSoundEngine.SetState(stateGroup, newState);
    }
}
