using UnityEngine;
using System.Collections;

public class Boat_Collision : MonoBehaviour
{


    // Collision happen
    //Hit angle, location, velocity of the boat
    void OnCollisionEnter(Collision collision)
    {
        //factors are used to be combined together to calculate the final damage dealt

        float factorB = 0;
        float factorC = 0;

        //Bigger the velocity is, more damage will there be
        float factorA = calculate_factorA(collision.relativeVelocity);

        //If it ram hitting the boat, there will be more damage
        if (gameObject.tag == "BoatBody")
        {
            if (collision.collider.tag == "BoatBody")
            {
                //boat body hit boat body deal regular damage
                factorB = 50;
            }
            else if (collision.collider.tag == "Ram")
                //deal extra damage
                factorB = 100;
        }
        else if (gameObject.tag == "Ram")
        {
            if (collision.collider.tag == "BoatBody")
            {
                //boat body hit ram deal little damage
                factorB = 0;
            }
            else if (collision.collider.tag == "Ram")
                //ram hits ram deal little damage
                factorB = 10;
        }

        //Hit angle will be updated soon

        damage_done(factorA, factorB, factorC);

    }

    // Collision finished
    void OnCollisionExit(Collision collision)
    {

    }

    // During collision
    void OnCollisionStay(Collision collision)
    {

    }

    float calculate_factorA(Vector3 speed)
    {
        Debug.Log("Their relative velocity is " + speed);
        return 20;
    }

    //calculate the actual damage done
    void damage_done(float factorA, float factorB, float factorC)
    {
        float damage = factorA + factorB + factorC;
        Debug.Log("Damage dealt is " + damage);
    }
}
