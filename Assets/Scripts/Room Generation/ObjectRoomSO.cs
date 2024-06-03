using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room Object")]
public class ObjectRoomSO : ScriptableObject
{
    public GameObject prefabObject;
    public Vector2 Area;
    public CellTag zone;

    [Range(0, 1)]
    public float spawnRatio;
}
