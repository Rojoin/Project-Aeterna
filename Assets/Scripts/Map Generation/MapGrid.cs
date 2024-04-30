using System;
using UnityEditor;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private Room[] rooms;
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float spacing = 1f;
    public float gap = 0.1f;
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
        Instantiate(rooms[4].prefabRoom, gridZones[gridWidth/2, gridHeight/2].position, Quaternion.identity, gameObject.transform);
        gridZones[gridWidth / 2, gridHeight / 2].type = rooms[4].type;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridZones[x, y].type == RoomType.Nothing)
                {
                    Instantiate(rooms[2].prefabRoom, gridZones[x, y].position, Quaternion.identity, gameObject.transform);
                    gridZones[x, y].type = rooms[2].type;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        GUIStyle style;
        Vector3 startPosition = transform.position - new Vector3((gridWidth - 1) * (spacing + gap) / 2, 0, (gridHeight - 1) * (spacing + gap) / 2);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridZones[x, y].type == RoomType.Nothing)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }

                Vector3 currentPos = startPosition + new Vector3(x * (spacing + gap), 0, y * (spacing + gap));

                Gizmos.DrawWireCube(currentPos, new Vector3(spacing, 0, spacing));

                Handles.Label(currentPos, gridZones[x, y].type.ToString());

            }
        }
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