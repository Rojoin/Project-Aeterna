using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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

public class RoomBehaviour : MonoBehaviour
{
    public RoomTypes roomType;
    public BoxCollider roomConfiner;

    [SerializeField]private bool doorsOpened;
    [SerializeField] private GameObject[] doorsGameobject;
    [SerializeField] private GameObject[] particleDoorsGameobject;
    [SerializeField] private DoorBehaviour[] doorCollider;

    private Dictionary<RoomDirection, GameObject> doors = new();

    public UnityEvent<RoomDirection> PlayerInteractNewDoor;

    private void OnEnable()
    {
        SetRoomDoorState(doorsOpened);
        for (int i = 0; i < 4; i++)
        {
            doorCollider[i].OnPlayerInteractDoor.AddListener(PlayerInteractDoor);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < 4; i++)
        {
            doorCollider[i].OnPlayerInteractDoor.RemoveListener(PlayerInteractDoor);
        }
    }

    private void PlayerInteractDoor(RoomDirection direction)
    {
        if (doorsOpened)
        {
            PlayerInteractNewDoor.Invoke(direction);
        }
    }

    [ContextMenu("Open/Close Doors")]
    public void Test()
    {
        SetRoomDoorState(!doorsOpened);
    }

    public void SetRoomDoorState(bool doorIsOpen)
    {
        doorsOpened = doorIsOpen;
        foreach (var pd in particleDoorsGameobject)
        {
            if (doorIsOpen)
            {
                pd.SetActive(false);
                pd.GetComponent<ParticleSystem>().Stop();
            }
            else
            {
                pd.SetActive(true);
                pd.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    public void StartDictionary()
    {
        doors = new Dictionary<RoomDirection, GameObject>
        {
            { RoomDirection.UP, doorsGameobject[0] },
            { RoomDirection.DOWN, doorsGameobject[1] },
            { RoomDirection.LEFT, doorsGameobject[2] },
            { RoomDirection.RIGHT, doorsGameobject[3] }
        };
    }

    public void SetDoorDirection(RoomDirection direction, bool addDoor)
    {
        if (addDoor)
        {
            doors[direction].SetActive(true);
        }
        else
        {
            doors[direction].SetActive(false);
        }
    }

    public GameObject GetDoorDirection(RoomDirection direction)
    {
        return doors[direction];
    }
}