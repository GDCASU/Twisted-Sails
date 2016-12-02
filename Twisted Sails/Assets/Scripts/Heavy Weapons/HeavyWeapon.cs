using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//  Programmer:     Nizar Kury
//  Date:           11/30/2016
//  Description:    Heavy weapon system (ground works)

//  Programmer:     Kyle Chapman
//  Date:           11/30/2016
//  Description:    Heavy weapon system first completion

//  Programmer      Nizar Kury
//  Date:           12/1/2016
//  Description:    Added networking code to the heavy weapon system

public class HeavyWeapon : NetworkBehaviour
{
    [Header("Weapon")]
    public GameObject weaponPrefab;
    public Vector3 weaponStartingPosition;
    public Vector3 weaponVelocity;

	[Header("Activation")]
	public KeyCode weaponUseKey;

	[Header("Cooldown")]
	public float coolDownTotalSeconds;
	private float coolDownTimer;

	[Header("Ammo")]
	public int startingAmmoAmount = 0;
    public int ammoCapacity;
	public int ammoPerPack = 1;
	public int ammoUsePerActivation = 1;
	public string ammoPackTag = "AmmoPack";
	private int ammoCount;

	[Header("Debug")]
	public KeyCode addAmmoKey = KeyCode.U;

	public virtual void Start ()
	{
        if(!isLocalPlayer) { return; }
		ammoCount = startingAmmoAmount;
	}
	
    public virtual void Update ()
	{
        
        if (isLocalPlayer)
        {
            if (coolDownTimer >= 0)
            {
                coolDownTimer -= Time.deltaTime;
            }

            //the player has pressed the button identified by the weaponUseKey field
            //and the weapon is not on cooldown
            if (Input.GetKeyDown(weaponUseKey))
            {
                //if enough ammo to use weapon, check for cooldown
                if (ammoCount > ammoUsePerActivation)
                {
                    //if weapon not on cooldown, activate it and do cleanup
                    if (coolDownTimer <= 0)
                    {
                        //take ammo away from the player
                        ammoCount -= ammoUsePerActivation;

                        //restart the cooldown timer
                        coolDownTimer = coolDownTotalSeconds;

                        ActivateWeapon();

                        //if ammo is now zero, report the depletion
                        if (ammoCount == 0)
                        {
                            AmmoDepleted();
                        }
                    }
                    //if weapon is on cooldown, report activation while on cooldown
                    else
                    {
                        WeaponActivatedOnCooldown(coolDownTimer);
                    }
                }
                //if not enough ammo to use weapon, report activation while no ammo
                else
                {
                    WeaponActivatedNotEnoughAmmo(ammoCount);
                }
            }

            if (Input.GetKeyDown(addAmmoKey))
            {
                AddAmmo(1);
            }
        }
	}

	#region CollisionFunctions

	public virtual void OnCollisionEnter(Collision other)
    {
		//if collides with an ammo pack, destroy it and gain its ammo
		if (other.gameObject.tag.Equals(ammoPackTag))
		{
			Destroy(other.gameObject);

			AddAmmo(ammoPerPack);

			CollectedAmmo(ammoPerPack);           
        }
    }

    public virtual void OnCollisionExit(Collision other)
    {

    }

    public virtual void OnCollisionStay(Collision other)
    {

    }

	#endregion

	#region TriggerFunctions

	public virtual void OnTriggerEnter(Collider other)
    {

    }

    public virtual void OnTriggerExit(Collider other)
    {

    }

    public virtual void OnTriggerStay(Collider other)
    {

    }

	#endregion

	#region ExtendableFunctions

	protected virtual void ActivateWeapon()
	{
        // networking component for shooting the heavy weapon
        CmdFire(weaponStartingPosition, weaponVelocity, this.GetComponent<NetworkIdentity>().netId);
    }

	protected virtual void AmmoDepleted()
	{
	}

	protected virtual void AmmoMaxedOut()
	{
	}

	protected virtual void CollectedAmmo(int amountCollected)
	{
    }

	protected virtual void WeaponActivatedNotEnoughAmmo(int currentAmmo)
	{
	}

	protected virtual void WeaponActivatedOnCooldown(float currentCooldown)
	{
	}

	#endregion

	#region InterfaceFunctions

	public void AddAmmo(int amount)
	{
		ammoCount += amount;

		//if now at ammo max capacity, cap ammo and report maxing out
		if (ammoCount >= ammoCapacity)
		{
			ammoCount = ammoCapacity;

			AmmoMaxedOut();
		}
	}

    //Called by client, runs on server.
    //Spawns heavy weapon on server, then on all clients.
    // (Borrowed from BoatMovementNetworked)
    [Command]
    private void CmdFire(Vector3 spawnPosition, Vector3 spawnVelocity, NetworkInstanceId shooterID)
    {
        //Spawn object on server
        GameObject _heavyWeapon = GameObject.Instantiate(weaponPrefab);

        // Set position, velocity
        _heavyWeapon.transform.position = spawnPosition;
        _heavyWeapon.GetComponent<Rigidbody>().velocity = spawnVelocity;

        //Ignore collision between cannonball and ship that shot it
        Physics.IgnoreCollision(_heavyWeapon.GetComponent<Collider>(), NetworkServer.FindLocalObject(shooterID).GetComponent<Collider>());

        _heavyWeapon.GetComponent<CannonBallNetworked>().owner = shooterID;

        //Spawn the object across all clients
        NetworkServer.Spawn(_heavyWeapon);
    }

    #endregion
}
