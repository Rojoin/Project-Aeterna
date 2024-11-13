using System;
using Projectile;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects.Entities
{
    [CreateAssetMenu(menuName = "Create ShootingEnemy", fileName = "ShootingEnemy", order = 0)]
    public class ShootingEnemySO : EntitySO
    {
      
        public float timeBetweenAttacks;
        public float timeUntilBlock;
        public float attackRange = 50;
        [Header("Projectile")]
        public Projectile.ProjectileProperties settings;
        public float projectileSpeed;
        public float projectileDamage;
        public float projectileLifeTime;
        public int hitsUntilCounterAttack = 3;
        [FormerlySerializedAs("escapeDistance")] public float maxEscapeDistance = 30;
        public float minEscapeDistance = 10;
        public float timeUntilShieldFullyAppears = 0.5f;


        private void OnValidate()
        {
            if (!settings.HasFlag(ProjectileProperties.ShouldBeDeathOverTime))
            {
                projectileLifeTime = 0;
            }
        }
    }
}