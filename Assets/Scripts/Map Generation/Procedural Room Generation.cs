using System;
using Unity.Mathematics;
using UnityEngine;

public class ProceduralRoomGeneration : MonoBehaviour
{
    public void CreateRoomProps(LevelRoomPropsSo room)
    {
        foreach (Props currentProp in room.PropsList)
        {
            GameObject newProp = Instantiate(currentProp.prop, transform);
            newProp.transform.localPosition = currentProp.propPosition;
        }
    }
}