using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals
{
    public class ParticleSystemTrigger : MonoBehaviour
    {
        public ParticleSystem particle;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                particle.Play();
            }

            if (other.CompareTag("Attack"))
            {
                var particleSystem = Instantiate(particle, particle.transform.position, particle.transform.rotation);
                particleSystem.Play();
                StartCoroutine(DestroyParticleAfterPlay(particleSystem));
                Destroy(this.gameObject);
            }
        }
        private IEnumerator DestroyParticleAfterPlay(ParticleSystem particle)
        {
            yield return new WaitUntil(() => !particle.isPlaying);
            Destroy(particle.gameObject);
        }
    }
}