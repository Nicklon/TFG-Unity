using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

	int score;
    [SerializeField] Text scoreText;

	// Use this for initialization
	void Start()
	{
		scoreText.text = "Points : " + score.ToString();
    }

	public void ScoreHit(int scorePerHit)
	{
		score = score + scorePerHit;
		scoreText.text = "Points : " + score.ToString();
	}

}
