using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class ParticlesController : MonoBehaviour
    {
        [SerializeField]
        List<ParticleSystem> particles;

        [SerializeField]
        float distance;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            //if (!PlayerController.Local)
            //    return;

            Transform target = Camera.main.transform;

            Vector3 direction = Vector3.ProjectOnPlane(transform.position - target.position, Vector3.up);
            float currentDistance = direction.magnitude;
            //bool blocked = true;

            //if(currentDistance < 3f * distance)
            //{
            //    Vector3 origin = transform.position;
            //    origin.y = 1.8f;
            //    int mask = LayerMask.GetMask("Wall");
            //    blocked = Physics.Raycast(origin, -direction, direction.magnitude, mask);
            //}

            if(currentDistance < distance/* || !blocked*/)
            {
                foreach(ParticleSystem particle in particles)
                {
                    if (!particle.isPlaying)
                        particle.Play();
                }
                
            }
            else
            {
                foreach (ParticleSystem particle in particles)
                {
                    if (particle.isPlaying)
                        particle.Stop();
                }
                
            }
        }
    }

}
