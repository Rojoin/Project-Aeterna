using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class DungeonGeneration : MonoBehaviour
{
    [SerializeField] private LevelRoomsSO levelRoom;
    [SerializeField] private Vector2 gapBetweenRooms;
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private CharacterController player;
    [SerializeField] private CinemachineConfiner cameraConfiner;
    [SerializeField] private float playerTpPositionY;
    private int nCurrentRooms;

    private Queue<DungeonRoom> pendingRooms = new Queue<DungeonRoom>();
    private List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();

    public static Action OnRequestPosition;
    public static Action<Vector3> OnProvidePosition;

    private DungeonRoom ActualPlayerRoom;

    [Serializable]
    private class DungeonRoom
    {
        public int xPosition;
        public int zPosition;
        public RoomBehaviour roomBehaviour;
        public ProceduralRoomGeneration proceduralRoomGeneration;
        public GameObject dungeonRoomInstance;

        public int NeighboursCount
        {
            get { return _neighbours.Count; }
        }

        private List<Tuple<RoomDirection, DungeonRoom>> _neighbours;

        public List<Tuple<RoomDirection, DungeonRoom>> Neighbours
        {
            get { return _neighbours; }
        }

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

    private void OnEnable()
    {
        OnRequestPosition += ProvidePosition;
    }

    private void OnDisable()
    {
        OnRequestPosition -= ProvidePosition;

        foreach (DungeonRoom dungeon in dungeonRooms)
        {
            dungeon.roomBehaviour.PlayerInteractNewDoor.RemoveListener(TranslatePlayerToNewRoom);
        }
    }

    public void GenerateDungeon()
    {
        GenerateDungeonLayout();

        GenerateSpecialRooms();

        InstantiateDungeon();

        SetVisibleRooms();
        
        Debug.Log(" === DUNGEON HAS BEEN GENERATED === ");
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

            int nNeighbours = (nCurrentRooms + pendingRooms.Count < levelRoom.maxRooms)
                ? UnityEngine.Random.Range(1, 4)
                : 0;
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
    }

    private void TranslatePlayerToNewRoom(RoomDirection direction)
    {
        ActualPlayerRoom = ActualPlayerRoom.GetNeighbourDirection(direction);
        cameraConfiner.m_BoundingVolume = ActualPlayerRoom.roomBehaviour.roomConfiner;
        
        RoomDirection opositeDirection;
        switch (direction)
        {
            case RoomDirection.UP:
                opositeDirection = RoomDirection.DOWN;
                break;
            case RoomDirection.RIGHT:
                opositeDirection = RoomDirection.LEFT;
                break;
            case RoomDirection.DOWN:
                opositeDirection = RoomDirection.UP;
                break;
            case RoomDirection.LEFT:
                opositeDirection = RoomDirection.RIGHT;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        Transform nextDoorPosition = ActualPlayerRoom.roomBehaviour.GetDoorDirection(opositeDirection);

        player.enabled = false;
        player.transform.position = nextDoorPosition.position + (nextDoorPosition.forward * 2) +
                                    (nextDoorPosition.up * playerTpPositionY);
        player.enabled = true;

        SetVisibleRooms();
    }

    private void SetVisibleRooms()
    {
        foreach (DungeonRoom d in dungeonRooms)
        {
            d.dungeonRoomInstance.SetActive(d == ActualPlayerRoom);
        }
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


            GameObject roomInstance = Instantiate(CurrentRoom,
                new Vector3(room.xPosition * gapBetweenRooms.x, 0, room.zPosition * gapBetweenRooms.y),
                Quaternion.identity, gameObject.transform);

            room.roomBehaviour = roomInstance.GetComponent<RoomBehaviour>();
            room.proceduralRoomGeneration = roomInstance.GetComponent<ProceduralRoomGeneration>();

            room.roomBehaviour.StartRoom();
                
            room.proceduralRoomGeneration.CreateGrid();
            RotateRoom(room, roomInstance);
            room.proceduralRoomGeneration.CreateObjects();


            if (room.type == RoomTypes.START)
            {
                PlayerPrefab.transform.position = new Vector3(room.xPosition * gapBetweenRooms.x, 0.2f,
                    room.zPosition * gapBetweenRooms.y);
                
                ActualPlayerRoom = room;
                cameraConfiner.m_BoundingVolume = ActualPlayerRoom.roomBehaviour.roomConfiner;
            }
            
            room.dungeonRoomInstance = roomInstance;
            room.roomBehaviour.PlayerInteractNewDoor.AddListener(TranslatePlayerToNewRoom);
        }
    }

    private void RotateRoom(DungeonRoom room, GameObject newRoom)
    {
        RoomBehaviour roomBehaviour = newRoom.GetComponent<RoomBehaviour>();
        ProceduralRoomGeneration proceduralRoomGeneration = newRoom.GetComponent<ProceduralRoomGeneration>();

        if (room.HasNeighbourInDirection(RoomDirection.UP))
        {
            roomBehaviour.SetDoorDirection(RoomDirection.UP, true);
            proceduralRoomGeneration.SetDoorState(RoomDirection.UP);
        }

        if (room.HasNeighbourInDirection(RoomDirection.DOWN))
        {
            roomBehaviour.SetDoorDirection(RoomDirection.DOWN, true);
            proceduralRoomGeneration.SetDoorState(RoomDirection.DOWN);
        }

        if (room.HasNeighbourInDirection(RoomDirection.LEFT))
        {
            roomBehaviour.SetDoorDirection(RoomDirection.LEFT, true);
            proceduralRoomGeneration.SetDoorState(RoomDirection.LEFT);
        }

        if (room.HasNeighbourInDirection(RoomDirection.RIGHT))
        {
            roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, true);
            proceduralRoomGeneration.SetDoorState(RoomDirection.RIGHT);
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

    public static void RequestPosition()
    {
        OnRequestPosition?.Invoke();
    }

    private void ProvidePosition()
    {
        OnProvidePosition?.Invoke(player.transform.position);
    }
}