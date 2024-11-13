using UnityEngine;
using UnityEngine.Events;

public class SwBasic : MonoBehaviour
{
    //string event 
    public string eventName;

    //others
    public UnityEvent onPlayAudioEvent;
    
    [ContextMenu("Play Audio Event")]
    public void OnPlayAudio()
    {
        AkSoundEngine.PostEvent(eventName, gameObject);
    }
    
    public void OnPlayAudio(string EventName)
    {
        AkSoundEngine.PostEvent(EventName, gameObject);
    }
}