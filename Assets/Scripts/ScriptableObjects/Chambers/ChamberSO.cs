using UnityEngine;

[CreateAssetMenu(fileName = "New Chamber", menuName = "Chamber")]
public class ChamberSO : ScriptableObject
{
    public RoomTypes roomType;
    public RoomForm roomForm;

    public GameObject roomPrefab;

    public int roomWidth;
    public int roomHeight;
}

public enum RoomTypes
{
    START = 0,
    EMPTY,
    ENEMIES,
    TREASURE,
    BOSS,
    INVALID
}

public enum RoomForm
{
    U,
    I,
    L,
    T,
    X
}

public enum RoomDirection
{
    UP = 0,
    RIGHT,
    DOWN,
    LEFT
}