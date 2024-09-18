using System.Collections.Generic;
using Enemy;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Level", menuName = "Enemy Level Stats")]
public class LevelRoomPropsSo : ScriptableObject
{
    public ChamberSO levelRoom;
    public Dictionary<GameObject, Vector3> levelRoomPositions;
}
