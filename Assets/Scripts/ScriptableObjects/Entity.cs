using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Entity", menuName = "Entity")]
public class Entity : ScriptableObject
{
    public float health;
    public float maxHealth;

    public float damage;
    public float maxDamage;

    public bool isDead;
}
