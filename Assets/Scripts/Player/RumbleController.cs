using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public static class RumbleController
{
    private static Coroutine isRumbling;


    public static void StartRumble(this GameObject instance, float duration,float forceAmmount)
    {
        if (isRumbling != null)
        {
            instance.GetComponent<MonoBehaviour>().StopCoroutine(isRumbling);
        }

        if (Gamepad.current != null)
        {
            isRumbling = instance.GetComponent<MonoBehaviour>().StartCoroutine(RumbleCoroutine(duration,forceAmmount));
        }
    }

    private static IEnumerator RumbleCoroutine(float rumbleDuration,float lowFrequency)
    {
        Gamepad.current.SetMotorSpeeds(lowFrequency, lowFrequency);
        yield return new WaitForSecondsRealtime(rumbleDuration);
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
}