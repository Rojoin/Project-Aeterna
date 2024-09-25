using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "AttackFileSO", menuName = "Attacks", order = 0)]
    public class AttackSO : ScriptableObject
    {
        public string animationName;
        public float damage;
        public Vector3 colliderCenter;
        public Vector3 colliderSize;
        public float attackTime;
        public float timeUntilComboEnds;
    }
    //Todo: Make a way for multihit attacks.
    //Maybe make a custom method inside the attackSo
}