using UnityEngine;
using System.Collections;

// Developer:       Kyle Aycock
// Date:            10/7/2016
// Description:     This script, when controlling the camera, will cause the camera to
//                  orbit around 'target' at the specified radius, height, and speed.
//                  Because the intended usage is to be enabled and disabled when needed,
//                  the orbital camera uses interpolation and trigonometry to take over
//                  where the other controller left off and cause the camera to smoothly
//                  move into orbit.

// Developer:   Kyle Aycock
// Date:        3/24/2017
// Description: Changed camera functionality so it stays above water at all times

public class OrbitalCamera : MonoBehaviour
{

    public GameObject target;
    public float rotationSpeed;
    public float updateSpeed;
    public float orbitRadius;
    public float orbitHeight;

    private float lerpProgress; //used to smoothly transition from current cam position to target
    private float startAngle; //camera will start rotating from its current angle rather than 0
    private float startYPos;
    private Vector3 targetInitialPos;

    void OnEnable()
    {
        // NK 10/20 added target check to avoid null exceptions
        if (target)
        {
            lerpProgress = 0f;
            Vector3 relativePos = transform.position - target.transform.position;
            startAngle = Mathf.Atan2(relativePos.z, relativePos.x);
            startYPos = transform.position.y;
            targetInitialPos = target.transform.position;
        }
    }

    void Update()
    {
        // NK 10/20 added target check to avoid null exceptions
        if (target)
        {
            Vector3 targetPos = target.transform.position + new Vector3(Mathf.Cos(lerpProgress * rotationSpeed + startAngle) * orbitRadius, orbitHeight, Mathf.Sin(lerpProgress * rotationSpeed + startAngle) * orbitRadius);
            targetPos.y = startYPos;
            transform.position = Vector3.Slerp(transform.position, targetPos, lerpProgress);
            transform.LookAt(targetInitialPos);
            lerpProgress += updateSpeed * Time.deltaTime;
        }
    }
}
