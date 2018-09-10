using UnityEngine;
using System.Collections;

public class TurnScript : MonoBehaviour {
	  
	[Tooltip("Origin of the rotation.")]
	public GameObject OriginPoint;
	public float RotationSpeed;
	Vector3 initialPosition;
	private double elevate;
	public float triggerHeight;
	// Use this for initialization
	private float initialHeight;

	void Start ()
	{
		var origin = OriginPoint.transform;
		initialHeight = origin.position.y;
		initialPosition = origin.position;
	}

	// Update is called once per frame
	void Update () 
	{
		transform.RotateAround (OriginPoint.transform.position, new Vector3(0,1,(float)elevate) , RotationSpeed * Time.deltaTime);

		//Comprueba si esta subiendo o bajando
		ChangeElevation();
	}

	void ChangeElevation()
	{
		if (transform.position.y > initialHeight + triggerHeight) 
		{
			elevate =  -0.1;
		} 
		else if (transform.position.y > initialHeight + triggerHeight) 
		{
			elevate = 0.1;
		} 
		else 
		{
			if (Random.Range(0, 10) == 5) 
			{
				switch (Random.Range (0, 2)) 
				{
					case 0:
						elevate += 0.1;
						break;
					case 1:
						elevate = 0;
						break;
					case 2:
						elevate += -0.1;
						break;
				}
			}
		}
	}
}
