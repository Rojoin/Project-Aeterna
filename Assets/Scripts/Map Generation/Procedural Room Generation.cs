using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProceduralRoomGeneration : MonoBehaviour
{
    public void CreateRoomProps(List<Props> propsList)
    {
        foreach (Props currentProp in propsList)
        {
            GameObject newProp = Instantiate(currentProp.prop, transform);
            newProp.transform.localPosition = currentProp.propPosition;
        }
    }
}