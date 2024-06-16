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
        Debug.Log("Play Audio");
        AkSoundEngine.PostEvent(eventName, gameObject);
    }
}