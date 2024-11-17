using System;
using UnityEngine;

public class GridSFX : MonoBehaviour
{
    [SerializeField] private VoidChannelSO OnPlacedTurret;
    public AK.Wwise.Event PlaceTurret;

    private void Awake()
    {
        OnPlacedTurret.Subscribe(PlayPlaceTurret);
    }

    private void OnDestroy()
    {
        OnPlacedTurret.Unsubscribe(PlayPlaceTurret);
    }

    private void PlayPlaceTurret()
    {
        AkSoundEngine.PostEvent(PlaceTurret.Id, gameObject);
    }

}