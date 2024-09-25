using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Base chamber List", menuName = "New Base chamber List")]
public class BaseChamberSo : ScriptableObject
{
    public List<ChamberSO> Chambers;
}
