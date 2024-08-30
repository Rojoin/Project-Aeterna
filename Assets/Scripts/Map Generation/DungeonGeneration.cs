using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Enemy;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DungeonGeneration : MonoBehaviour
{
    [Header("Level So")] [SerializeField] private LevelRoomsSO levelRoom;
    
    [Header("Level Enemy So")] [SerializeField] private EnemyLevelSO enemyLevelSo;

    [Header("Grid")] [SerializeField] private Vector2 gapBetweenRooms;

    [Header("Player Data")] [SerializeField]
    private GameObject PlayerPrefab;

    [SerializeField] private CharacterController player;
    [SerializeField] private float playerTpPositionY;

   [Header("Camera Data")] [SerializeField]
    private CinemachineVirtualCamera camera;

    [SerializeField] private VoidChannelSO OnEnd;

    private int nCurrentRooms;
    private Queue<DungeonRoom> pendingRooms = new Queue<DungeonRoom>();
    private List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();

    public static Action OnRequestPosition;
    public static Action<Vector3> OnProvidePosition;

    private DungeonRoom ActualPlayerRoom;

    [SerializeField] private PlayerHudInputs selectCardMenu;

    public GameObject transitionGO;

    private Dictionary<(int, int), DungeonRoom> dungeonRoomsLayout = new();
    private Dictionary<RoomForm, List<GameObject>> chambersTypes = new();

    [Serializable]
    private class DungeonRoom
    {
        public int xPosition;
        public int zPosition;
        public RoomForm roomForm;
        public RoomBehaviour roomBehaviour;
        public ProceduralRoomGeneration proceduralRoomGeneration;
        public EnemyManager enemyManager;
        public GameObject dungeonRoomInstance;

        public int NeighboursCount
        {
            get { return _neighbours.Count; }
        }

        private List<Tuple<RoomDirection, DungeonRoom>> _neighbours;

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

        public void DefineRoomForm()
        {
            switch (NeighboursCount)
            {
                case 1:
                    roomForm = RoomForm.U;
                    break;

                case 2:
                    roomForm = GetTwoEntranceForm();
                    break;

                case 3:
                    roomForm = RoomForm.T;
                    break;

                case 4:
                    roomForm = RoomForm.X;
                    break;
            }
        }

        private RoomForm GetTwoEntranceForm()
        {
            if (HasNeighbourInDirection(GetOpositeDirection(_neighbours[0].Item1)))
            {
                return RoomForm.I;
            }
            else
            {
                return RoomForm.L;
            }
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
            if (dungeon.enemyManager)
            {
                dungeon.enemyManager.OnLastEnemyKilled.RemoveListener(OpenDungeonRoom);
            }
        }
    }

    public void GenerateDungeon()
    {
        SetRoomsDivision();

        GenerateDungeonLayout();

        GenerateSpecialRooms();

        CreateRoomConnections();

        InstantiateDungeon();

        SetVisibleRooms();

        Debug.Log(" === DUNGEON HAS BEEN GENERATED === ");
    }

    private void SetRoomsDivision()
    {
        foreach (ChamberSO currentChamber in levelRoom.Chambers)
        {
            if (!chambersTypes.ContainsKey(currentChamber.roomForm))
            {
                chambersTypes[currentChamber.roomForm] = new List<GameObject>();
            }

            chambersTypes[currentChamber.roomForm].Add(currentChamber.roomPrefab);
        }
    }

    private void CreateRoomConnections()
    {
        foreach (DungeonRoom current in dungeonRooms)
        {
            AddNeightbourDungeon(current);
            current.DefineRoomForm();
        }
    }

    private void AddNeightbourDungeon(DungeonRoom current)
    {
        DungeonRoom PossibleNeghitbour;

        if (!current.HasNeighbourInDirection(RoomDirection.LEFT) &&
            dungeonRoomsLayout.TryGetValue((current.xPosition - 1, current.zPosition), out PossibleNeghitbour))
        {
            current.AddNeighbourInDirection(PossibleNeghitbour, RoomDirection.LEFT);
        }

        if (!current.HasNeighbourInDirection(RoomDirection.RIGHT) &&
            dungeonRoomsLayout.TryGetValue((current.xPosition + 1, current.zPosition), out PossibleNeghitbour))
        {
            current.AddNeighbourInDirection(PossibleNeghitbour, RoomDirection.RIGHT);
        }

        if (!current.HasNeighbourInDirection(RoomDirection.UP) &&
            dungeonRoomsLayout.TryGetValue((current.xPosition, current.zPosition + 1), out PossibleNeghitbour))
        {
            current.AddNeighbourInDirection(PossibleNeghitbour, RoomDirection.UP);
        }

        if (!current.HasNeighbourInDirection(RoomDirection.DOWN) &&
            dungeonRoomsLayout.TryGetValue((current.xPosition, current.zPosition - 1), out PossibleNeghitbour))
        {
            current.AddNeighbourInDirection(PossibleNeghitbour, RoomDirection.DOWN);
        }
    }

    private void GenerateDungeonLayout()
    {
        nCurrentRooms = 0;
        DungeonRoom startRoom = new DungeonRoom(0, 0);
        pendingRooms.Enqueue(startRoom);
        dungeonRooms.Add(startRoom);

        AddDungeonToLayout(startRoom);

        while (pendingRooms.Count > 0)
        {
            nCurrentRooms++;
            DungeonRoom currentRoom = pendingRooms.Dequeue();

            int nNeighbours = (nCurrentRooms + pendingRooms.Count < levelRoom.maxRooms)
                ? Random.Range(1, 4)
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
                        AddDungeonToLayout(newNeighbourRoom);
                    }
                }
            }
        }
    }

    private void AddDungeonToLayout(DungeonRoom dungeonRoom)
    {
        dungeonRoomsLayout.Add((dungeonRoom.xPosition, dungeonRoom.zPosition), dungeonRoom);
    }

    private void OpenDungeonRoom()
    {
        ActualPlayerRoom.roomBehaviour.SetRoomDoorState(true);
        StartCoroutine(selectCardMenu.ShowSelectableCardMenu());
    }

    private void TranslatePlayerToNewRoom(RoomDirection direction)
    {
        transitionGO.SetActive(true);
        StartCoroutine(DisableTransition());

        ActualPlayerRoom = ActualPlayerRoom.GetNeighbourDirection(direction);
        camera.Follow = ActualPlayerRoom.dungeonRoomInstance.transform;

        RoomDirection opositeDirection;
        opositeDirection = GetOpositeDirection(direction);

        Transform nextDoorPosition = ActualPlayerRoom.roomBehaviour.GetDoorDirection(opositeDirection);
        
        ActualPlayerRoom.enemyManager.OnEnterNewRoom();

        player.enabled = false;
        player.transform.position = nextDoorPosition.position + (nextDoorPosition.up * playerTpPositionY);
        player.enabled = true;

        SetVisibleRooms();
        if (ActualPlayerRoom.type == RoomTypes.BOSS)
        {
            foreach (DungeonRoom d in dungeonRooms)
            {
                d.dungeonRoomInstance.SetActive(true);
            }

            OnEnd.RaiseEvent();
            player.gameObject.SetActive(false);
        }
    }

    private static RoomDirection GetOpositeDirection(RoomDirection direction)
    {
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

        return opositeDirection;
    }

    private IEnumerator DisableTransition()
    {
        yield return new WaitForSeconds(2);
        transitionGO.SetActive(false);
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

            CurrentRoom = chambersTypes[room.roomForm][Random.Range(0, chambersTypes[room.roomForm].Count)];

            GameObject roomInstance = Instantiate(CurrentRoom,
                new Vector3(room.xPosition * gapBetweenRooms.x, 0, room.zPosition * gapBetweenRooms.y),
                Quaternion.identity, gameObject.transform);

            room.roomBehaviour = roomInstance.GetComponent<RoomBehaviour>();
            room.proceduralRoomGeneration = roomInstance.GetComponent<ProceduralRoomGeneration>();

            //TODO : SET BETTER ENEMY MANAGER
            SetEnemyManager(room, roomInstance);

            room.roomBehaviour.StartRoom();

            roomInstance.transform.Rotate(0, GetFinalRoomRotation(room), 0);

            room.dungeonRoomInstance = roomInstance;
            room.roomBehaviour.PlayerInteractNewDoor.AddListener(TranslatePlayerToNewRoom);

            if (room.type == RoomTypes.START)
            {
                ActualPlayerRoom = room;
                room.roomBehaviour.SetRoomDoorState(true);
                camera.Follow = ActualPlayerRoom.dungeonRoomInstance.transform;
                room.enemyManager.CallEndRoom();
            }
        }
    }

    private void SetEnemyManager(DungeonRoom room, GameObject roomInstance)
    {
        EnemyManager enemyManager = roomInstance.GetComponent<EnemyManager>();

        room.enemyManager = enemyManager;

        room.enemyManager.SetEnemyRoomStats(enemyLevelSo);
        
        room.enemyManager.OnLastEnemyKilled.AddListener(OpenDungeonRoom);
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
        return (RoomDirection)Random.Range(0, 4);
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
        float rng = Random.Range(0f, 1f);
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

    //TODO : Cambiar como funciona y quemar al hdp que lo hizo
    private float GetFinalRoomRotation(DungeonRoom room)
    {
        float finalRotation = 0;
        switch (room.roomForm)
        {
            case RoomForm.U:
                if (room.HasNeighbourInDirection(RoomDirection.UP))
                {
                    finalRotation = 270;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, RoomDirection.UP);
                }
                else if (room.HasNeighbourInDirection(RoomDirection.LEFT))
                {
                    finalRotation = 180;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, RoomDirection.LEFT);
                }
                else if (room.HasNeighbourInDirection(RoomDirection.DOWN))
                {
                    finalRotation = 90;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, RoomDirection.DOWN);
                }

                return finalRotation;
            case RoomForm.I:
                if (room.HasNeighbourInDirection(RoomDirection.LEFT))
                {
                    finalRotation = 90;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.LEFT, RoomDirection.RIGHT);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.DOWN, RoomDirection.LEFT);
                }

                return finalRotation;
            case RoomForm.L:
                if (room.HasNeighbourInDirection(RoomDirection.DOWN) &&
                    room.HasNeighbourInDirection(RoomDirection.LEFT))
                {
                    finalRotation = 90;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.DOWN, RoomDirection.LEFT);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, RoomDirection.DOWN);
                }
                else if (room.HasNeighbourInDirection(RoomDirection.UP) &&
                         room.HasNeighbourInDirection(RoomDirection.LEFT))
                {
                    finalRotation = 180;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.DOWN, RoomDirection.RIGHT);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, RoomDirection.LEFT);
                }
                else if (room.HasNeighbourInDirection(RoomDirection.UP) &&
                         room.HasNeighbourInDirection(RoomDirection.RIGHT))
                {
                    finalRotation = 270;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.DOWN, RoomDirection.RIGHT);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, RoomDirection.UP);
                }

                return finalRotation;
            case RoomForm.T:
                if (room.HasNeighbourInDirection(RoomDirection.DOWN) &&
                    room.HasNeighbourInDirection(RoomDirection.LEFT) &&
                    room.HasNeighbourInDirection(RoomDirection.RIGHT))
                {
                    finalRotation = 90;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.UP, RoomDirection.RIGHT);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, RoomDirection.DOWN);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.DOWN, RoomDirection.LEFT);
                }
                else if (room.HasNeighbourInDirection(RoomDirection.UP) &&
                         room.HasNeighbourInDirection(RoomDirection.DOWN) &&
                         room.HasNeighbourInDirection(RoomDirection.LEFT))
                {
                    finalRotation = 180;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.UP, RoomDirection.DOWN);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, RoomDirection.LEFT);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.DOWN, RoomDirection.UP);
                }
                else if (room.HasNeighbourInDirection(RoomDirection.UP) &&
                         room.HasNeighbourInDirection(RoomDirection.RIGHT) &&
                         room.HasNeighbourInDirection(RoomDirection.LEFT))
                {
                    finalRotation = 270;
                    room.roomBehaviour.SetDoorDirection(RoomDirection.UP, RoomDirection.LEFT);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.RIGHT, RoomDirection.UP);
                    room.roomBehaviour.SetDoorDirection(RoomDirection.DOWN, RoomDirection.RIGHT);
                }

                return finalRotation;
            case RoomForm.X:
                return finalRotation;
        }

        return finalRotation;
    }
}