using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LaserSwordColissionManager : MonoBehaviour {

    int score;
    public Text scoreText;

    // Use this for initialization
    void Start ()
    {
        scoreText.text = "Points : 0";
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "NormalBall")
        {
            score += 25;
            scoreText.text = "Points : " + score.ToString();
        }
        else if(collision.gameObject.tag == "SpecialBall")
        {
            score += 50;
            scoreText.text = "Points : " + score.ToString();
        }
    }
}
