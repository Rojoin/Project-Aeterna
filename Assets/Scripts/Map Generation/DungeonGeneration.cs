using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum RoomTypes
{
    START = 0,
    EMPTY,
    ENEMIES,
    TREASURE,
    BOSS,
    INVALID
}
public enum RoomDirection
{
    UP = 0,
    RIGHT,
    DOWN,
    LEFT

}

public class DungeonGeneration : MonoBehaviour
{
    [SerializeField] private LevelRoomsSO levelRoom;
    [SerializeField] private Vector2 gapBetweenRooms;
    [SerializeField] private GameObject PlayerPrefab;
    private int nCurrentRooms;

    private Queue<DungeonRoom> pendingRooms = new Queue<DungeonRoom>();
    private List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();
    private List<GameObject> dungeonRoomInstances = new List<GameObject>();

    private DungeonRoom ActualPlayerRoom;

    [Serializable]
    private class DungeonRoom
    {
        public int xPosition;
        public int zPosition;
        public int NeighboursCount { get { return _neighbours.Count; } }

        private List<Tuple<RoomDirection, DungeonRoom>> _neighbours;
        public List<Tuple<RoomDirection, DungeonRoom>> Neighbours { get { return _neighbours; } }

        public RoomTypes type = RoomTypes.INVALID;

        public DungeonRoom(int x, int z)
        {
            xPosition = x;
            zPosition = z;
            _neighbours = new List<Tuple<RoomDirection, DungeonRoom>>();
        }

        public bool HasNeighbourInDirection(RoomDirection direction)
        {
            foreach (Tuple<RoomDirection, DungeonRoom> n in _neighbours)
            {
                if (n.Item1 == direction)
                    return true;
            }
            return false;
        }

        public DungeonRoom GetNeighbourDirection(RoomDirection direction)
        {
            foreach (Tuple<RoomDirection, DungeonRoom> n in _neighbours)
            {
                if (n.Item1 == direction)
                    return n.Item2;
            }
            return null;
        }

        public void AddNeighbourInDirection(DungeonRoom room, RoomDirection direction)
        {
            _neighbours.Add(new Tuple<RoomDirection, DungeonRoom>(direction, room));
        }
    }

    private void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        GenerateDungeonLayout();

        GenerateSpecialRooms();

