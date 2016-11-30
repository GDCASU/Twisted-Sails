using UnityEngine;
using System.Collections;

//  Programmer:     Nizar Kury
//  Date:           11/30/2016
//  Description:    Heavy weapon system (ground works)

//  Programmer:     Kyle Chapman
//  Date:           11/30/2016
//  Description:    Heavy weapon system first completion

public class HeavyWeapon : MonoBehaviour
{
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
		ammoCount = startingAmmoAmount;
	}
	
    public virtual void Update ()
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
				//if wepaon is on cooldown, report activation while on cooldown
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

	#endregion
}
