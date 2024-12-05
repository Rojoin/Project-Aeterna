using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tutorial Level Room List")]
public class TutorialLevelRoomsSO : ScriptableObject
{
    public List<TutorialRoom> Chambers = new List<TutorialRoom>();
}

[Serializable]
public class TutorialRoom
{
    public ChamberSO levelRoom;
    public List<DummySpawnPosition> DummyPositions = new List<DummySpawnPosition>();
}

[Serializable]
public class DummySpawnPosition
{
    public Vector3 position;
    public Vector3 rotation;
    public float chamberRotation;
}