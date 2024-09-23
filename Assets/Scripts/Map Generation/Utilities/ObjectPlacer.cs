using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public List<Props> placedGameObjects = new();

    public int PlaceObject(ObjectData objectData, Vector3 position)
    {
        GameObject newObject = Instantiate(objectData.Prefab);
        newObject.transform.position = position;
        placedGameObjects.Add(new Props(objectData.Prefab, position));
        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex 
            || placedGameObjects[gameObjectIndex] == null)
            return;
        Destroy(placedGameObjects[gameObjectIndex].prop);
        placedGameObjects[gameObjectIndex] = null;
    }
}