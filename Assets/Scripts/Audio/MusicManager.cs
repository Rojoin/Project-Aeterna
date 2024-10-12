using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AkEvent Music;
    void Start()
    {
        AkSoundEngine.SetState("DeathFloorMusic", "Exploring");
    }
}
