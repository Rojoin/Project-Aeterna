using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "AttackFileSO", menuName = "Attacks" , order = 0)]
    public class AttackSO : ScriptableObject
    {
        public AnimatorOverrideController overrideController;
        public float damage;
    }
}