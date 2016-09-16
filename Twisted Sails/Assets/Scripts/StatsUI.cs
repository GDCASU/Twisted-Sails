/*
 WaitTime = 5.0f
 
 SpeedButton(baseSpeed){
 	if(Time > CoolDown)
 		ChangeSpeed(baseSpeed)
		CoolDown = Time + WaitTime
 }
 FireRateButton(baseFireRate){
 	if(Time > CoolDown)
 		ChangeFireRate(baseFireRate)
 		CoolDown = Time + WaitTime
 }
 DurabilityButton(baseDurability){
 	if(Time > CoolDown)
 		ChangeDurability(baseDurability)
 		CoolDown = Time + WaitTime
 }
*/

using UnityEngine;
using System.Collections;

public class StatsUI : MonoBehaviour {

	//Player Stats
	public int speed;
	public int fireRate;
	public int durability;
	public double waitTime = 5.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*if(SpeedButtonPressed){
		 * 	SpeedButton(speed);
		 * }
		*/

		/*if(FireRateButtonPressed){
		* 	FireRateButton(fireRate);
		* }
		*/

		/*if(DurabilityButtonPressed){
		* 	DurabilityButton(durability);
		* }
		*/
	}

	public void SpeedButton(int baseSpeed){
		double coolDown = 0.0f;
		if(Time.deltaTime > coolDown){
			//ChangeSpeed(baseSpeed)
			coolDown = Time.deltaTime + waitTime;
		}
	}

	public void FireRateButton(int baseFireRate){
		double coolDown = 0.0f;
		if(Time.deltaTime > coolDown){
			//ChangeFireRate(baseFireRate)
			coolDown = Time.deltaTime + waitTime;
		}
	}

	public void DurabilityButton(int baseDurability){
		double coolDown = 0.0f;
		if(Time.deltaTime > coolDown){
			//ChangeDurability(baseDurability)
			coolDown = Time.deltaTime + waitTime;
		}
	}

}
