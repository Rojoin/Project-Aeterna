using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public class DoorColecction
{
    private bool showWalls;
    private bool doorState;

    public bool DoorState
    {
        get { return doorState; }
        set
        {
            SetDoorState(value);

            doorState = value;

            void SetDoorState(bool value)
            {
                foreach (GameObject o in doorGameObjects)
                {
                    o.SetActive(value);
                }
                
                doorCollider.enabled = value;
            }
        }
    }
    
    public bool ShowWalls
    {
        get { return showWalls; }
        set
        {
            SetWallState(value);

            showWalls = value;

            void SetWallState(bool value)
            {
                foreach (GameObject o in wallGameObjects)
                {
                    o.SetActive(value);
                }
            }
        }
    }

    public RoomDirection doorDirection;

    public Transform spawnPosition;

    public List<GameObject> doorGameObjects;
    public List<GameObject> wallGameObjects;

    public DoorBehaviour doorBehaviour;
    public BoxCollider doorCollider;
    public GameObject particleDoorsGameobject;
}

public class RoomBehaviour : MonoBehaviour
{
    public RoomTypes roomType;
    public BoxCollider roomConfiner;

    [SerializeField] private bool doorsOpened;

    [SerializeField] private DoorColecction[] doorColecctions;

    public UnityEvent<RoomDirection> PlayerInteractNewDoor;

    public Animator doorAnimation;

    private void OnEnable()
    {
        foreach (DoorColecction d in doorColecctions)
        {
            d.doorBehaviour.OnPlayerInteractDoor.AddListener(PlayerInteractDoor);
        }
    }

    public void StartRoom()
    {
        SetRoomDoorState(doorsOpened);

        foreach (DoorColecction d in doorColecctions)
        {
            d.DoorState = false;
            d.doorBehaviour.doorDirection = d.doorDirection;
        }
    }

    private void OnDisable()
    {
        foreach (DoorColecction d in doorColecctions)
        {
            d.doorBehaviour.OnPlayerInteractDoor.RemoveListener(PlayerInteractDoor);
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

        foreach (var pd in doorColecctions)
        {
            if (doorIsOpen)
            {
                pd.particleDoorsGameobject.SetActive(false);
                pd.particleDoorsGameobject.GetComponent<ParticleSystem>().Stop();
                doorAnimation.SetTrigger("OpenDoor");
            }
            else
            {
                pd.particleDoorsGameobject.SetActive(true);
                pd.particleDoorsGameobject.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    public Transform GetDoorDirection(RoomDirection direction)
    {
        foreach (DoorColecction d in doorColecctions)
        {
            if (d.doorDirection == direction)
            {
                return d.spawnPosition;
            }
        }

        return null;
    }

    public float GetRotaionWithNeighbour()
    {
        
    }
}