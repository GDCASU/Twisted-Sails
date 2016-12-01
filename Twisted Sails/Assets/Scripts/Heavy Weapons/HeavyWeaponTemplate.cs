using UnityEngine;
using System.Collections;

/**
	[Template Class Header]
	Template Makers:	Nizar Kury and Kyle Chapman
	Date:				11/30/2016
	If need help using this template, contact the programmer's above.
**/

public class HeavyWeaponTemplate : HeavyWeapon {

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
        // ADD YOUR CODE HERE
	}

    /// <summary>
    /// This function is called when the player has 0 ammo left after activating their weapon.
    /// </summary>
    override protected void AmmoDepleted()
	{
        // ADD YOUR CODE
	}

    /// <summary>
    /// This function is called when the player collects ammo, but the ammo capacity has already
    /// been reached.
    /// </summary>
    override protected void AmmoMaxedOut()
	{
        // ADD YOUR CODE
	}

    /// <summary>
    /// This function is called when the player collects ammo.
    /// </summary>
    /// <param name="amountCollected"> Determine the amount of ammo collected by a single ammo pack
    override protected void CollectedAmmo(int amountCollected)
	{
        // ADD YOUR CODE
	}

    /// <summary>
    /// This function is called when the player does not have enough ammo to activate their weapon.
    /// </summary>
    /// <param name="currentAmmo"> Determine the amount of ammo to be set when ammo has been depleted
    override protected void WeaponActivatedNotEnoughAmmo(int currentAmmo)
	{
        // ADD YOUR CODE
	}

    /// <summary>
    /// This function is called when the player activates weapon while it is still on cooldown.
    /// </summary>
    /// <param name="currentCooldown"> The current cooldown the heavy weapon has to go.
    override protected void WeaponActivatedOnCooldown(float currentCooldown)
	{
        // ADD YOUR CODE
	}

	#endregion
}
