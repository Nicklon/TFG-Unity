using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class WiimoteShooter : MonoBehaviour
{
    public GameObject MenuController;
    public bool MainMenu;

    public GameObject bulletPrefab;
    public WiimoteModel model;
    public RectTransform[] ir_dots;
    public RectTransform[] ir_bb;
    public RectTransform ir_pointer;
    public GameObject ir_origin;
    private float firingRate = 0.5f;

    private Quaternion initial_rotation;
    private float firingTimer;
    private Wiimote wiimote;
    private float ticksSinceHomeBton = 100;

    //To prevent multiple scenes load at the same time
    private float initialDelay = 100;
    private float ticksSinceLoad = 0;

    void Start()
    {
        Time.timeScale = 0;
        firingTimer = 0f;
        //initial_rotation = model.rot.localRotation;
    }
    void Update()
    {
        if(ticksSinceLoad < initialDelay)
        {
            ticksSinceLoad++;
        }
        else if(ticksSinceLoad == initialDelay)
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
            int ret;
            do
            {
                ret = wiimote.ReadWiimoteData();

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

            if (wiimote.current_ext != ExtensionController.MOTIONPLUS)
                model.rot.localRotation = initial_rotation;

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

            UpdateControls();
            UpdateRotationWiimote();
        }
        
    }
    
    void UpdateControls()
    {
        ticksSinceHomeBton++;

        if (wiimote.Button.home && ticksSinceHomeBton > 100)
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            ticksSinceHomeBton = 0;
            MenuController.SendMessage("ShowHideMenu", Time.timeScale != 0);
        }

        //Shoot if button A is pressed.
        if (wiimote.Button.a)
        {
            //Shoots raycasts to interact with buttons in menu mode, shoots balls to destroy objectives if not.
            if (Time.timeScale == 0 || MainMenu)
            {
                RaycastHit hit;

                if (Physics.Raycast(ir_origin.transform.position, - (ir_origin.transform.position - ir_pointer.position).normalized*50, out hit))
                {
                    Button button = hit.collider.gameObject.GetComponent<Button>();
                    if (button != null) button.onClick.Invoke();
                }
            }
            else
            {
                if (firingTimer > firingRate)
                {
                    firingTimer = 0f;
                    CreateBullet();
                }
            }
        }

        firingTimer += Time.deltaTime;
    }

    private void CreateBullet()
    {
        var bullet = (GameObject)Instantiate(bulletPrefab, ir_origin.transform.position, Quaternion.LookRotation(ir_pointer.position, model.rot.position) * Quaternion.Euler(new Vector3(0, 0, 180 * wiimote.Ir.GetPointingPosition()[1])));
        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10 ;
        // Destroy the bullet after 2 seconds
        Destroy(bullet, 0.75f);
    }

    void UpdateRotationWiimote()
    {
        model.rot.rotation = Quaternion.FromToRotation(ir_origin.transform.position, ir_pointer.position) * Quaternion.Euler(new Vector3(0, 180, 0));
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
        wiimote.SetupIRCamera(IRDataType.EXTENDED);
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
