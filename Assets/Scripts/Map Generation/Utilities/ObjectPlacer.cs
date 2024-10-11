using System.Collections.Generic;
using UI;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public List<Props> placedGameObjects = new();
    public List<Props> placedEnemies = new();

    public List<(GameObject, GameObject)> placedGameObjectsGameObjects = new();
    public List<(GameObject, GameObject)> placedEnemiesGameObjects = new();

    [SerializeField] private GameObject zoneMark;
    private List<GameObject> marksList = new List<GameObject>();
    private bool activeMarks = true;

    public int PlaceObject(ObjectData objectData, Vector3 position)
    {
        GameObject mark = Instantiate(zoneMark, transform);
        mark.transform.localScale = new Vector3(objectData.Size.x, 1, objectData.Size.y);
        mark.transform.position = new Vector3(position.x, 2.1f, position.z);
        marksList.Add(mark);

        GameObject newObject = Instantiate(objectData.Prefab, transform);
        newObject.transform.position = position;

        UILookAtCamera aux = newObject.transform.GetComponentInChildren<UILookAtCamera>();
        if (aux != null)
            Destroy(aux.gameObject);

        if (objectData.IsEnemy)
        {
            placedEnemiesGameObjects.Add((newObject, mark));
            placedEnemies.Add(new Props(objectData.Prefab, position));
            return placedEnemies.Count - 1;
        }
        else
        {
            placedGameObjectsGameObjects.Add((newObject, mark));
            placedGameObjects.Add(new Props(objectData.Prefab, position));
            return placedGameObjects.Count - 1;
        }
    }

    internal void RemoveObjectAt(int gameObjectIndex, bool isEnemy)
    {
        if (isEnemy)
        {
            Destroy(placedEnemiesGameObjects[gameObjectIndex].Item1);
            Destroy(placedEnemiesGameObjects[gameObjectIndex].Item2);
            placedEnemies[gameObjectIndex] = null;
        }
        else
        {
            Destroy(placedGameObjectsGameObjects[gameObjectIndex].Item1);
            Destroy(placedGameObjectsGameObjects[gameObjectIndex].Item2);
            placedGameObjects[gameObjectIndex] = null;
        }
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