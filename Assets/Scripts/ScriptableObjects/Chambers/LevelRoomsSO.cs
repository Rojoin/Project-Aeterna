using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Room List")]
public class LevelRoomsSO : ScriptableObject
{
    public int maxRooms;
    public List<LevelRoomPropsSo> Chambers = new List<LevelRoomPropsSo>();
}