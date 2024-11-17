using UnityEngine;

public class CouldronSFX : MonoBehaviour
{
    private bool PreparingPotion = false;
    public void PlayPickItem()
    {
        AkSoundEngine.PostEvent("Play_Agarrar_item_Porcion", gameObject);
    }

    public void PlayPlaceItem()
    {
        AkSoundEngine.PostEvent("Play_Poner_Item_caldero", gameObject);
    }

    public void PlayRandom()
    {
        AkSoundEngine.PostEvent("Play_SonidoRandom_Caldero", gameObject);
    }

    public void PlayReadyPotion()
    {

        AkSoundEngine.PostEvent("Play_Pocion_Lista", gameObject);
        PreparingPotion = false;

    }

    public void PlayPreparingPotion()
    {
        if (!PreparingPotion)
        {
            AkSoundEngine.PostEvent("Play_Pocion_Preparandose", gameObject);
            PreparingPotion = true;
        }
    }

    public void PlayOnIteamError()
    {
        AkSoundEngine.PostEvent("Error_no_se_puede_poner_cristal", gameObject);
    }
}
