using UnityEngine;
using System.Collections;

/**
	[Template Class Header]
	Template Makers:	Nizar Kury and Kyle Chapman
	Date:				11/30/2016

    Duplicate this script and rename the duplicate
	to place on a boat prefab game object.
    Each boat type should have a different heavy weapon script
    associated with it. For instance, a trireme would have a 
    TriremeHeavyWeapon script while the dragon boat would have
    a DragonBoatHeavyWeapon script attached to it.

	If need help using this template, contact the programmer's above.
**/

public class HeavyWeaponBramble : HeavyWeapon {

    public int numProjectiles;

	// Use this for initialization
	new void Start () {
        base.Start();
        // ADD YOUR CODE HERE
	}
	
	// Update is called once per frame
	new void Update()
    {
        base.Update();
        // ADD YOUR CODE HERE
    }

    #region CollisionFunctions

    /// <summary>
    /// This method is called the moment the object touches/collides with another object (typically once)
    /// </summary>
    /// <param name="other">The game object it collided with
    new void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        // ADD YOUR CODE HERE
    }

    /// <summary>
    /// This method is called the moment the object stops touching/colliding with another object (typically once)
    /// </summary>
    /// <param name="other">The game object it stopped colliding with
    new void OnCollisionExit(Collision other)
    {
        base.OnCollisionExit(other);
        // ADD YOUR CODE HERE
    }

    /// <summary>
    /// This method is called while the object is touching another game object
    /// </summary>
    /// <param name="other">The game object it is colliding with
    new void OnCollisionStay(Collision other)
    {
        base.OnCollisionStay(other);
        // ADD YOUR CODE HERE
    }
    #endregion

    #region TriggerFunctions

    /// <summary>
    /// This method is called the moment the object touches/collides with another object (typically once)
    /// </summary>
    /// <param name="other">The game object it collided with
    new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        // ADD YOUR CODE HERE
    }

    /// <summary>
    /// This method is called the moment the object's trigger stops touching/colliding with another object (typically once)
    /// </summary>
    /// <param name="other">The game object it stopped colliding with
    new void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        // ADD YOUR CODE HERE
    }

    /// <summary>
    /// This method is called while the object's trigger is touching another game object
    /// </summary>
    /// <param name="other">The game object it is colliding with
    new void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
        // ADD YOUR CODE HERE
    }
    #endregion

    #region HeavyWeaponSystemFunctions

    /// <summary>
    /// This function is called when the player presses the activation key (the keyboard input to 
    /// shoot the weapon-- weaponUseKey). It is only called when the weapon is off cooldown and
    /// the player has enough ammo to activate it.
    /// </summary>
    override protected void ActivateWeapon()
	{
		weaponStartingPosition = transform.position + Vector3.up;
        //weaponVelocity = Vector3.back * 1;

        // ADD YOUR CODE HERE
        //  modify spawn position with weaponStartingPosition
        //  modify velocity with weaponVelocity

        for (int i = 0; i < numProjectiles; i++)
        {
            weaponRotation = Quaternion.Euler(0,i*(360/numProjectiles),0);
            base.ActivateWeapon();
        }
	}

    /// <summary>
    /// This function is called when the player has 0 ammo left after activating their weapon.
    /// </summary>
    override protected void AmmoDepleted()
	{
        // ADD YOUR CODE
	}

    /// <summary>
    /// This function is called when the player collects ammo, and the ammo capacity has now
    /// been reached or already was reached.
    /// </summary>
    override protected void AmmoMaxedOut()
	{
        // ADD YOUR CODE
	}

    /// <summary>
    /// This function is called when the player collect (causes to be destroyed) an ammo pack.
    /// </summary>
    override protected void CollectedAmmoPack()
	{
        // ADD YOUR CODE
	}

    /// <summary>
    /// This function is called when the player tries to activate their weapon, but does not have enough ammo.
    /// </summary>
    override protected void WeaponActivatedNotEnoughAmmo()
	{
        // ADD YOUR CODE
	}

    /// <summary>
    /// This function is called when the player tries to activate thier weapon, but it is still on cooldown.
    /// </summary>
    override protected void WeaponActivatedOnCooldown()
	{
        // ADD YOUR CODE
	}

	#endregion
}
