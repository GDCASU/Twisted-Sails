using UnityEngine;
using System.Collections;

public class NewMovement : MonoBehaviour
{

    Rigidbody boat;
    public float acceleration = 1.0f;
    public float topSpeed = 10;
    private float velocity;
    public float rotationalVelocity = 30;

    // Use this for initialization
    void Start()
    {
        boat = this.GetComponent<Rigidbody>();
        velocity = 0;
    }


    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.U)) {
            if (velocity < topSpeed)
                velocity += (acceleration * Time.deltaTime);
            boat.MovePosition(boat.position + (boat.transform.forward * velocity * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.J)) {
            if (velocity < topSpeed)
                velocity += (acceleration * Time.deltaTime);
            boat.MovePosition(boat.position - (boat.transform.forward * velocity * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.K)) {
            boat.transform.Rotate(Vector3.up * rotationalVelocity * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.H)) {
            boat.transform.Rotate(-Vector3.up * rotationalVelocity * Time.deltaTime);
        }
    }
}

