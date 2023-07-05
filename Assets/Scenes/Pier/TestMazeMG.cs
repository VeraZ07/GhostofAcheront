using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMazeMG : MonoBehaviour
{
    [SerializeField]
    GameObject floor;

    [SerializeField]
    float maxSpeed = 2f;

    [SerializeField]
    float maxResetSpeed = 1f;

    [SerializeField]
    float maxAngle = 5f;

    float horizontal = 0; 
    float vertical = 0;

    

    float hAngle = 0;
    float vAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();

        Move();
    }

    void Move()
    {
        if(horizontal != 0)
        {
            hAngle += horizontal * maxSpeed * Time.deltaTime;
        }
        else
        {
            hAngle = Mathf.MoveTowards(hAngle, 0f, maxResetSpeed * Time.deltaTime);
        }
        

        if (vertical != 0)
        {
            vAngle += vertical * maxSpeed * Time.deltaTime;
        }
        else
        {
            vAngle = Mathf.MoveTowards(vAngle, 0f, maxResetSpeed * Time.deltaTime);
        }

        hAngle = Mathf.Clamp(hAngle, -maxAngle, maxAngle);
        vAngle = Mathf.Clamp(vAngle, -maxAngle, maxAngle);

        floor.transform.localRotation = Quaternion.Euler(vAngle, 0, hAngle);
    }

    void CheckInput()
    {
        horizontal = 0f;
        vertical = 0f;

        //if(Input.GetKey)    
        if (Input.GetKey(KeyCode.S))
        {
            vertical = -1;
        }
        if (Input.GetKey(KeyCode.W))
        {
            vertical = 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontal = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontal = -1;
        }
    }

    private void FixedUpdate()
    {
        
    }
}
