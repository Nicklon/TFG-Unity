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
	[SerializeField] float height = 10;

    [SerializeField] Material specialMaterial;
    [SerializeField] Material normalMaterial;

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
		var objScale = new Vector3 (0.4f, 1f, 0.5f);
        objective.transform.localScale = objScale;
        objective.transform.SetParent(gameObject.transform);
		objective.transform.localPosition = new Vector3 (rightBorder, height, 0);
        var objectiveScript = objective.AddComponent<ObjectiveScript>();

        objectiveScript.setPoints(20);
        objective.AddComponent<Rigidbody> ();

        Renderer renderer = objective.GetComponent<Renderer>();
        renderer.material = normalMaterial;

        Rigidbody objectiveRigidbody = objective.GetComponent<Rigidbody> ();
		objectiveRigidbody.useGravity = false;
		objectiveRigidbody.AddRelativeForce (new Vector3 (-objectiveSpeed, 0f, 0f));
		return objective;
	}

	GameObject createSpecialObjective ()
    {
        GameObject objective = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var objScale = new Vector3(0.4f, 0.5f, 0.5f);
        objective.transform.localScale = objScale;
        objective.transform.SetParent(gameObject.transform);
        objective.transform.localPosition = new Vector3(rightBorder, height-30, 0);
        var objectiveScript = objective.AddComponent<ObjectiveScript>();

        Renderer renderer = objective.GetComponent<Renderer>();
        renderer.material = specialMaterial;

        objectiveScript.setPoints(50);
        objective.AddComponent<Rigidbody>();

        Rigidbody objectiveRigidbody = objective.GetComponent<Rigidbody>();
        objectiveRigidbody.useGravity = false;
        objectiveRigidbody.AddRelativeForce(new Vector3(-objectiveSpeed*1.5f, 0f, 0f));
        return objective;
    }

    //Receives messages from the script assigned to the objective, ObjectiveScript.cs in this case, 
    //when it gets destroyed, by particles or collision with the scene.
	void ObjectiveDestroyed(GameObject origin)
	{
		objectivePool.Remove (origin);
		objectivesActive--;
	}
}