        InstantiateDungeon();
    }

    private void GenerateDungeonLayout()
    {
        nCurrentRooms = 0;
        DungeonRoom startRoom = new DungeonRoom(0, 0);
        pendingRooms.Enqueue(startRoom);
        dungeonRooms.Add(startRoom);

        while (pendingRooms.Count > 0)
        {
            nCurrentRooms++;
            DungeonRoom currentRoom = pendingRooms.Dequeue();

            int nNeighbours = (nCurrentRooms + pendingRooms.Count < levelRoom.maxRooms) ? UnityEngine.Random.Range(1, 4) : 0;
            for (int i = 0; i < nNeighbours; ++i)
            {
                if (currentRoom.NeighboursCount < 4)
                {
                    RoomDirection newNeighbourDirection = GetRandomNeighbourDirection(currentRoom);
                    (DungeonRoom, bool) newNeighbour = GenerateNeighbour(currentRoom, newNeighbourDirection);
                    DungeonRoom newNeighbourRoom = newNeighbour.Item1;
                    bool neighbourJustCreated = newNeighbour.Item2;
                    currentRoom.AddNeighbourInDirection(newNeighbourRoom, newNeighbourDirection);
                    if (neighbourJustCreated)
                    {
                        pendingRooms.Enqueue(newNeighbourRoom);
                        dungeonRooms.Add(newNeighbourRoom);
                    }
                }
            }
        }

        ActualPlayerRoom = startRoom;

        Debug.Log(" === DUNGEON HAS BEEN GENERATED === ");
    }

    private bool IsThereRoomInPosition(int x, int z)
    {
        bool result = false;

        for (int i = 0; i < dungeonRooms.Count; ++i)
        {
            if (dungeonRooms[i].xPosition == x && dungeonRooms[i].zPosition == z)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private DungeonRoom GetRoomInPosition(int x, int z)
    {
        for (int i = 0; i < dungeonRooms.Count; ++i)
        {
            if (dungeonRooms[i].xPosition == x && dungeonRooms[i].zPosition == z)
            {
                return dungeonRooms[i];
            }
        }
        return null;
    }

    private void InstantiateDungeon()
    {
        foreach (DungeonRoom room in dungeonRooms)
        {
            GameObject CurrentRoom = null;

            switch (room.type)
            {
                case RoomTypes.START:
                    CurrentRoom = levelRoom.StartRoom;
                    break;
                case RoomTypes.EMPTY:
                    CurrentRoom = levelRoom.GenericRoom;
                    break;
                case RoomTypes.ENEMIES:
                    CurrentRoom = levelRoom.GenericRoom;
                    break;
                case RoomTypes.TREASURE:
                    CurrentRoom = levelRoom.GenericRoom;
                    break;
                case RoomTypes.BOSS:
                    CurrentRoom = levelRoom.EndRoom;
                    break;
                case RoomTypes.INVALID:
                    CurrentRoom = levelRoom.GenericRoom;
                    break;
                default:
                    break;
            }

            RotateRoom(room, CurrentRoom);

            GameObject roomInstance = Instantiate(CurrentRoom, new Vector3(room.xPosition * gapBetweenRooms.x, 0, room.zPosition * gapBetweenRooms.y), Quaternion.identity, gameObject.transform);

            if (room.type == RoomTypes.START)
            {
                PlayerPrefab.transform.position = new Vector3(room.xPosition * gapBetweenRooms.x, 0.2f, room.zPosition * gapBetweenRooms.y);
            }

            dungeonRoomInstances.Add(roomInstance);
        }
    }

    private void RotateRoom(DungeonRoom room, GameObject newRoom)
    {
        newRoom.GetComponent<RoomBehaviour>().doorsUp.SetActive(false);
        newRoom.GetComponent<RoomBehaviour>().doorsDown.SetActive(false);
        newRoom.GetComponent<RoomBehaviour>().doorsRight.SetActive(false);
        newRoom.GetComponent<RoomBehaviour>().doorsLeft.SetActive(false);

        if (room.HasNeighbourInDirection(RoomDirection.UP))
        {
            newRoom.GetComponent<RoomBehaviour>().doorsUp.SetActive(true);

        }

        if (room.HasNeighbourInDirection(RoomDirection.DOWN))
        {
            newRoom.GetComponent<RoomBehaviour>().doorsDown.SetActive(true);

        }

        if (room.HasNeighbourInDirection(RoomDirection.LEFT))
        {
            newRoom.GetComponent<RoomBehaviour>().doorsLeft.SetActive(true);

        }

        if (room.HasNeighbourInDirection(RoomDirection.RIGHT))
        {
            newRoom.GetComponent<RoomBehaviour>().doorsRight.SetActive(true);

        }
    }

    private RoomDirection GetRandomNeighbourDirection(DungeonRoom currentRoom)
    {
        bool found = false;
        RoomDirection direction = RoomDirection.UP;
        while (!found)
        {
            direction = GetRandomDirection();
            if (!currentRoom.HasNeighbourInDirection(direction))
                found = true;
        }
        return direction;
    }

    private RoomDirection GetRandomDirection()
    {
        return (RoomDirection)UnityEngine.Random.Range(0, 4);
    }

    private (DungeonRoom, bool) GenerateNeighbour(DungeonRoom currentRoom, RoomDirection direction)
    {
        (DungeonRoom, bool) resultTuple;
        DungeonRoom result;
        bool roomCreated = false;
        (int, int)[] newRoomPositions =
            {
                (currentRoom.xPosition, currentRoom.zPosition + 1),
                (currentRoom.xPosition + 1, currentRoom.zPosition),
                (currentRoom.xPosition, currentRoom.zPosition - 1),
                (currentRoom.xPosition - 1, currentRoom.zPosition)
            };

        (int, int) newPosition = newRoomPositions[(int)direction];
        if (IsThereRoomInPosition(newPosition.Item1, newPosition.Item2))
            result = GetRoomInPosition(newPosition.Item1, newPosition.Item2);
        else
        {
            result = new DungeonRoom(newPosition.Item1, newPosition.Item2);
            roomCreated = true;
        }
        RoomDirection oppositeDirection = (RoomDirection)(((int)direction + 2) % 4);

        result.AddNeighbourInDirection(currentRoom, oppositeDirection);

        resultTuple.Item1 = result;
        resultTuple.Item2 = roomCreated;
        return resultTuple;
    }

    private void GenerateSpecialRooms()
    {
        bool bossGenerated = false;
        dungeonRooms[0].type = RoomTypes.START;

        for (int i = dungeonRooms.Count - 1; i >= 0; --i)
        {
            DungeonRoom room = dungeonRooms[i];
            if (room.NeighboursCount == 1)
            {
                if (!bossGenerated)
                {
                    room.type = RoomTypes.BOSS;
                    bossGenerated = true;
                }
                else
                {
                    if (room.type == RoomTypes.INVALID)
                    {
                        RoomTypes roomType = GetRandomSpecialRoomType();
                        room.type = roomType;
                    }
                }
            }
        }
    }

    private RoomTypes GetRandomSpecialRoomType()
    {
        float rng = UnityEngine.Random.Range(0f, 1f);
        if (rng < 0.5f)
            return RoomTypes.TREASURE;
        else if (rng < 0.9f)
            return RoomTypes.ENEMIES;
        else
            return RoomTypes.EMPTY;
    }
}