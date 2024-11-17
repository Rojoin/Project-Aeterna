using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    public AK.Wwise.Event Attack;
    public AK.Wwise.Event Death;
    public AK.Wwise.Event TakeDamage;
    public AK.Wwise.Event RandomSound;
    public AK.Wwise.Event SpecialSound;

    public void PlayAttack()
    {
        AkSoundEngine.PostEvent(Attack.Id, gameObject);
    }

    public void PlayDeath()
    {
        AkSoundEngine.PostEvent(Death.Id, gameObject);
    }

    public void PlayTakeDamage()
    {
        AkSoundEngine.PostEvent(TakeDamage.Id, gameObject);
    }

    public void PlayRandomSound()
    {
        AkSoundEngine.PostEvent(RandomSound.Id, gameObject);
    }

    public void PlaySpecialSound()
    {
        AkSoundEngine.PostEvent(SpecialSound.Id, gameObject);
    }
}
