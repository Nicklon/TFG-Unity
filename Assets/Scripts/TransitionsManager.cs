using UnityEngine;
using System.Collections;

public class TransitionsManager : MonoBehaviour {

    public Canvas Menu;

	// Use this for initialization
	void Start ()
    {
        Menu.enabled = false;
	}
	
    void ShowHideMenu(bool menu)
    {
        if (menu)
        {
            Menu.enabled = false;
        }
        else
        {
            Menu.enabled = true;
        }
    }
}
