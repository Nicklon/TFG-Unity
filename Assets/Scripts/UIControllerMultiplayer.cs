using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIControllerMultiplayer : MonoBehaviour {
    
	List<int> score;
    [SerializeField] Text[] scoreTextList;
    [SerializeField] GameObject[] panelList;

    // Use this for initialization
    void Start()
    {
        score = new List<int>();

        foreach (var scoreText in scoreTextList)
        {
            scoreText.text = "Points : " + score.ToString();
        }
    }

	public void ScoreHit(int[] input)
    {
        int playerIndex = input[0];
        int scorePerHit = input[1];

        score[playerIndex] = score[playerIndex] + scorePerHit;
        scoreTextList[playerIndex].text = "Points : " + score[playerIndex].ToString();
	}

    public void SetColorPanel(object[] input)
    {
        int playerIndex = (int) input[0];
        Color color = (Color)input[1];

        panelList[playerIndex].GetComponent<Image>().color = color;
        
        score.Add(0);
    }

    public void HidePlayerPanel(int playerIndex)
    {
        panelList[playerIndex].SetActive(false);
    }
    
}
