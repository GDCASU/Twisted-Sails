using UnityEngine;
using System.Collections;

/* MOTION LOGGER
 * Simple script to keep track of the horizontal velocity of a rigidbody.
 * 
 * Author: S. Alex Bradt
 * Date created: 2016-11-15
 * Date modified: 2016-11-15
 */

public class MotionLogger : MonoBehaviour
{
    public float m_TimeBetweenLogs;
    private Rigidbody m_Body;
    private float m_LastLog;

    void Start()
    {
        m_Body = GetComponent<Rigidbody>();
        m_LastLog = Time.time;
	}
	
	void Update()
    {
        if (Time.time - m_LastLog > m_TimeBetweenLogs)
        {
            Vector3 velocity = m_Body.velocity;
            velocity.y = 0.0f;
            Debug.Log("Velocity: " + velocity.magnitude);
            m_LastLog = Time.time;
        }
	}
}
