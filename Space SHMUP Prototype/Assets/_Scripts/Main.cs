using UnityEngine;
using System.Collections;
using System.Collections.Generic;  

public class Main : MonoBehaviour {

	static public Main S;
	static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;

	public GameObject[] prefabEnemies;
	public float enemySpawnPerSecond = 0.5f;  //# enemies/second
	public float enemySpawnPadding = 1.5f;  //Padding for position
	public WeaponDefinition[] weaponDefinitions;
	public GameObject prefabPowerUp;
	public WeaponType[] powerUpFrequency = new WeaponType[] {
											WeaponType.blaster, WeaponType.blaster,
											WeaponType.spread, 
											WeaponType.shield
											};

	public bool _______________;

	public WeaponType[] activeWeaponTypes;
	public float enemySpawnRate;  //Delay between enemy spawns

	void Awake()
	{
		S = this;
		//Set Utils.camBounds
		Utils.SetCameraBounds (this.camera);
		//0.5 enemies per second = enemySpawnRate of 2
		enemySpawnRate = 1f / enemySpawnPerSecond;
		//Invoke call SpawnEnemy() once after a 2 second delay
		Invoke ("SpawnEnemy", enemySpawnRate);
		//Debug.Log ("test1");

		//A generic Dictionary with WeaponType as the key
		W_DEFS = new Dictionary<WeaponType, WeaponDefinition> ();
		foreach(WeaponDefinition def in weaponDefinitions)
		{
			W_DEFS[def.type] = def;
		}
	}

	public void ShipDestroyed(Enemy e)
	{
		//Potentially generate a PowerUp
		if (Random.value <= e.powerUpDropChance)
		{
			//Random.value generates a value between 0 & 1 (though never == 1)
			//If the e.powerUpDropChance is 0.50f, a PowerUp will be generated 50% of the time.  
			//For testing, it's now set to 1f.

			//Choose which PowerUp to pick
			//Pick one from the possabilities in powerUpFrequency.
			int ndx = Random.Range(0, powerUpFrequency.Length);
			WeaponType puType = powerUpFrequency[ndx];

			//Spawn a PowerUp
			GameObject go = Instantiate(prefabPowerUp) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp>();
			//Set it to the proper WeaponType
			pu.SetType(puType);

			//Set it to the position of the destroyed ship
			pu.transform.position = e.transform.position;
		}
	}

	static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
	{
		//Check to make sure the key exists in the dictionary
		//Attempting to retrieve a key that didn't exist, would throw an error, so the following statement is important
		if (W_DEFS.ContainsKey(wt))
		{
			return(W_DEFS[wt]);
		}
		//This will return a definition for WeaponType.none which means it has failed to find the WeaponDefinition
		return(new WeaponDefinition ());
	}

	public void SpawnEnemy()
	{
		//Pick a random enemy prefab to instantiate
		int ndx = Random.Range (0, prefabEnemies.Length);
		GameObject go = Instantiate(prefabEnemies[ndx]) as GameObject;
		//Position the enemy above the screen with a random X position
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x+enemySpawnPadding;
		float xMax = Utils.camBounds.max.x-enemySpawnPadding;
		pos.x = Random.Range(xMin, xMax);
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		//Call SpawnEnemy() again in a couple of seconds
		Invoke("SpawnEnemy", enemySpawnRate);
	}

	public void DelayedRestart(float delay)
	{
		//Invoke the Restart() method in delay seconds
		Invoke ("Restart", delay);
	}

	public void Restart() 
	{
		//Reload _scene_0 to restart the game
		Application.LoadLevel("_Scene_0");
	}

	// Use this for initialization
	void Start () {
		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for (int i=0; i<weaponDefinitions.Length; i++)
		{
			activeWeaponTypes[i] = weaponDefinitions[i].type;
		}
	}

	//last paste
	// Update is called once per frame
	void Update () {
	
	}
}
