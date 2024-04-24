using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entity", menuName = "Entity")]
public class EntitySO : ScriptableObject
{
    public float health;

    public float damage;

    public bool isDead;
}
