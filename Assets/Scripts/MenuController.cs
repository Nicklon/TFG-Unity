using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    
    public Button exitBtn;
    public Button iRGameBtn;
    public Button plusGameBtn;


    // Use this for initialization
    void Start ()
    {
        exitBtn.onClick.AddListener(ExitBtn_OnClick);
        iRGameBtn.onClick.AddListener(IRGameBtn_OnClick);
        plusGameBtn.onClick.AddListener(PlusGameBtn_OnClick);
    }
	
	void ExitBtn_OnClick()
    {
        Application.Quit();
    }
    void IRGameBtn_OnClick()
    {
        SceneManager.LoadScene(1);
    }
    void PlusGameBtn_OnClick()
    {
        SceneManager.LoadScene(2);
    }
}
