using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private Room[] rooms;

    private void Start()
    {

    }

    private void SetRoom()
    {

    }

}

public enum RoomType
{
    I,
    L,
    X,
    T,
    End,
    Start,
}

[Serializable]
public class Room
{
    public GameObject prefabRoom;
    public RoomType type;
}