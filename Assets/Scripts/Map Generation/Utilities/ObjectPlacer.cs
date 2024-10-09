using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public List<Props> placedGameObjects = new();
    public List<Props> placedEnemies = new();

    [SerializeField] private GameObject zoneMark;
    private List<GameObject> marksList = new List<GameObject>();
    private bool activeMarks = true;

    public int PlaceObject(ObjectData objectData, Vector3 position)
    {
        GameObject mark = Instantiate(zoneMark, transform);
        mark.transform.localScale = new Vector3(objectData.Size.x, 1, objectData.Size.y);
        mark.transform.position = new Vector3(position.x, 2.1f, position.z);
        marksList.Add(mark);

        GameObject newObject = Instantiate(objectData.Prefab);
        newObject.transform.position = position;

        if (objectData.IsEnemy)
        {
            placedEnemies.Add(new Props(objectData.Prefab, position));
        }
        else
        {
            placedGameObjects.Add(new Props(objectData.Prefab, position));
        }

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

    public void ChangeStateMarks()
    {
        activeMarks = !activeMarks;
        foreach (GameObject m in marksList)
        {
            m.SetActive(!activeMarks);
        }
    }
}