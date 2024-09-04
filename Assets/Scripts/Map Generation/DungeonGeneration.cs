using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonGeneration : MonoBehaviour
{
    [Header("Scriptable Objects")] [SerializeField]
    private LevelRoomsSO levelRoom;

    [SerializeField] private EnemyLevelSO enemyLevelSo;

    [Header("Channels")] [SerializeField] private VoidChannelSO OnEnd;

    [Header("Grid Settings")] [SerializeField]
    private Vector2 gapBetweenRooms;

    [Header("Player Settings")] [SerializeField]
    private GameObject PlayerPrefab;

    [SerializeField] private CharacterController player;
    [SerializeField] private float playerTpPositionY;

    [Header("Camera Settings")] [SerializeField]
    private CinemachineVirtualCamera camera;

    [SerializeField] private PlayerHudInputs selectCardMenu;
    [SerializeField] private GameObject transitionGO;

    private int nCurrentRooms;
    private int roomsCounter = 0;
    private DungeonRoom actualPlayerRoom;

    private Queue<DungeonRoom> pendingRooms = new();
    private List<DungeonRoom> dungeonRooms = new();
    private Dictionary<(int, int), DungeonRoom> dungeonRoomsLayout = new();
    private Dictionary<RoomForm, List<GameObject>> chambersTypes = new();

    public static Action OnRequestPosition;
    public static Action<Vector3> OnProvidePosition;

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
        foreach (DungeonRoom room in dungeonRooms)
        {
            room.roomBehaviour.PlayerInteractNewDoor.RemoveListener(TranslatePlayerToNewRoom);
            if (room.enemyManager)
                room.enemyManager.OnLastEnemyKilled.RemoveListener(OpenDungeonRoom);
        }
    }

    private void GenerateDungeon()
    {
        SetRoomsDivision();

        GenerateDungeonLayout();
        CreateRoomConnections();

        InstantiateDungeon();
        AssingRoomType();

        SetVisibleRooms();

        Debug.Log(" === DUNGEON HAS BEEN GENERATED === ");
    }

    private void SetRoomsDivision()
    {
        foreach (ChamberSO chamber in levelRoom.Chambers)
        {
            if (!chambersTypes.ContainsKey(chamber.roomForm))
                chambersTypes[chamber.roomForm] = new List<GameObject>();

            chambersTypes[chamber.roomForm].Add(chamber.roomPrefab);
        }
    }

    private void CreateRoomConnections()
    {
        foreach (DungeonRoom room in dungeonRooms)
        {
            AddNeighbours(room);
            room.DefineRoomForm();
        }
    }

    private void AddNeighbours(DungeonRoom room)
    {
        CheckAndAddNeighbour(room, RoomDirection.LEFT, (-1, 0));
        CheckAndAddNeighbour(room, RoomDirection.RIGHT, (1, 0));
        CheckAndAddNeighbour(room, RoomDirection.UP, (0, 1));
        CheckAndAddNeighbour(room, RoomDirection.DOWN, (0, -1));
    }

    private void CheckAndAddNeighbour(DungeonRoom room, RoomDirection direction, (int xOffset, int zOffset) offset)
    {
        if (!room.HasNeighbourInDirection(direction) &&
            dungeonRoomsLayout.TryGetValue((room.xPosition + offset.xOffset, room.zPosition + offset.zOffset),
                out var neighbour))
        {
            room.AddNeighbourInDirection(neighbour, direction);
        }
    }

    private void GenerateDungeonLayout()
    {
        DungeonRoom startRoom = new(0, 0);
        pendingRooms.Enqueue(startRoom);
        dungeonRooms.Add(startRoom);
        AddDungeonToLayout(startRoom);

        while (pendingRooms.Count > 0)
        {
            nCurrentRooms++;
            DungeonRoom currentRoom = pendingRooms.Dequeue();
            int maxNeighbours = (nCurrentRooms + pendingRooms.Count < levelRoom.maxRooms) ? Random.Range(1, 4) : 0;

            for (int i = 0; i < maxNeighbours; i++)
            {
                if (currentRoom.NeighboursCount < 4)
                {
                    RoomDirection direction = GetRandomNeighbourDirection(currentRoom);
                    (DungeonRoom neighbour, bool created) = GenerateNeighbour(currentRoom, direction);
                    currentRoom.AddNeighbourInDirection(neighbour, direction);

                    if (created)
                    {
                        pendingRooms.Enqueue(neighbour);
                        dungeonRooms.Add(neighbour);
                        AddDungeonToLayout(neighbour);
                    }
                }
            }
        }
    }

    private void AddDungeonToLayout(DungeonRoom room)
    {
        dungeonRoomsLayout[(room.xPosition, room.zPosition)] = room;
    }

    private void OpenDungeonRoom()
    {
        actualPlayerRoom.roomBehaviour.SetRoomDoorState(true);
        if (roomsCounter >= 1)
        {
            StartCoroutine(selectCardMenu.ShowSelectableCardMenu());
            roomsCounter = 0;
        }
        else
        {
            roomsCounter++;
        }
    }

    private void TranslatePlayerToNewRoom(RoomDirection direction)
    {
        transitionGO.SetActive(true);
        StartCoroutine(DisableTransition(direction));
    }

    private IEnumerator DisableTransition(RoomDirection direction)
    {
        actualPlayerRoom = actualPlayerRoom.GetNeighbourDirection(direction);
        camera.transform.position =
            actualPlayerRoom.dungeonRoomInstance.transform.position + new Vector3(6.24f, 4.67f, -6.24f);
        yield return new WaitForSecondsRealtime(1);

        RoomDirection oppositeDirection = GetOppositeDirection(direction);
        Transform nextDoorPosition = actualPlayerRoom.roomBehaviour.GetDoorDirection(oppositeDirection);
        actualPlayerRoom.enemyManager.OnEnterNewRoom();

        player.enabled = false;
        player.transform.position = nextDoorPosition.position + (nextDoorPosition.up * playerTpPositionY);
        player.enabled = true;

        SetVisibleRooms();

        if (actualPlayerRoom.roomBehaviour.roomType == RoomTypes.BOSS)
        {
            foreach (DungeonRoom room in dungeonRooms)
                room.dungeonRoomInstance.SetActive(true);

            OnEnd.RaiseEvent();
            player.gameObject.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(1);
        transitionGO.SetActive(false);
    }

    private void SetVisibleRooms()
    {
        foreach (DungeonRoom room in dungeonRooms)
            room.dungeonRoomInstance.SetActive(room == actualPlayerRoom);
    }

    private RoomDirection GetOppositeDirection(RoomDirection direction) =>
        direction switch
        {
            RoomDirection.UP => RoomDirection.DOWN,
            RoomDirection.RIGHT => RoomDirection.LEFT,
            RoomDirection.DOWN => RoomDirection.UP,
            RoomDirection.LEFT => RoomDirection.RIGHT,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    private RoomDirection GetRandomNeighbourDirection(DungeonRoom room)
    {
        RoomDirection direction;
        do
        {
            direction = GetRandomDirection();
        } while (room.HasNeighbourInDirection(direction));

        return direction;
    }

    private RoomDirection GetRandomDirection() =>
        (RoomDirection)Random.Range(0, 4);

    private (DungeonRoom, bool) GenerateNeighbour(DungeonRoom room, RoomDirection direction)
    {
        var (xOffset, zOffset) = direction switch
        {
            RoomDirection.UP => (0, 1),
            RoomDirection.RIGHT => (1, 0),
            RoomDirection.DOWN => (0, -1),
            RoomDirection.LEFT => (-1, 0),
            _ => (0, 0)
        };

        (int x, int z) newPosition = (room.xPosition + xOffset, room.zPosition + zOffset);
        DungeonRoom newRoom = dungeonRoomsLayout.TryGetValue(newPosition, out var existingRoom)
            ? existingRoom
            : new DungeonRoom(newPosition.x, newPosition.z);

        if (newRoom != existingRoom)
        {
            newRoom.AddNeighbourInDirection(room, GetOppositeDirection(direction));
            return (newRoom, true);
        }

        return (existingRoom, false);
    }

    private void AssingRoomType()
    {
        bool bossGenerated = false;

        for (int i = dungeonRooms.Count - 1; i >= 0; i--)
        {
            if (dungeonRooms[i].NeighboursCount == 1)
            {
                dungeonRooms[i].roomBehaviour.roomType = bossGenerated ? RoomTypes.ENEMIES : RoomTypes.BOSS;
                bossGenerated = true;
            }
            else
            {
                dungeonRooms[i].roomBehaviour.roomType = RoomTypes.ENEMIES;
            }
        }


        dungeonRooms[0].roomBehaviour.roomType = RoomTypes.START;
        actualPlayerRoom = dungeonRooms[0];
        dungeonRooms[0].roomBehaviour.SetRoomDoorState(true);
        dungeonRooms[0].enemyManager.CallEndRoom();
    }

    private void InstantiateDungeon()
    {
        foreach (DungeonRoom room in dungeonRooms)
        {
            GameObject prefab = chambersTypes[room.roomForm][Random.Range(0, chambersTypes[room.roomForm].Count)];
            GameObject roomInstance = Instantiate(prefab,
                new Vector3(room.xPosition * gapBetweenRooms.x, 0, room.zPosition * gapBetweenRooms.y),
                Quaternion.identity, transform);

            room.roomBehaviour = roomInstance.GetComponent<RoomBehaviour>();
            room.proceduralRoomGeneration = roomInstance.GetComponent<ProceduralRoomGeneration>();

            SetEnemyManager(room, roomInstance);

            room.roomBehaviour.StartRoom();
            roomInstance.transform.Rotate(0, GetFinalRoomRotation(room), 0);
            room.dungeonRoomInstance = roomInstance;
            room.roomBehaviour.PlayerInteractNewDoor.AddListener(TranslatePlayerToNewRoom);
        }
    }

    private void SetEnemyManager(DungeonRoom room, GameObject roomInstance)
    {
        room.enemyManager = roomInstance.GetComponent<EnemyManager>();
        room.enemyManager.SetEnemyRoomStats(enemyLevelSo);
        room.enemyManager.OnLastEnemyKilled.AddListener(OpenDungeonRoom);
        room.roomBehaviour.SetRoomDoorState(false);
    }

    private void ProvidePosition() => OnProvidePosition?.Invoke(player.transform.position);

    private float GetFinalRoomRotation(DungeonRoom room)
    {
        float finalRotation = 0;

        RotationValues rotationValues = new RotationValues();

        if (rotationValues.RotationConfigs.TryGetValue(room.roomForm, out var configs))
        {
            foreach (var (neighbours, rotation, doorConfigs) in configs)
            {
                if (AllNeighboursPresent(room, neighbours))
                {
                    finalRotation = rotation;
                    SetRoomDoors(room.roomBehaviour, doorConfigs);
                    break;
                }
            }
        }

        return finalRotation;
    }

    private bool AllNeighboursPresent(DungeonRoom room, RoomDirection[] neighbours)
    {
        foreach (var direction in neighbours)
        {
            if (!room.HasNeighbourInDirection(direction))
                return false;
        }

        return true;
    }

    private void SetRoomDoors(RoomBehaviour roomBehaviour, (RoomDirection, RoomDirection)[] doorConfigs)
    {
        foreach (var (from, to) in doorConfigs)
        {
            roomBehaviour.SetDoorDirection(from, to);
        }
    }
}