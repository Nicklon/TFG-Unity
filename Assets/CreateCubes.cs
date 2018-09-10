using UnityEngine;

public class CreateCubes : MonoBehaviour {

    [SerializeField] GameObject borderTop;
    [SerializeField] GameObject borderBottom;
    [SerializeField] GameObject borderLeft;
    [SerializeField] GameObject borderRight;
    [SerializeField] float SpawnZ;

    [SerializeField] Material specialMaterial;
    [SerializeField] Material normalMaterial;

    [SerializeField] float minSpawnTime;
    [SerializeField] float maxSpawnTime;
    
    [SerializeField] GameObject parent;

    private float maxX;
    private float minX;
    private float maxY;
    private float minY;
    
    private float timeSinceLastSpawn = 0;
    private float spawnTime = 0;
    // Use this for initialization
    void Start ()
    {
        maxY = borderTop.transform.position.y;
        minY = borderBottom.transform.position.y;
        maxX = borderRight.transform.position.x;
        minX = borderLeft.transform.position.x;

        spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
    }
	
	// Update is called once per frame
	void Update ()
    {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnTime)
            {
                CreateObjective();
                timeSinceLastSpawn = 0;
                spawnTime = Random.Range(minSpawnTime, maxSpawnTime);
        }
        
    }

    void CreateObjective()
    {
        int objectiveType = Random.Range(1, 10);

        if (objectiveType > 8)
        {
            CreateObjective(new Vector3(0.5f, 0.5f, 0.5f), "SpecialBall", specialMaterial, GeneratePosition());
        }
        else
        {
            CreateObjective(new Vector3(0.75f, 0.75f, 0.75f), "NormalBall", normalMaterial, GeneratePosition());
        }
    }

    private Vector3 GeneratePosition()
    {
        return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), SpawnZ);
    }

    void CreateObjective(Vector3 scale, string tag, Material mat, Vector3 position)
    {
        GameObject objective = GameObject.CreatePrimitive(PrimitiveType.Cube);
        objective.tag = tag;
        objective.transform.localScale = scale;
        objective.transform.SetParent(parent.transform);
        objective.transform.position = position;

        Renderer renderer = objective.GetComponent<Renderer>();
        renderer.material = mat;
    }
}
