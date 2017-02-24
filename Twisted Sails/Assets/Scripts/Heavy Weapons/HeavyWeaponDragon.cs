using UnityEngine;
using System.Collections;

/**
	[Heavy Weapon: Dragon]
	Creator:            Sean McGonegle
	Date:				2/24/2017

    The Activate method of this script sets the spawn position of the dragon projectile to the player's ship, with a 
    small offset in front of the ship, and sets the projectile's speed.

    Collision is handled in a seperate script hat is attatched to the projectile itself. This was done because the
    current system does not have a way to access the unique prefab that is created when the projectile is instantiated,
    which makes handeling collision and the destruction of said projectile difficult.

**/

public class HeavyWeaponDragon: HeavyWeapon {

    // Use this for initialization
    private int projectileSpeed = 50;
    private int projectileOffset = 5;

	new void Start () {
        base.Start();
    }
	
	// Update is called once per frame
	new void Update()
    {
        base.Update();
    }

    #region CollisionFunctions

    /// <summary>
    /// This method is called the moment the object touches/collides with another object (typically once)
    /// </summary>
    /// <param name="other">The game object it collided with
    new void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
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
        weaponStartingPosition = transform.position + (transform.forward * projectileOffset);
        weaponVelocity = transform.forward * projectileSpeed;
        weaponPrefab.GetComponent<Rigidbody>().AddForce(weaponVelocity);

        base.ActivateWeapon();
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
        //Displays to player they have no ammo
	}

    /// <summary>
    /// This function is called when the player collect (causes to be destroyed) an ammo pack.
    /// </summary>
    override protected void CollectedAmmoPack()
	{
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
