using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

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

	//public readonly interface for the coolDownTimer
	public float CoolDownTimer
	{
		get
		{
			return coolDownTimer;
		}
	}

	[Header("Ammo")]
	public int startingAmmoAmount = 0;
    public int ammoCapacity;
	public int ammoPerPack = 1;
	public int ammoUsePerActivation = 1;
	public string ammoPackTag = "AmmoPack";
	public bool collectPacksWhileAtMaxAmmo = true;
	private int ammoCount;

	private Text ammoCounter;

	//public readonly interface for ammoCount
	public int AmmoCount
	{
		get
		{
			return ammoCount;
		}
	}

	[Header("Debug")]
	public KeyCode addAmmoKey = KeyCode.U;

	public virtual void Start ()
	{
        if(!isLocalPlayer) { return; }

		ammoCount = startingAmmoAmount;

		ammoCounter = GameObject.Find("HeavyAmmoCounter").GetComponent<Text>();
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
                if (ammoCount >= ammoUsePerActivation)
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
                        WeaponActivatedOnCooldown();
                    }
                }
                //if not enough ammo to use weapon, report activation while no ammo
                else
                {
                    WeaponActivatedNotEnoughAmmo();
                }
            }

            if (Input.GetKeyDown(addAmmoKey))
            {
                AddAmmo(1);
            }

			ammoCounter.text = ammoCount.ToString() + "/" + ammoCapacity.ToString();
        }
	}

	#region CollisionFunctions

	public virtual void OnCollisionEnter(Collision other)
    {
		//if collides with an ammo pack, destroy it and gain its ammo
		if (other.gameObject.tag.Equals(ammoPackTag))
		{
			//if not at max ammo capacity, collect ammo
			//if at max ammo capacity, but still collect while at max, collect ammo
			if (ammoCount < ammoCapacity || collectPacksWhileAtMaxAmmo)
			{
				Destroy(other.gameObject);

				AddAmmo(ammoPerPack);

				CollectedAmmoPack();
			}        
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

	protected virtual void CollectedAmmoPack()
	{
    }

	protected virtual void WeaponActivatedNotEnoughAmmo()
	{
	}

	protected virtual void WeaponActivatedOnCooldown()
	{
	}

	#endregion

	#region InterfaceFunctions

	/// <summary>
	/// Adds to the ammo count the given amount of ammo, capping at the ammo capacity.
	/// If this causes the ammo capacity to be reached, call AmmoMaxedOut() to report it.
	/// </summary>
	/// <param name="amount"></param> The amount of ammo to add to the count.
	/// <param name="ignoreCap"></param> Whether or not to ignore the ammo count when giving the ammo. Optional parameter.
	public void AddAmmo(int amount, bool ignoreCap = false)
	{
		ammoCount += amount;

		//if now at ammo max capacity, cap ammo and report maxing out
		//don't do so if ignoring the cap
		if (ammoCount >= ammoCapacity && !ignoreCap)
		{
			ammoCount = ammoCapacity;

			AmmoMaxedOut();
		}
	}

    //Called by client, runs on server.
    //Spawns heavy weapon on server, then on all clients.
    [Command]
    private void CmdFire(Vector3 spawnPosition, Vector3 spawnVelocity, NetworkInstanceId shooterID)
    {
        //Spawn object on server
        GameObject instantiatedProjectile = GameObject.Instantiate(weaponPrefab);

        // Set position, velocity
        instantiatedProjectile.transform.position = spawnPosition;
        instantiatedProjectile.GetComponent<Rigidbody>().velocity = spawnVelocity;

		InteractiveObject interactiveObject = instantiatedProjectile.GetComponent<InteractiveObject>();
        if (interactiveObject != null)
			interactiveObject.owner = shooterID;

        //Spawn the object across all clients
        NetworkServer.Spawn(instantiatedProjectile);
    }

    #endregion
}
