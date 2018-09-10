using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerController : MonoBehaviour {

    public float PlayTime = 60f;
    public Text TimerText;
    public GameObject MenuController;
	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        PlayTime -= Time.deltaTime;
        TimerText.text = "Time Left: " + ((int)PlayTime).ToString();

        if (PlayTime <= 0)
        {
            Time.timeScale = 0;
            MenuController.SendMessage("ShowHideMenu", Time.timeScale != 0);
        }
	}
}
