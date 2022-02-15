using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 dir;
    [SerializeField] private int speed;
    [SerializeField] private int turnSpeed;
    [SerializeField] private int gravity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
        Turn();
        Fall();
    }

    void Move()
    {
        if(Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(new Vector3(Vector3.forward.x, 0, Vector3.forward.z) * speed * 10);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(new Vector3(Vector3.forward.x, 0, Vector3.forward.z) * -speed * 10);
        }

        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.x = 0;
        rb.velocity = transform.TransformDirection(localVelocity);
    }

    void Turn()
    {
        if(Input.GetKey(KeyCode.D))
        {
            rb.AddTorque(Vector3.up * turnSpeed * 10);
        }
        else if(Input.GetKey(KeyCode.A))
        {
            rb.AddTorque(-Vector3.up * turnSpeed * 10);
        }
    }

    void Fall()
    {
        rb.AddForce(Vector3.down * gravity);
    }
}
