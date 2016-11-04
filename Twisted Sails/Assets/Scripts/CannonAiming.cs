// The CannonAiming class causes the cannon it is attached to to automatically adjust 
// its firing angle in order to hit whatever the camera is pointing at.
//
// Edward Borroughs
// Version 10/13/2016

using UnityEngine;
using System.Collections;

public class CannonAiming : MonoBehaviour {

    private RaycastHit hit;
    private float horizontalDistance;
    private float verticalOffset;
    private float firingAngle;
    private float gravity;
    private float initialLocalRotation;
    public Camera shipCamera;
    public int projectileSpeed;
    public float scaleFactor;

    void Start()
    {
        // Initializes gravity and which direction the cannon is facing
        gravity = Mathf.Abs(scaleFactor * -9.81f);
        initialLocalRotation = this.transform.localRotation.z;
    }

    void Update()
    {
        if (shipCamera != null)
        {
            // This draws a ray with a length equal to the maximum firing distance of the canon
            // from the camera in the direction it is facing and gets the position where the ray
            // intersects with any collider.
            Physics.Raycast(shipCamera.transform.position, shipCamera.transform.forward, out hit, Mathf.Pow(projectileSpeed, 2) / gravity);

            // This gets the horizontal distance from the cannon to the point found by the raycast
            horizontalDistance = Mathf.Sqrt(Mathf.Pow(this.transform.position.x - hit.point.x, 2) + Mathf.Pow(this.transform.position.z - hit.point.z, 2));

            // This gets the vertical difference in height between the cannon and the point found
            // by the raycast.
            verticalOffset = hit.point.y - this.transform.position.y;

            // This calculates the angle the cannon needs to be at in order to hit the point found
            // by the raycast based on the projectileSpeed. The angle is in degrees and the smaller
            // of the two angles is used for a flatter cannonball trajectory and shorter travel 
            // time. It is possible for an imaginary number to be returned if the cannon can not
            // hit the point that was found. The formula used can be found at 
            // https://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Angle_.7F.27.22.60UNIQ--postMath-00000010-QINU.60.22.27.7F_required_to_hit_coordinate_.28x.2Cy.29
            firingAngle = Mathf.Rad2Deg * Mathf.Min(
                Mathf.Atan((Mathf.Pow(projectileSpeed, 2) + Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - gravity * (gravity * Mathf.Pow(horizontalDistance, 2) + 2 * verticalOffset * Mathf.Pow(projectileSpeed, 2)))) / (gravity * horizontalDistance)),
                Mathf.Atan((Mathf.Pow(projectileSpeed, 2) - Mathf.Sqrt(Mathf.Pow(projectileSpeed, 4) - gravity * (gravity * Mathf.Pow(horizontalDistance, 2) + 2 * verticalOffset * Mathf.Pow(projectileSpeed, 2)))) / (gravity * horizontalDistance)));

            // Makes sure the cannon only rotates if a real number firingAngle was found.
            if (!float.IsNaN(firingAngle))
            {
                // Ensures the cannon does not point down
                if (firingAngle < 0f)
                {
                    firingAngle = 0;
                }

                // If the cannon was initialized with a positive z-rotation then its rotation is 
                // set to 90 - firingAngle. Otherwise its rotation is set to -90 + firingAngle.
                if (initialLocalRotation > 0f)
                {
                    this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90f - firingAngle));
                }
                else
                {
                    this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90f + firingAngle));
                }
            }
        }
    }
}
