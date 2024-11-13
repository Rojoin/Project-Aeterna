using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public static class ChangeColorOnHit
{
    private static readonly int HitColorEffect = Shader.PropertyToID("_HitColorEffect");

    public static void StartColorChange(this GameObject instance, Material materialToChange, float duration)
    {
        instance.GetComponent<MonoBehaviour>().StartCoroutine(ColorChangeCorroutine(duration, materialToChange));
    }

    private static IEnumerator ColorChangeCorroutine(float colorShiftDuration, Material material)
    {
        float timer = 0;
        float shiftDurationFirstHalf = colorShiftDuration / 2;
        while (timer < shiftDurationFirstHalf)
        {
            material.SetFloat(HitColorEffect, timer / colorShiftDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        material.SetFloat(HitColorEffect, 1);
        while (timer > 0)
        {
            material.SetFloat(HitColorEffect, timer / colorShiftDuration);
            timer -= Time.deltaTime;
            yield return null;
        }

        material.SetFloat(HitColorEffect, 0);
    }
}