using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {

	public GameObject enemyPrefab;
	const int LENGTH_OF_ARRAYS = 3;
	GameObject [] arrayOfEnemies = new GameObject[LENGTH_OF_ARRAYS];
	Enemy[] arrayOfScripts = new Enemy[LENGTH_OF_ARRAYS];
	public Transform[] arrayOfTransforms = new Transform[LENGTH_OF_ARRAYS];

	void Start () {
		SpawnCharacters ();
	}

	// Update is called once per frame
	void Update () {
		for (int arrayNumb = 0; arrayNumb < arrayOfScripts.Length; arrayNumb ++){
			if (arrayOfScripts[arrayNumb].dead == true) {
				Debug.Log("Enemy " + arrayNumb + " has died");
				StartCoroutine(MyCoroutine(arrayOfEnemies[arrayNumb], arrayOfScripts[arrayNumb], arrayOfTransforms[arrayNumb]));
				arrayOfScripts[arrayNumb].dead = false;
			}
		}
	}

	IEnumerator MyCoroutine(GameObject enemy, Enemy script, Transform location) {
		yield return new WaitForSeconds (8f);
		//print ("It Works!");
		enemy = Instantiate (enemyPrefab, location.position, new Quaternion (0, 0, 0, 0)) as GameObject;
		script = enemy.GetComponent<Enemy> ();
	}

	void SpawnCharacters(){
		//print ("Void SpawnCharacters");
		for (int arrayNumb = 0; arrayNumb < arrayOfEnemies.Length; arrayNumb++) {
			//print("for loop " + arrayNumb);
			arrayOfEnemies [arrayNumb] = Instantiate (enemyPrefab, arrayOfTransforms [arrayNumb].position, new Quaternion (0, 0, 0, 0)) as GameObject;
			arrayOfScripts [arrayNumb] = arrayOfEnemies [arrayNumb].GetComponent<Enemy> ();
		}
	}
}
