using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class WiimoteMotionPlus : MonoBehaviour
{
    private float preventDoublePause = 0.25f;
    private float timeSinceHomeClicked = 0f;
    public GameObject MenuController;
    public GameObject PlusSetupImage;
    public GameObject LaserSwordEffects;
    public GameObject LaserSwordBladeGlow;
    public Text ScoreText;

    public WiimoteModel model;
    public RectTransform[] ir_dots;
    public RectTransform[] ir_bb;
    public RectTransform ir_pointer;
    public GameObject ir_origin;

    private Quaternion initial_rotation;
    private float firingTimer;
    private Wiimote wiimote;

    private Vector3 wmpOffset = Vector3.zero;

    private Vector3 wmpDeviation = Vector3.zero;
    private Vector3 accelDeviaton = Vector3.zero;

    private bool motionPlus = false;
    private bool plusSetup = false;
    private float plusSetupStill = 0;
    private Vector3 plusSetupVector = Vector3.zero;
    private float pauseButtonClick = 100;
    private float firingRate = 0.5f;
    public Image Dot5;

    void Start()
    {
        firingTimer = 0f;
        Time.timeScale = 0;
        PlusSetupImage.SetActive(true);
        Dot5.enabled = false;
        //initial_rotation = model.rot.localRotation;
    }

    void Update()
    {
        
        if (!WiimoteManager.HasWiimote())
        {
            SearchAndInitialize();
            return;
        }
        int ret;
        do
        {
            ret = wiimote.ReadWiimoteData();

            if (motionPlus && wiimote.current_ext == ExtensionController.MOTIONPLUS)
            {
                UpdateControls();
                ConfigureMotionPlus();
                
            }
            else SearchAndActivateMotionPlus();

        } while (ret > 0);

        model.a.enabled = wiimote.Button.a;
        model.b.enabled = wiimote.Button.b;
        model.one.enabled = wiimote.Button.one;
        model.two.enabled = wiimote.Button.two;
        model.d_up.enabled = wiimote.Button.d_up;
        model.d_down.enabled = wiimote.Button.d_down;
        model.d_left.enabled = wiimote.Button.d_left;
        model.d_right.enabled = wiimote.Button.d_right;
        model.plus.enabled = wiimote.Button.plus;
        model.minus.enabled = wiimote.Button.minus;
        model.home.enabled = wiimote.Button.home;

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

    void UpdateControls()
    {
        pauseButtonClick++;


        if (WiimoteManager.HasWiimote())
        {
            Dot5.enabled = Time.timeScale == 0 && plusSetup  ? true : false;
            UpdateRotationWiimotePlus();

            if (wiimote.Button.home && pauseButtonClick > 100 && plusSetup)
            {
                pauseButtonClick = 0;
                Time.timeScale = Time.timeScale == 0 ? 1 : 0;
                MenuController.SendMessage("ShowHideMenu", Time.timeScale != 0 && plusSetup);
            }

            //Shoot if button A is pressed.
            if (wiimote.Button.a)
            {
                //Shoots raycasts to interact with buttons in menu mode, shoots balls to destroy objectives if not.
                if (Time.timeScale == 0)
                {
                    RaycastHit hit;

                    Debug.DrawRay(ir_origin.transform.position, -(ir_origin.transform.position - ir_pointer.position).normalized * 20);
                    if (Physics.Raycast(ir_origin.transform.position, -(ir_origin.transform.position - ir_pointer.position).normalized * 20, out hit))
                    {
                        Button button = hit.collider.gameObject.GetComponent<Button>();
                        if (button != null) button.onClick.Invoke();
                    }
                }
            }
            
        }
    }

    void UpdateRotationWiimotePlus()
    {
        if (Time.timeScale != 0)
        {
            if (motionPlus)
            {

                if (plusSetup)
                {
                    Vector3 offset = new Vector3(-wiimote.MotionPlus.PitchSpeed,
                                                  wiimote.MotionPlus.YawSpeed,
                                                  wiimote.MotionPlus.RollSpeed) / 95f; // Divide by 95Hz (average updates per second from wiimote)
                    
                    if(offset - wmpDeviation != Vector3.zero)
                        model.rot.Rotate(offset - wmpDeviation, Space.Self);
                }
            }

            //model.rot.rotation = Quaternion.FromToRotation(ir_origin.transform.position, ir_pointer.position) * Quaternion.Euler(new Vector3(0, 180, 0));
            //model.rot.localRotation = Quaternion.LookRotation(ir_pointer.position, model.rot.position) * Quaternion.Euler(new Vector3(0, 0, 180 * wiimote.Ir.GetPointingPosition()[1]));
        }
        else if(plusSetup)
        {
            model.rot.rotation = Quaternion.FromToRotation(ir_origin.transform.position, - (ir_origin.transform.position - ir_pointer.position).normalized) * Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    private void UpdatePosition()
    {

        var accelVector = GetAccelVector();
        

        double precision = 0.1;

        if (Mathf.Abs(accelVector.x - accelDeviaton.x) > precision || Mathf.Abs(accelVector.y - accelDeviaton.y) > precision || Mathf.Abs(accelVector.z - accelDeviaton.z) > precision)
        {
            model.rot.position += accelVector.normalized * 10 * Time.deltaTime;
        }

    }

    void SearchAndInitialize()
    {
        WiimoteManager.FindWiimotes();

        if (!WiimoteManager.HasWiimote())
        {
            return;
        }

        wiimote = WiimoteManager.Wiimotes[0];
        wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);
        wiimote.SetupIRCamera(IRDataType.BASIC);
        wiimote.SendPlayerLED(true, true, true, true);

    }

    private Vector3 GetAccelVector()
    {
        float accel_x;
        float accel_y;
        float accel_z;

        float[] accel = wiimote.Accel.GetCalibratedAccelData();
        accel_x = accel[0];
        accel_y = -accel[2];
        accel_z = -accel[1];

        return new Vector3(accel_x, accel_y, accel_z).normalized;
    }

    //Cleans wiimotes from pool when scene changes or you exit from application.
    void OnApplicationQuit()
    {
        OnDestroy();
    }

    void OnDestroy()
    {
        if (wiimote != null)
        {
            WiimoteManager.Cleanup(wiimote);
            wiimote = null;
        }
    }
    
    private void ConfigureMotionPlus()
    {
        if (!plusSetup)
        {
            var precision = 0.2f;

            wmpDeviation = new Vector3(-wiimote.MotionPlus.PitchSpeed,
                                        wiimote.MotionPlus.YawSpeed,
                                        wiimote.MotionPlus.RollSpeed) / 95f;

            //Setup based on time
            if ((wmpDeviation.x - plusSetupVector.x) < precision && (wmpDeviation.y - plusSetupVector.y) < precision && (wmpDeviation.z - plusSetupVector.z) < precision)
            {
                if (plusSetupStill > 1000)
                {
                    InitializeScene();
                }
                else
                {
                    plusSetupStill++;
                }
            }
            else
            {
                wmpDeviation = plusSetupVector;
                plusSetupStill = 0;
            }
        }

        //Setup based on buttons
        if (wiimote.Button.one && wiimote.Button.two)
        {
            InitializeScene();
        }
    }

    private void InitializeScene()
    {
        plusSetup = true;
        Time.timeScale = 1;
        LaserSwordEffects.SendMessage("TurnOn", true);
        model.rot.localRotation = initial_rotation;
        PlusSetupImage.SetActive(false);
        ConfigureAccelerometer();

        if(LaserSwordBladeGlow.GetComponent<BoxCollider>() == null)
        {
            var boxCollider = LaserSwordBladeGlow.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(0.65f, 0.65f, boxCollider.size.z);

            var laserCollisionManager = LaserSwordBladeGlow.AddComponent<LaserSwordColissionManager>();
                laserCollisionManager.scoreText = ScoreText;
        }
    }

    private void ConfigureAccelerometer()
    {
        if (accelDeviaton == null)
        {
            accelDeviaton = GetAccelVector();
        }
    }

    private void SearchAndActivateMotionPlus()
    {
        motionPlus = false;
        wiimote.RequestIdentifyWiiMotionPlus();

        if (wiimote.wmp_attached)
        {
            wiimote.ActivateWiiMotionPlus();
            motionPlus = true;
        }
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

}
