using System.Collections;
using UnityEngine;

namespace Project_Data.Scripts
{
    public class CoinsEffect : MonoBehaviour {
 
        public float speed = 15;
        public ParticleSystem system;
        public float delayBeforeMoveToTarget = 1;

        private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
        private Transform _target;
        private bool _moveToTarget;

        void Update()
        {
            if (_moveToTarget)
            {
                if (system == null) system = GetComponent<ParticleSystem>();

                var count = system.GetParticles(particles);

                for (int i = 0; i < count; i++)
                {
                    var particle = particles[i];
                    particle.position = Vector3.MoveTowards(particle.position, system.transform.InverseTransformPoint(_target.position), Time.deltaTime * speed);
                    particles[i] = particle;
                }

                system.SetParticles(particles, count);
            }
        }

        public void PlayEffect(Transform t)
        {
            _target = t;
            StartCoroutine(PlayParticleCoroutine());
            Destroy(gameObject, 3);
        }

        IEnumerator PlayParticleCoroutine()
        {
            system.Play();
            yield return new WaitForSeconds(delayBeforeMoveToTarget);
            _moveToTarget = true;
            yield return new WaitForSeconds(0.7f);
            GameManager.Instance.playCoinBounceEffect();
        }
    }
}