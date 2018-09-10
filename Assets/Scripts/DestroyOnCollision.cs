using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyOnCollision : MonoBehaviour {

    public List<string> destroyTags;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        var collisionObject = collision.gameObject;

        if (destroyTags.Contains(collisionObject.tag))
        {
            Destroy(gameObject);
        }
    }
}
