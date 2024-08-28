using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioCollection : MonoBehaviour
{
    public void OnPlayWalkSound()
    {
        AkSoundEngine.PostEvent("Player_Walk_Level1", gameObject);
    }

    public void OnPlayAttackSword()
    {
        AkSoundEngine.PostEvent("Player_Attack_Sword", gameObject);
    }
    
    public void OnPlayRecieveDamage()
    {
        AkSoundEngine.PostEvent("Player_RecieveDamage", gameObject);
    }
}
