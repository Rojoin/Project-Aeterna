using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Room List")]
public class LevelRoomsSO : ScriptableObject
{
    public int maxRooms;
    public int deadEnds;

    public GameObject StartRoom;
    public GameObject GenericRoom;
    public GameObject EndRoom;
}
