using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Level Room List")]
public class LevelRoomsSO : ScriptableObject
{
    public int maxRooms;
    public ChamberSO[] Chambers;
}
