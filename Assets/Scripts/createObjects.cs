using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class createObjects : MonoBehaviour {

	[SerializeField] float leftBorder = 200;
	[SerializeField] float rightBorder = -200;
	[SerializeField] int spawnLimit = 5;
	[SerializeField] float minTimeSpawn = 1;
	[SerializeField] float maxTimeSpawn = 5;
	[SerializeField] int objectiveSpeed = 5;

	float spawnTime = 0;
	float timeSinceLastSpawn = 5;
	List<GameObject> objectivePool = new List<GameObject>();
	int objectivesCounter = 0;
	int objectivesActive = 0;

	// Use this for initialization
	void Start () 
	{
		spawnTime = Random.Range (minTimeSpawn, maxTimeSpawn);
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeSinceLastSpawn += Time.deltaTime;

		if (timeSinceLastSpawn >= spawnTime) 
		{
			CreateObjective ();
			timeSinceLastSpawn = 0;
			spawnTime = Random.Range (minTimeSpawn, maxTimeSpawn);
		}

	}

	void CreateObjective()
	{
		if (objectivesActive < spawnLimit) 
		{
			int objectiveType = Random.Range (1, 10);
			GameObject objective = null;

			if (objectiveType > 8) 
			{
				objective = createSpecialObjective ();
			}
			else 
			{
				objective = createNormalObjective ();
			}

			objectivePool.Add(objective);

			objectivesCounter++;
			objectivesActive++;
		}
	}

	GameObject createNormalObjective ()
	{
		GameObject objective = GameObject.CreatePrimitive (PrimitiveType.Cube);
		objective.transform.localScale = new Vector3 (50, 100, 20);
		objective.transform.SetParent (gameObject.transform);
		objective.name = "Objective " + objectivesCounter;
		objective.transform.position = gameObject.transform.position + new Vector3 (leftBorder, 60, 0);
		objective.AddComponent<ObjectiveScript> ();
		objective.AddComponent<Rigidbody> ();
		Rigidbody objectiveRigidbody = objective.GetComponent<Rigidbody> ();
		objectiveRigidbody.useGravity = false;
		objectiveRigidbody.AddRelativeForce (new Vector3 (-10000, 0, 0));
		return objective;
	}

	GameObject createSpecialObjective ()
	{
		GameObject objective = GameObject.CreatePrimitive (PrimitiveType.Cube);
		objective.transform.localScale = new Vector3 (50, 60, 20);
		objective.transform.SetParent (gameObject.transform);
		objective.name = "Objective " + objectivesCounter;
		objective.transform.position = gameObject.transform.position + new Vector3 (leftBorder, 60, 0);
		objective.AddComponent<ObjectiveScript> ();
		objective.AddComponent<Rigidbody> ();
		Rigidbody objectiveRigidbody = objective.GetComponent<Rigidbody> ();
		objectiveRigidbody.useGravity = false;
		objectiveRigidbody.AddRelativeForce (new Vector3 (-10000, 0, 0));
		return objective;
	}

	void ObjectiveDestroyed(GameObject origin)
	{
		print ("Removing " + origin.name);
		objectivePool.Remove (origin);
		objectivesActive--;
	}
}
