using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public void PlayMove()
    {
        AkSoundEngine.PostEvent("Player_Desplazamiento", gameObject);
    }

    public void PlayRandom()
    {
        AkSoundEngine.PostEvent("Player_SonidoRandom", gameObject);
    }
}
