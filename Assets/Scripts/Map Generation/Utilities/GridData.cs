using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    private Dictionary<Vector3Int, PlacementData> PlaceObjectData = new();

    public void AddObject(Vector3Int gridPosition, Vector2Int ObjectSize, int ID, int placedObjectsIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, ObjectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectsIndex);
        foreach (Vector3Int i in positionToOccupy)
        {
            if (PlaceObjectData.ContainsKey(i))
            {
                Debug.Log("Cell Occupied");
            }
            else
            {
                PlaceObjectData[i] = data;
            }
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x,0,y));
            }
        }

        return returnVal;
    }

    public bool CanPlaceObject(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (Vector3Int pos in positionToOccupy)
        {
            if (PlaceObjectData.ContainsKey(pos))
                return false;
        }

        return true;
    }
}


[Serializable]
public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = id;
        PlacedObjectIndex = placedObjectIndex;
    }

}