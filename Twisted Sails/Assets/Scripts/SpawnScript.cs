using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {

	public GameObject enemyPrefab;
	const int LENGTH_OF_ARRAYS = 3;
	GameObject [] arrayOfEnemies = new GameObject[LENGTH_OF_ARRAYS];
	Health[] arrayOfScripts = new Health[LENGTH_OF_ARRAYS];
	public Transform[] arrayOfTransforms = new Transform[LENGTH_OF_ARRAYS];
	public bool CR_Running;

	void Start () {
		SpawnCharacters ();
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log (arrayOfEnemies [0]);
		for (int arrayNumb = 0; arrayNumb < arrayOfScripts.Length; arrayNumb ++){
			if (arrayOfScripts[arrayNumb].dead == true) {
				Debug.Log("Enemy " + arrayNumb + " has died");
				//Debug.Log (arrayOfScripts[arrayNumb].dead);
				CR_Running = false;
				arrayOfScripts [arrayNumb].dead = false;
				StartCoroutine (MyCoroutine ());
				//StartCoroutine(MyCoroutine(arrayOfEnemies[arrayNumb], arrayOfScripts[arrayNumb], arrayOfTransforms[arrayNumb]));
			}
			if (CR_Running == true) {
				//Debug.Log("Instantiate!");
				arrayOfEnemies [arrayNumb] = Instantiate (enemyPrefab, arrayOfTransforms [arrayNumb].position, new Quaternion (0, 0, 0, 0)) as GameObject;
				arrayOfScripts [arrayNumb] = arrayOfEnemies [arrayNumb].GetComponent<Health> ();
				CR_Running = false;
			}
		}
	}

	IEnumerator MyCoroutine() {
		//Debug.Log ("Coroutine started");
		yield return new WaitForSeconds (8f);
		CR_Running = true;
		//Debug.Log ("Coroutine finished");
		//print ("It Works!");
		//enemy = Instantiate (enemyPrefab, location.position, new Quaternion (0, 0, 0, 0)) as GameObject;
		//script = enemy.GetComponent<Health> ();
	}

	void SpawnCharacters(){
		//print ("Void SpawnCharacters");
		for (int arrayNumb = 0; arrayNumb < arrayOfEnemies.Length; arrayNumb++) {
			//print("for loop " + arrayNumb);
			arrayOfEnemies [arrayNumb] = Instantiate (enemyPrefab, arrayOfTransforms [arrayNumb].position, new Quaternion (0, 0, 0, 0)) as GameObject;
			arrayOfScripts [arrayNumb] = arrayOfEnemies [arrayNumb].GetComponent<Health> ();
		}
	}
}