using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProceduralRoomGeneration : MonoBehaviour
{
    [SerializeField] private bool showGrid;

    [SerializeField] private Vector2 roomSize;
    [SerializeField] private Vector2 doorSize;
    [SerializeField] private Vector2 cellSize;

    [SerializeField] private ObjectRoomSO[] ObjectRoom;
    [SerializeField] private int totalObjects;

    [SerializeField] private int wallSize;

    private Cell[,] grid;

    public void CreateGrid()
    {
        grid = new Cell[(int)roomSize.x, (int)roomSize.y];
        Vector2 centerPosition = new Vector2(gameObject.transform.position.x - roomSize.x / 2 + 0.5f,
            gameObject.transform.position.z - roomSize.y / 2 + 0.5f);

        for (int x = 0; x < roomSize.x; x++)
        {
            for (int y = 0; y < roomSize.y; y++)
            {
                Vector3 cellPosition =
                    new Vector3(centerPosition.x + x * cellSize.x, 0, centerPosition.y + y * cellSize.y);
                CellTag cellTag = DetermineCellTag(x, y);
                Cell cell = new Cell(cellPosition, cellTag);
                grid[x, y] = cell;
            }
        }
    }

    public void SetDoorState(RoomDirection direction)
    {
        int midRoomX = Mathf.FloorToInt(roomSize.x / 2);
        int midRoomY = Mathf.FloorToInt(roomSize.y / 2);

        int midDoorX = Mathf.FloorToInt(doorSize.x / 2);

        Vector2 startPosition;

        switch (direction)
        {
            case RoomDirection.UP:
                startPosition = new Vector2(midRoomX - midDoorX, (int)roomSize.y - 1);

                for (int x = 0; x < doorSize.x; x++)
                {
                    for (int y = 0; y < doorSize.y; y++)
                    {
                        grid[(int)startPosition.x + x, (int)startPosition.y - y].zone = CellTag.occupied;
                    }
                }

                break;

            case RoomDirection.LEFT:
                //grid[0, midRoomY].zone = CellTag.occupied;
                startPosition = new Vector2(0, midRoomY - midDoorX);

                for (int y = 0; y < doorSize.x; y++)
                {
                    for (int x = 0; x < doorSize.y; x++)
                    {
                        grid[(int)startPosition.x + x, (int)startPosition.y + y].zone = CellTag.occupied;
                    }
                }

                break;

            case RoomDirection.DOWN:
                //grid[midRoomX, 0].zone = CellTag.occupied;
                startPosition = new Vector2(midRoomX - midDoorX, 0);

                for (int x = 0; x < doorSize.x; x++)
                {
                    for (int y = 0; y < doorSize.y; y++)
                    {
                        grid[(int)startPosition.x + x, (int)startPosition.y + y].zone = CellTag.occupied;
                    }
                }

                break;

            case RoomDirection.RIGHT:
                //grid[(int)roomSize.x-1, midRoomY].zone = CellTag.occupied;
                startPosition = new Vector2((int)roomSize.x-1, midRoomY - midDoorX);

                for (int y = 0; y < doorSize.x; y++)
                {
                    for (int x = 0; x < doorSize.y; x++)
                    {
                        grid[(int)startPosition.x - x, (int)startPosition.y + y].zone = CellTag.occupied;
                    }
                }
                break;
        }
    }

    private CellTag DetermineCellTag(int x, int y)
    {
        if (x < wallSize || y < wallSize || x >= roomSize.x - wallSize || y >= roomSize.y - wallSize)
        {
            return CellTag.walls;
        }
        else
        {
            return CellTag.inside;
        }
    }

    public void CreateObjects()
    {
        for (int i = 0; i < totalObjects; i++)
        {
            ObjectRoomSO roomObject = ObjectRoom[UnityEngine.Random.Range(0, ObjectRoom.Length)];

            int attempts = 10;
            bool placed = false;

            while (attempts > 0 && !placed)
            {
                Cell startCell = GetRandomCell(roomObject.zone);
                if (startCell != null)
                {
                    List<Cell> occupiedCells = GetCellsInArea(startCell, roomObject.Area);

                    if (occupiedCells != null && occupiedCells.TrueForAll(cell => cell.zone == roomObject.zone))
                    {
                        foreach (var cell in occupiedCells)
                        {
                            cell.zone = CellTag.occupied;
                        }

                        Vector3 position = startCell.position;
                        position.y = 0 + roomObject.prefabObject.transform.localScale.y / 2;
                        Instantiate(roomObject.prefabObject, position, Quaternion.identity, transform);
                        placed = true;
                    }
                }

                attempts--;
            }

            if (!placed)
            {
                Debug.LogWarning($"Failed to place object {roomObject.name}");
            }
        }
    }

    private Cell GetRandomCell(CellTag tag)
    {
        List<Cell> taggedCells = new List<Cell>();
        for (int x = 0; x < roomSize.x; x++)
        {
            for (int y = 0; y < roomSize.y; y++)
            {
                if (grid[x, y].zone == tag)
                {
                    taggedCells.Add(grid[x, y]);
                }
            }
        }

        if (taggedCells.Count > 0)
        {
            return taggedCells[UnityEngine.Random.Range(0, taggedCells.Count)];
        }

        return null;
    }

    private List<Cell> GetCellsInArea(Cell startCell, Vector2 area)
    {
        List<Cell> cellsInArea = new List<Cell>();

        int startX = (int)((startCell.position.x - transform.position.x + roomSize.x / 2 * cellSize.x) / cellSize.x);
        int startY = (int)((startCell.position.z - transform.position.z + roomSize.y / 2 * cellSize.y) / cellSize.y);

        int halfAreaX = Mathf.FloorToInt(area.x / 2);
        int halfAreaY = Mathf.FloorToInt(area.y / 2);

        for (int x = startX - halfAreaX; x <= startX + halfAreaX; x++)
        {
            for (int y = startY - halfAreaY; y <= startY + halfAreaY; y++)
            {
                if (x >= 0 && x < roomSize.x && y >= 0 && y < roomSize.y)
                {
                    cellsInArea.Add(grid[x, y]);
                }
                else
                {
                    return null; // Fuera de los límites
                }
            }
        }

        return cellsInArea;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ProceduralRoomGeneration))]
    public class ProceduralRoomGenerationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ProceduralRoomGeneration script = (ProceduralRoomGeneration)target;
            if (GUILayout.Button("Create Grid"))
            {
                script.CreateGrid();
            }
        }
    }
#endif

    private void OnDrawGizmos()
    {
        if (grid != null && showGrid)
        {
            for (int x = 0; x < roomSize.x; x++)
            {
                for (int y = 0; y < roomSize.y; y++)
                {
                    var cell = grid[x, y];
                    switch (cell.zone)
                    {
                        case CellTag.none:
                            Gizmos.color = Color.gray;
                            break;
                        case CellTag.inside:
                            Gizmos.color = Color.green;
                            break;
                        case CellTag.walls:
                            Gizmos.color = Color.blue;
                            break;
                        case CellTag.occupied:
                            Gizmos.color = Color.red;
                            break;
                        default:
                            break;
                    }

                    Gizmos.DrawWireCube(cell.position, new Vector3(cellSize.x, 0.1f, cellSize.y));
                }
            }
        }
    }
}

[Serializable]
public class Cell
{
    public Vector3 position;
    public CellTag zone;

    public Cell(Vector3 position, CellTag zone)
    {
        this.position = position;
        this.zone = zone;
    }
}

[Serializable]
public enum CellTag
{
    none = 0,
    inside,
    walls,
    occupied,
}