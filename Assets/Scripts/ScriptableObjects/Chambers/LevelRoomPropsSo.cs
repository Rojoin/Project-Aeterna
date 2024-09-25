using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelRoomPropsSo
{
    public LevelRoomPropsSo(ChamberSO levelRoom, List<Props> PropsList)
    {
        this.levelRoom = levelRoom;
        this.PropsList = PropsList;
    }
    
    public ChamberSO levelRoom;
    public List<Props> PropsList = new List<Props>();
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