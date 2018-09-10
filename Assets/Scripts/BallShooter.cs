using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallShooter : MonoBehaviour
{
    [SerializeField] Transform spawnPosition;
    [SerializeField] float minTimeSpawn = 1;
    [SerializeField] float maxTimeSpawn = 5;
    [SerializeField] float projectileSpeed = 400;
    [SerializeField] float projectileSpeedDeviation = 200;
    [SerializeField] float initialDelay = 10;
    [SerializeField] Material specialMaterial;
    [SerializeField] Material normalMaterial;

    float spawnTime;
    float timeSinceLastSpawn = 0;
    float delayTimer = 0;
    float minSpeed;
    float maxSpeed;

    // Use this for initialization
    void Start()
    {
        spawnTime = Random.Range(minTimeSpawn, maxTimeSpawn);
        timeSinceLastSpawn = maxTimeSpawn;
        minSpeed = projectileSpeed - projectileSpeedDeviation;
        maxSpeed = projectileSpeed + projectileSpeedDeviation;
    }

    // Update is called once per frame
    void Update()
    {
        delayTimer += Time.deltaTime;

        if(delayTimer > initialDelay)
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnTime)
            {
                CreateObjective();
                timeSinceLastSpawn = 0;
                spawnTime = Random.Range(minTimeSpawn, maxTimeSpawn);
            }
        }
    }

    void CreateObjective()
    {
        int objectiveType = Random.Range(1, 10);

        if (objectiveType > 8)
        {
            CreateObjective(new Vector3(0.5f, 0.5f, 0.5f), maxSpeed, "SpecialBall",specialMaterial);
        }
        else
        {
            CreateObjective(new Vector3(0.75f, 0.75f, 0.75f), Random.Range(minSpeed, maxSpeed) , "NormalBall",normalMaterial);
        }
    }


    void CreateObjective(Vector3 scale, float speed, string tag, Material mat)
    {
        GameObject  objective = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    objective.tag = tag;
                    objective.transform.localScale = scale;
                    objective.transform.SetParent(gameObject.transform);
                    objective.transform.position = spawnPosition.position;

        Renderer renderer = objective.GetComponent<Renderer>();
                 renderer.material = mat;

        Rigidbody   objectiveRigidbody = objective.AddComponent<Rigidbody>();
                    objectiveRigidbody.useGravity = true;
                    objectiveRigidbody.AddForce(spawnPosition.forward * speed);

        DestroyOnCollision destroyScript = objective.AddComponent<DestroyOnCollision>();
                           destroyScript.destroyTags = new List<string>() { "Player", "Background" };
    }
}
