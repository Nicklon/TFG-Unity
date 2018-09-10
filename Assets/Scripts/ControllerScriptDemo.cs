using UnityEngine;
using System.Collections;

public class ControllerScriptDemo : MonoBehaviour {

	public float rotateSpeed;
	public GameObject[] rotateOrder;
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKey(KeyCode.A))
		{
			gameObject.transform.Rotate(0, -rotateSpeed*Time.deltaTime, 0);
			
		}
		if (Input.GetKey(KeyCode.D))
		{
			gameObject.transform.Rotate(0, rotateSpeed*Time.deltaTime, 0);
		}
	}

}
