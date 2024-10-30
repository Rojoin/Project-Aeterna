using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    [Header("Settings")] [SerializeField] private DungeonGeneration dungeonGeneration;
    [SerializeField] private RectTransform miniMapContainer;
    [SerializeField] private RectTransform miniMapContent;

    [SerializeField] private int roomSize = 20;
    [SerializeField] private int roomSpacing = 5;

    [Header("Spite Player")] [SerializeField]
    private Sprite playerPositionSprite;

    [Header("Spite Room")] [SerializeField]
    private Sprite hideRoomSprite;

    [SerializeField] private Sprite revealedRoomSprite;

    [Header("Spite Tunnel")] [SerializeField]
    private Sprite hideBridgeSprite;

    [SerializeField] private Sprite revealedBridgeSprite;

    private Dictionary<(int, int), (RoomForm, float)> roomForm;
    private (int, int) playerPosition = (0, 0);

    private Dictionary<(int, int), Image> roomData = new();
    private Dictionary<((int, int), (int, int)), Image> bridgeData = new();

    private Vector2 finalDirection = Vector2.zero;

    private void OnEnable()
    {
        dungeonGeneration.OnSendChambersValue.AddListener(SetMiniMapValues);
        dungeonGeneration.OnStartChangeRoom.AddListener(MoveMiniMap);
        dungeonGeneration.OnEndChangeRoom.AddListener(UpdateMap);
    }

    private void OnDisable()
    {
        dungeonGeneration.OnSendChambersValue.AddListener(SetMiniMapValues);
        dungeonGeneration.OnStartChangeRoom.AddListener(MoveMiniMap);
        dungeonGeneration.OnEndChangeRoom.AddListener(UpdateMap);
    }

    private void SetMiniMapValues(Dictionary<(int, int), (RoomForm, float)> newValues)
    {
        roomForm = newValues;

        CreateMiniMap();
    }

    private void CreateMiniMap()
    {
        foreach (var room in roomForm)
        {
            (int x, int y) = room.Key;

            GameObject roomObject = new GameObject($"Room_{x}_{y}");
            roomObject.transform.SetParent(miniMapContent);
            roomObject.transform.localScale = Vector3.one;

            Image roomImage = roomObject.AddComponent<Image>();
            if (playerPosition == room.Key)
            {
                roomImage.sprite = playerPositionSprite;
            }
            else
            {
                roomImage.sprite = hideRoomSprite;
            }

            RectTransform rectTransform = roomObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(roomSize, roomSize);
            rectTransform.anchoredPosition = new Vector2(x * (roomSize + roomSpacing), y * (roomSize + roomSpacing));

            roomData.Add(room.Key, roomImage);

            CreateBridges(x, y);
        }

        miniMapContent.Rotate(new Vector3(0, 0, -45));
    }

    private void CreateBridges(int x, int y)
    {
        if (roomForm.ContainsKey((x + 1, y)))
        {
            CreateBridgesObject(x, y, x + 1, y, isHorizontal: true);
        }

        if (roomForm.ContainsKey((x, y + 1)))
        {
            CreateBridgesObject(x, y, x, y + 1, isHorizontal: false);
        }
    }

    private void CreateBridgesObject(int x1, int y1, int x2, int y2, bool isHorizontal)
    {
        GameObject bridgeObject = new GameObject($"Tunnel_{x1}_{y1}_to_{x2}_{y2}");
        bridgeObject.transform.SetParent(miniMapContent);
        bridgeObject.transform.localScale = Vector3.one;

        Image bridgeImage = bridgeObject.AddComponent<Image>();
        bridgeImage.sprite = hideBridgeSprite;

        bridgeData.Add(((x1, y1), (x2, y2)), bridgeImage);

        RectTransform rectTransform = bridgeObject.GetComponent<RectTransform>();

        rectTransform.sizeDelta =
            isHorizontal ? new Vector2(roomSize / 2, roomSize / 4) : new Vector2(roomSize / 4, roomSize / 2);

        Vector2 position1 = new Vector2(x1 * (roomSize + roomSpacing), y1 * (roomSize + roomSpacing));
        Vector2 position2 = new Vector2(x2 * (roomSize + roomSpacing), y2 * (roomSize + roomSpacing));
        rectTransform.anchoredPosition = (position1 + position2) / 2;

        rectTransform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    public void MoveMiniMap(RoomDirection direction)
    {
        finalDirection = Vector2.zero;
        switch (direction)
        {
            case RoomDirection.UP:
                finalDirection = new Vector2(0, 1);
                break;
            case RoomDirection.RIGHT:
                finalDirection = new Vector2(1, 0);
                break;
            case RoomDirection.DOWN:
                finalDirection = new Vector2(0, -1);
                break;
            case RoomDirection.LEFT:
                finalDirection = new Vector2(-1, 0);
                break;
        }
    }

    private void UpdateMap()
    {
        if (finalDirection == Vector2.zero)
            return;

        Vector2 rotatedDirection = Quaternion.Euler(0, 0, -45) * finalDirection;

        float offsetX = -rotatedDirection.x * (roomSize + roomSpacing);
        float offsetY = -rotatedDirection.y * (roomSize + roomSpacing);

        miniMapContent.anchoredPosition += new Vector2(offsetX, offsetY);

        (int, int) lasPlayerPosition = playerPosition;

        playerPosition = (playerPosition.Item1 + (int)finalDirection.x, playerPosition.Item2 + (int)finalDirection.y);

        Debug.LogError(lasPlayerPosition.Item1 + " + " + lasPlayerPosition.Item2 + " / " + playerPosition.Item1 +
                       " + " + playerPosition.Item2);

        roomData[lasPlayerPosition].sprite = revealedRoomSprite;
        roomData[playerPosition].sprite = playerPositionSprite;

        if (bridgeData.ContainsKey((playerPosition, lasPlayerPosition)))
        {
            bridgeData[(playerPosition, lasPlayerPosition)].sprite = revealedBridgeSprite;
        }
        else
        {
            bridgeData[(lasPlayerPosition, playerPosition)].sprite = revealedBridgeSprite;
        }

        finalDirection = Vector2.zero;
    }
}