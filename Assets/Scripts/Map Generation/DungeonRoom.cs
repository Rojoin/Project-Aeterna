using System;
using System.Collections.Generic;
using Enemy;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class DungeonRoom
{
    public int xPosition { get; private set; }
    public int zPosition { get; private set; }
    public RoomBehaviour roomBehaviour { get; set; }
    public ProceduralRoomGeneration proceduralRoomGeneration { get; set; }
    public EnemyManager enemyManager { get; set; }
    public GameObject dungeonRoomInstance { get; set; }
    public RoomForm roomForm { get; private set; }

    private Dictionary<RoomDirection, DungeonRoom> neighbours = new();

    public DungeonRoom(int x, int z)
    {
        xPosition = x;
        zPosition = z;
    }

    public int NeighboursCount => neighbours.Count;

    public void AddNeighbourInDirection(DungeonRoom neighbour, RoomDirection direction)
    {
        if (!neighbours.ContainsKey(direction))
        {
            neighbours[direction] = neighbour;
        }
    }

    public bool HasNeighbourInDirection(RoomDirection direction)
    {
        return neighbours.ContainsKey(direction);
    }

    public void DefineRoomForm()
    {
        int connections = neighbours.Count;
        if (connections == 1)
            roomForm = RoomForm.U;
        else if (connections == 2)
            roomForm = GetTwoEntranceForm();
        else if (connections == 3)
            roomForm = RoomForm.T;
        else
            roomForm = RoomForm.X;
    }

    public List<RoomDirection> GetNeighbourDirections()
    {
        return new List<RoomDirection>(neighbours.Keys);
    }

    private RoomForm GetTwoEntranceForm()
    {
        if (HasNeighbourInDirection(RoomDirection.UP) && HasNeighbourInDirection(RoomDirection.DOWN) ||
            HasNeighbourInDirection(RoomDirection.RIGHT) && HasNeighbourInDirection(RoomDirection.LEFT))
        {
            return RoomForm.I;
        }
        else
        {
            return RoomForm.L;
        }
    }
}