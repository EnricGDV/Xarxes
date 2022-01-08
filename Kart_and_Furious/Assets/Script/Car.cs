using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cartut : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float gravityMultiplier;

    private Rigidbody rb;

    Vector4 inputs;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inputs = new Vector4(0, 0, 0, 0);
        speed = 4;
        turnSpeed = 3;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector4 inputs = getInputs();

        Move(inputs);

        
    }

    Vector4 getInputs()
    {
        Vector4 ret = new Vector4(0,0,0,0);//xyzw - w a s d
        if (Input.GetKey(KeyCode.W))
        {
            ret.x = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            ret.z = 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            ret.y = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            ret.w = 1;
        }

        return ret;
    }

    void Move(Vector4 inputs)
    {
        Accelerate(inputs);
        Turn(inputs);
        Fall();
    }


    void Accelerate(Vector4 inputs)
    {
        if (inputs.x == 1)
        {
            Vector3 forceToAdd = transform.forward;
            forceToAdd.y = 0;
            rb.AddForce(forceToAdd * speed * 10);
        }
        else if (inputs.z == 1)
        {
            Vector3 forceToAdd = -transform.forward;
            forceToAdd.y = 0;
            rb.AddForce(forceToAdd * speed * 10);
        }

        Vector3 locVel = transform.InverseTransformDirection(rb.velocity);
        locVel = new Vector3(0, locVel.y, locVel.z);
        rb.velocity = new Vector3(transform.TransformDirection(locVel).x, rb.velocity.y, transform.TransformDirection(locVel).z);
    }


    void Turn(Vector4 inputs)
    {
        if (inputs.y == 1)
        {
            rb.AddTorque(-Vector3.up * turnSpeed * 10);
        }
        else if (inputs.w == 1)
        {
            rb.AddTorque(Vector3.up * turnSpeed * 10);
        }
    }

    void Fall()
    {
        rb.AddForce(Vector3.down * gravityMultiplier * 10);
    }
}
