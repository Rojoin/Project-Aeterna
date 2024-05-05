using System;
using UnityEditor;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [Header("Rooms Prefabs")]
    [SerializeField] private Room[] rooms;

    [Header("Grid Settings")]
    public int gridWidth;
    public int gridHeight;
    public float spacing;
    public float gap;
    public GridZone[,] gridZones;

    private void Start()
    {
        CreateGrid();
        GenerateDungeon();
    }

    private void CreateGrid()
    {
        gridZones = new GridZone[gridWidth, gridHeight];

        Vector3 startPosition = transform.position - new Vector3((gridWidth - 1) * (spacing + gap) / 2, 0, (gridHeight - 1) * (spacing + gap) / 2);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 currentPos = startPosition + new Vector3(x * (spacing + gap), 0, y * (spacing + gap));
                gridZones[x, y] = new GridZone(currentPos);
                gridZones[x, y].type = RoomType.Nothing;

            }
        }
    }

    private void GenerateDungeon()
    {
        Vector2 centerZone = new Vector2(gridWidth / 2, gridHeight / 2);
        ApplyNewRoom(rooms[4], centerZone);


    }

    private void ApplyNewRoom(Room room, Vector2 position)
    {
        Instantiate(room.prefabRoom, gridZones[(int)position.x, (int)position.y].position , Quaternion.identity, gameObject.transform);
        gridZones[(int)position.x, (int)position.y].type = room.type;
    }

    private void OnDrawGizmos()
    {
        GUIStyle style;
        Vector3 startPosition = transform.position - new Vector3((gridWidth - 1) * (spacing + gap) / 2, 0, (gridHeight - 1) * (spacing + gap) / 2);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (Application.isPlaying)
                {
                    if (gridZones[x, y].type == RoomType.Nothing)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                    }
                }
                else
                {
                    Gizmos.color = Color.white;
                }

                Vector3 currentPos = startPosition + new Vector3(x * (spacing + gap), 0, y * (spacing + gap));

                Gizmos.DrawWireCube(currentPos, new Vector3(spacing, 0, spacing));

                if (Application.isPlaying)
                {
                    Handles.Label(currentPos, gridZones[x, y].type.ToString());
                }

            }
        }
    }

}

public enum RoomType
{
    TB,
    TR,
    TL,
    LR,
    LB,
    RB,
    End,
    Start,
    Nothing,
}

[Serializable]
public class Room
{
    public GameObject prefabRoom;
    public RoomType type;
}

[System.Serializable]
public class GridZone
{
    public Vector3 position;
    public RoomType type;

    public GridZone(Vector3 pos)
    {
        position = pos;
    }
}