using System;
using ScriptableObjects.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Projectile
{
    [Flags]
    public enum ProjectileProperties
    {
        None = 0,
        ShouldBeDeathOverTime = 1,
        ShouldTraverseWithObject = 2,
        ShouldHitEnemies = 4,
        ShouldFollowTarget = 8
    }

    public class BaseProjectile : MonoBehaviour
    {
        [SerializeField] public Transform target;
        [SerializeField] private float gravity;
        [SerializeField] private Vector3 acceleration = Vector3.zero;
        [SerializeField] private float speed;
        [SerializeField] private float lifeTime;
        [SerializeField] private float currentTime;
        [SerializeField] private ProjectileProperties properties;
        private Vector3 direction;
        private Vector3 finalPos;
        [SerializeField] float scaleSpeed = 2;
        [SerializeField]  float maxScale = 2;

        [SerializeField] private UnityEvent onProyectileHit;
        [SerializeField] private UnityEvent onProyectileDeath;
        [SerializeField] private float damage;

        public void OnEnable()
        {
            currentTime = 0;
            direction = target != null
                ? (target.position - transform.position).normalized
                : (finalPos - transform.position).normalized;
            onProyectileDeath.AddListener(DeathBehaviour);
            transform.LookAt(target != null ? target.position : finalPos);
        }

        public void OnDisable()
        {
            onProyectileDeath.RemoveAllListeners();
        }


        private void DeathBehaviour()
        {
            gameObject.SetActive(false);
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


                transform.localScale = Vector3.one * Mathf.Clamp(currentTime * scaleSpeed, 1, maxScale);
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

        public void SetSettings(ShootingEnemySO config)
        {
            properties = config.settings;
            speed = config.projectileSpeed;
            damage = config.projectileDamage;
            lifeTime = config.projectileLifeTime;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        public void SetDirection(Vector3 position)
        {
            finalPos = position;
        }
    }
}