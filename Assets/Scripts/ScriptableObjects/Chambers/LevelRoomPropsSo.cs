using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelRoomPropsSo
{
    public LevelRoomPropsSo(ChamberSO levelRoom, List<Props> PropsList, List<Props> EnemyList)
    {
        this.levelRoom = levelRoom;
        this.PropsList = PropsList;
        this.EnemyList = EnemyList;
    }

    public ChamberSO levelRoom;
    public List<Props> PropsList = new List<Props>();
    public List<Props> EnemyList = new List<Props>();
}

[Serializable]
public class Props
{
    public Props(GameObject prop, Vector3 propPosition)
    {
        this.prop = prop;
        this.propPosition = propPosition;
    }

    public GameObject prop;
    public Vector3 propPosition;
}