using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using System.Linq;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WiimoteShooterMultiplayer : MonoBehaviour
{
    public GameObject MenuController;
    public bool MainMenu;

    public IRDotList[] ir_dots_list;
    public IRBBList[] ir_bb_list;
    public List<RectTransform> ir_pointer_list;
    public List<Color> playerColors;
    private float firingRate = 0.5f;

    private float firingTimer;
    private List<Wiimote> wiimotes;
    private float ticksSinceHomeBton = 100;

    //To prevent multiple scenes load at the same time
    private float initialDelay = 100;
    private float ticksSinceLoad = 0;
    
    int playersActive;

    void Start()
    {
        wiimotes = new List<Wiimote>();
        
        Time.timeScale = 0;
        firingTimer = 0f;
        playersActive = 0;
    }

    void Update()
    {
        if (ticksSinceLoad < initialDelay)
        {
            ticksSinceLoad++;
        }
        else if (ticksSinceLoad == initialDelay)
        {
            Time.timeScale = 1;
            ticksSinceLoad++;
        }
        else
        {
            if (!WiimoteManager.HasWiimote())
            {
                SearchAndInitialize();
                return;
            }

            var ret = new List<int>();

            if (ret.Count == 0 && playersActive >= 1)
            {
                for (int i = 0; i < playersActive; i++)
                {
                    ret.Add(0);
                }
            }
            
            for (int i = 0; i < playersActive; i++)
            {
                do
                {
                    ret[i] = wiimotes[i].ReadWiimoteData();
                }
                while (ret[i] > 0);

            }
            for (int i = 0; i < playersActive; i++)
            {
                UpdateIRPanel(i, wiimotes[i]);
                UpdateControls(i, wiimotes[i]);
            }

        }

    }
    void SearchAndInitialize()
    {
        WiimoteManager.FindWiimotes();

        if (!WiimoteManager.HasWiimote())
        {
            return;
        }
        else
        {
            playersActive = WiimoteManager.Wiimotes.Count;
            Debug.Log(playersActive);
            for (var i = 0; i < WiimoteManager.Wiimotes.Count; i++)
            {
                wiimotes.Add(WiimoteManager.Wiimotes[i]);
                wiimotes[i].SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);
                wiimotes[i].SetupIRCamera(IRDataType.EXTENDED);
                wiimotes[i].SendPlayerLED(i >= 0, i >= 1, i >= 2, i >= 3);
                
                ir_pointer_list[i].GetComponent<Image>().color = playerColors[i];

                object[] changeColorUI = { i, playerColors[i] };
                SendMessage("SetColorPanel", changeColorUI);
            }

            for(int i = playerColors.Count-1; i >= playersActive; i-- )
            {
                SendMessage("HidePlayerPanel", i);
            }
        }
    }

    private void UpdateIRPanel(int playerIndex, Wiimote wiimote)
    {
        var ir_dots = ir_dots_list[playerIndex].ir_dots;
        var ir_bb = ir_bb_list[playerIndex].ir_bbs;
        var ir_pointer = ir_pointer_list[playerIndex];

        if (ir_dots.Length < 4) return;

        float[,] ir = wiimote.Ir.GetProbableSensorBarIR();
        for (int i = 0; i < 2; i++)
        {
            float x = (float)ir[i, 0] / 1023f;
            float y = (float)ir[i, 1] / 767f;
            if (x == -1 || y == -1)
            {
                ir_dots[i].anchorMin = new Vector2(0, 0);
                ir_dots[i].anchorMax = new Vector2(0, 0);
            }

            ir_dots[i].anchorMin = new Vector2(x, y);
            ir_dots[i].anchorMax = new Vector2(x, y);

            if (ir[i, 2] != -1)
            {
                int index = (int)ir[i, 2];
                float xmin = (float)wiimote.Ir.ir[index, 3] / 127f;
                float ymin = (float)wiimote.Ir.ir[index, 4] / 127f;
                float xmax = (float)wiimote.Ir.ir[index, 5] / 127f;
                float ymax = (float)wiimote.Ir.ir[index, 6] / 127f;
                ir_bb[i].anchorMin = new Vector2(xmin, ymin);
                ir_bb[i].anchorMax = new Vector2(xmax, ymax);
            }
        }

        float[] pointer = wiimote.Ir.GetPointingPosition();
        ir_pointer.anchorMin = new Vector2(pointer[0], pointer[1]);
        ir_pointer.anchorMax = new Vector2(pointer[0], pointer[1]);
    }

    void UpdateControls(int playerIndex, Wiimote wiimote)
    {
        ticksSinceHomeBton++;
        var ir_pointer = ir_pointer_list[playerIndex];


        if (wiimote.Button.home && ticksSinceHomeBton > 100)
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            ticksSinceHomeBton = 0;
            MenuController.SendMessage("ShowHideMenu", Time.timeScale != 0);
        }

        wiimote.RumbleOn = false;

        //Shoot if button A is pressed.
        if (wiimote.Button.a)
        {
            var sphereCollider = Physics.OverlapSphere(ir_pointer.position, 0.3f);

            if (sphereCollider.Count()>0)
            {
                foreach(var collision in sphereCollider)
                {
                    if (Time.timeScale == 0 || MainMenu)
                    {
                        if (collision.tag == "Button")
                        {
                            Button button = collision.gameObject.GetComponent<Button>();
                            if (button != null) button.onClick.Invoke();
                        }
                    }
                    else
                    {
                            if (collision.tag == "NormalBall")
                            {
                                int[] pointsMsg = { playerIndex , 5 };
                                SendMessage("ScoreHit", pointsMsg);
                                Destroy(collision.gameObject);
                            }
                            else if (collision.tag == "SpecialBall")
                            {
                                int[] pointsMsg = { playerIndex, 20 };
                                SendMessage("ScoreHit", pointsMsg);
                                Destroy(collision.gameObject);
                            }
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        OnDestroy();
    }

    void OnDestroy()
    {
        foreach (var wiimote in wiimotes)
        {
            if (wiimote != null)
            {
                WiimoteManager.Cleanup(wiimote);
            }
        }
        
        wiimotes.Clear();
    }
   
    [System.Serializable]
    public class WiimoteModel
    {
        public Transform rot;
        public Renderer a;
        public Renderer b;
        public Renderer one;
        public Renderer two;
        public Renderer d_up;
        public Renderer d_down;
        public Renderer d_left;
        public Renderer d_right;
        public Renderer plus;
        public Renderer minus;
        public Renderer home;
    }

    [System.Serializable]
    public class IRBBList
    {
        public RectTransform[] ir_bbs;
    }

    [System.Serializable]
    public class IRDotList
    {
        public RectTransform[] ir_dots;
    }
}