using UnityEngine;

public class SwState : MonoBehaviour
{
    public void SetState(string EventName)
    {
        AkSoundEngine.PostEvent(EventName, gameObject);
    }
}
