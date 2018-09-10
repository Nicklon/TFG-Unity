using UnityEngine;
using System.Collections;

public class ObjectiveScript : MonoBehaviour {

	ScoreBoard scoreBoard;
	int points = 0;
	// Use this for initialization
	void Start () 
	{
		scoreBoard = FindObjectOfType<ScoreBoard>();
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	void OnParticleCollision(GameObject particle)
    {
        SendMessageUpwards ("ObjectiveDestroyed",gameObject);
        Destroy (gameObject);
	}

	void OnCollisionEnter(Collision collision)
	{
        if(collision.gameObject.tag == "bullet")
        {
            scoreBoard.ScoreHit(points);
        }

        SendMessageUpwards ("ObjectiveDestroyed",gameObject);
		Destroy (gameObject);
	}

	public void setPoints(int x)
	{
		points = x;
	}
}
