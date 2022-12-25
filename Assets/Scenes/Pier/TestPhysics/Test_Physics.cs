using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Physics : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    Rigidbody rb;
    int ticks = 0;

    float initialSpeed;

    private void Awake()
    {
        rb = target.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //rb.velocity = Vector3.forward * 3f;
        //initialSpeed = rb.velocity.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Debug.LogFormat("[{0}] - Ball.VelZ:{1}", ticks, rb.velocity.y);
        Debug.LogFormat("[{0}] - Expc.VelZ:{1}", ticks, ComputeSpeedByTick(0, Physics.gravity.y, rb.drag, ticks));

        ticks++;
    }

    float ComputeSpeedByTick(float initialSpeed, float acceleration, float drag, int ticks)
    {
        //return initialSpeed + ticks * Time.fixedDeltaTime * acceleration;
        //return initialSpeed * Mathf.Pow(1 - drag * Time.fixedDeltaTime, ticks);

        float speed = initialSpeed;
        for (int i = 0; i < ticks; i++)
        {
            speed += Time.fixedDeltaTime * acceleration;
            speed *= (1 - drag * Time.fixedDeltaTime);
        }

        return speed;
    }



}
