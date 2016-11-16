using UnityEngine;
using System.Collections;

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
            Debug.Log("Velocity: " + m_Body.velocity);
            m_LastLog = Time.time;
        }
	}
}
