using UnityEngine;

public class SwSwitch : MonoBehaviour
{
    public string switchGroup;

    public void SetSwitchState(AK.Wwise.Switch newState)
    {
        AkSoundEngine.SetSwitch(switchGroup, newState.ToString(), gameObject);
    }

    public void SetSwitchState(string newState)
    {
        AkSoundEngine.SetSwitch(switchGroup, newState, gameObject);
    }
}
