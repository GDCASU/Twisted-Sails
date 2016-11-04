using UnityEngine;
using System.Collections;

// Developer:       Kyle Aycock
// Date:            10/7/2016
// Description:     Simple lightweight script to be placed in one-off particle systems
//                  to self-destruct after emitting particles
public class ParticleSystemAutoDestroy : MonoBehaviour
{

    private ParticleSystem ps;

    public void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}
