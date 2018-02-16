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
		print ("Getting hit by particles");
		scoreBoard.ScoreHit (points);
		SendMessageUpwards ("ObjectiveDestroyed",gameObject);
		Destroy (gameObject);
	}

	void OnCollisionEnter(Collision collision)
	{
		GameObject origin = collision.gameObject;

		print ("Colliding " + gameObject.name + " with " + origin.name);
		SendMessageUpwards ("ObjectiveDestroyed",gameObject);
		Destroy (gameObject);
	}

	void setPoints(int x)
	{
		points = x;
	}
}
