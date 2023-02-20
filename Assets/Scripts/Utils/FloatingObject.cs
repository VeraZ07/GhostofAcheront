using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class FloatingObject : MonoBehaviour
    {
        [SerializeField]
        float minSpeed = 1f;

        [SerializeField]
        float maxSpeed = 2f;

        [SerializeField]
        [Range(0,10)]
        float smooth = 1f;

        //[SerializeField]
        Vector3 center = Vector3.zero;

        [SerializeField]
        Vector3 boxSize = Vector3.one;
        
        
        Vector3 targetPosition;
        float speed = 0;
        Vector3 velocity;

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            center = transform.position;
            targetPosition = GetNewTargetPosition();
            speed = Random.Range(minSpeed, maxSpeed);
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 moveVector = ComputeMoveVector();
            
            if(Vector3.Distance(targetPosition, transform.position) < moveVector.magnitude)
            {
                targetPosition = GetNewTargetPosition();
                speed = Random.Range(minSpeed, maxSpeed);
                moveVector = ComputeMoveVector();
            }

            transform.position += moveVector;
        }

        Vector3 ComputeMoveVector()
        {
            Vector3 targetVelocity = (targetPosition - transform.position).normalized * speed;

            velocity = Vector3.MoveTowards(velocity, targetVelocity, Time.deltaTime * smooth);

            return velocity * Time.deltaTime;
            
        }

        Vector3 GetNewTargetPosition()
        {
            
            Vector3 s = boxSize * .5f;
            float x = Random.Range(-s.x, s.x);
            float y = Random.Range(-s.y, s.y);
            float z = Random.Range(-s.z, s.z);
            return center + new Vector3(x,y,z);
        }
    }

}
