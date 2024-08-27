using System;
using UnityEngine;
using UnityEngine.Events;

namespace Projectile
{
    [Flags]
    enum ProjectileProperties
    {
        None = 0,
        ShouldBeDeathOverTime = 1,
        ShouldTraverseWithObject = 2,
        ShouldHitEnemies = 4,
        ShouldFollowTarget = 8
    }

    public class BaseProjectile : MonoBehaviour
    {
        public Transform target;
        [SerializeField] private float gravity;
        [SerializeField] private Vector3 acceleration = Vector3.zero;
        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [SerializeField] private float currentTime;
        [SerializeField] private ProjectileProperties properties;
        private Vector3 direction;
        private Vector3 finalPos;

        [SerializeField] private UnityEvent onProyectileHit;
        [SerializeField] private UnityEvent onProyectileDeath;
        [SerializeField] private float damage;

        public void OnEnable()
        {
            currentTime = 0;
            direction = (target.position - transform.position).normalized;
            onProyectileDeath.AddListener(DeathBehaviour);
        }

        public void OnDisable()
        {
            onProyectileDeath.RemoveAllListeners();
        }
        

        private void DeathBehaviour()
        {
            this.enabled = false;
        }

        private void Update()
        {
            if (properties.HasFlag(ProjectileProperties.ShouldFollowTarget))
            {
                direction = (target.position - transform.position).normalized;
            }

            if (properties.HasFlag(ProjectileProperties.ShouldBeDeathOverTime))
            {
                currentTime += Time.deltaTime;
                if (currentTime > lifeTime)
                {
                    onProyectileDeath.Invoke();
                }
            }

            Vector3 velocity = direction;
            velocity *= speed;
            velocity += acceleration != Vector3.zero ? acceleration * Time.deltaTime : velocity;
            transform.position += velocity * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IHealthSystem>(out var IHealthSystem))
            {
                Debug.Log($"Hitted {other.name}");
                if (other.CompareTag("Player"))
                {
                    IHealthSystem.ReceiveDamage(damage);
                    onProyectileDeath.Invoke();
                }
                else if (properties.HasFlag(ProjectileProperties.ShouldHitEnemies))
                {
                    IHealthSystem.ReceiveDamage(damage);
                    onProyectileDeath.Invoke();
                }
            }
            else if (!properties.HasFlag(ProjectileProperties.ShouldTraverseWithObject))
            {
                onProyectileDeath.Invoke();
            }
            
        }
    }
}