using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TablesSFX : MonoBehaviour
{
    public AK.Wwise.Event pickItem;
    public AK.Wwise.Event placeItem;
    public AK.Wwise.Event interactItem;

    public void PlayPickItem()
    {
        AkSoundEngine.PostEvent(pickItem.Id, gameObject);
    }

    public void PlayPlaceItem()
    {
        AkSoundEngine.PostEvent(placeItem.Id, gameObject);
    }

    public void PlayInteractItem()
    {
        AkSoundEngine.PostEvent(interactItem.Id, gameObject);
    }

    public void PlayCantPlaceItem()
    {
        AkSoundEngine.PostEvent("Error_no_se_puede_poner_mas_items_en_la_mesa", gameObject);
    }
}