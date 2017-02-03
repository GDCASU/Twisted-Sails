using UnityEngine;
using System.Collections;

/* *
   January 31, 2017: Kyle Chapman
   A static helper class that provides physics calculations for use in other scripts.
*/

public static class PhysicsHelper
{
	/// <summary>
	/// Calculates the in game distance between two game objects.
	/// </summary>
	/// <param name="objectA">The first object.</param>
	/// <param name="objectB">The second object.</param>
	/// <returns></returns>
	public static float DistanceBetweenObjects(GameObject objectA, GameObject objectB)
	{
		return (objectB.transform.position - objectA.transform.position).magnitude;
	}

	/// <summary>
	/// Calculates the horizontal range (distance traveled) of a launched projectile freefalling with gravity.
	/// </summary>
	/// <param name="launchSpeed">The initial speed of the projectile as it is launched.</param>
	/// <param name="altitudeAngle">The upwards angle in degrees of the launch. Zero is horizontal.</param>
	/// <param name="gravity">Positve value for the gravity experienced by the projectile.</param>
	/// <param name="initialHeight">The height in the world from which the object is launched.</param>
	/// <param name="landingHeight">The height in the world on which the object lands.</param>
	/// <returns></returns>
	public static float RangeOfProjectile(float launchSpeed, float altitudeAngle, float gravity = 9.81f, float initialHeight = 0, float landingHeight = 0)
	{
		//make the gravity value positive
		gravity = Mathf.Abs(gravity);

		//calculate initial speeds
		float initialHorizontalSpeed = launchSpeed * Mathf.Cos(altitudeAngle * Mathf.Deg2Rad);
		float initialVerticalSpeed = launchSpeed * Mathf.Sin(altitudeAngle * Mathf.Deg2Rad);

		//calculate time and distance of ascent
		float timeToReachMaxHeight = initialVerticalSpeed / gravity;

		float upwardsHeightGain = initialVerticalSpeed * timeToReachMaxHeight - .5f * gravity * timeToReachMaxHeight * timeToReachMaxHeight;
		float zenithHeight = initialHeight + upwardsHeightGain;

		//calculate distance and time of descent
		float fallingDistance = zenithHeight - landingHeight;
		float timeToFall = Mathf.Sqrt(2 * fallingDistance / gravity);

		float totalAirTime = timeToReachMaxHeight + timeToFall;

		//return the total horizontal distance traveled
		return totalAirTime * initialHorizontalSpeed;
    }
}
