using System;
using UnityEngine;

public class ProceduralRoomGeneration : MonoBehaviour
{
    [SerializeField] private bool showGrid;
    [SerializeField] private ChamberSO chamberSO;

    private Vector3 playerLastPosition;

    [SerializeField] private Grid unityGrid;
    private GridData gridData;

    private void OnEnable()
    {
        DungeonGeneration.OnProvidePosition += HandlePlayerPosition;
    }

    private void OnDisable()
    {
        DungeonGeneration.OnProvidePosition -= HandlePlayerPosition;
    }

    public void CreateGrid()
    {
        
    }

    public void HandlePlayerPosition(Vector3 position)
    {
        playerLastPosition = position;
    }

    // private void OnDrawGizmos()
    // {
    //     if (showGrid)
    //     {
    //         for (int x = 0; x < chamberSO.roomWidth; x++)
    //         {
    //             for (int y = 0; y < chamberSO.roomHeight; y++)
    //             {
    //                 var cell = customGrid.customGrid[x, y];
    //                 switch (cell.zone)
    //                 {
    //                     case CellTag.none:
    //                         Gizmos.color = Color.gray;
    //                         break;
    //                     case CellTag.inside:
    //                         Gizmos.color = Color.green;
    //                         break;
    //                     case CellTag.walls:
    //                         Gizmos.color = Color.blue;
    //                         break;
    //                     case CellTag.EnemySpawn:
    //                         Gizmos.color = Color.cyan;
    //                         break;
    //                     case CellTag.occupied:
    //                         Gizmos.color = Color.red;
    //                         break;
    //                     default:
    //                         break;
    //                 }
    //    
    //                 Gizmos.DrawWireCube(cell.position, new Vector3(unityGrid.cellSize.x, 0.1f, unityGrid.cellSize.y));
    //             }
    //         }
    //     }
    // }
}