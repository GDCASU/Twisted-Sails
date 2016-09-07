using UnityEngine;
using System.Collections;

public class destroyPowerUp : MonoBehaviour
{

    public float destroyTime = 90.0f;
    public float powerUpRotateSpeed = 120.0f;

    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * powerUpRotateSpeed);
    }
}
