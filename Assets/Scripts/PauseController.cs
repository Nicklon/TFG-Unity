using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour {

    public Button resetBtn;
    public Button exitBtn;
    public bool exitDesktop;

    // Use this for initialization
    void Start ()
    {
        exitBtn.onClick.AddListener(ExitBtn_OnClick);
        resetBtn.onClick.AddListener(ResetBtn_OnClick);
	}

	void ExitBtn_OnClick()
    {
        //Clean wii remote?
        if (exitDesktop)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
    void ResetBtn_OnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
