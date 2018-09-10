using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using WiimoteApi;
using System.Linq;

public class WiimoteIRDemo : MonoBehaviour {

	//[SerializeField] Particle shootParticles;

    public WiimoteModel model;
    public RectTransform[] ir_dots;
    public RectTransform[] ir_bb;
    public RectTransform ir_pointer;
	public GameObject ir_origin;

	private Quaternion initial_rotation;

    private Wiimote wiimote;

    private Vector3 wmpOffset = Vector3.zero;

    void Start() 
	{
        //initial_rotation = model.rot.localRotation;
    }

	void Update () {
		
        if (!WiimoteManager.HasWiimote()) { 
			SearchAndInitialize ();
			return; 
		}
		print ("NO DEBERIAS ESTAR AQUI!");
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
            if (x == -1 || y == -1) {
                ir_dots[i].anchorMin = new Vector2(0, 0);
                ir_dots[i].anchorMax = new Vector2(0, 0);
            }

            ir_dots[i].anchorMin = new Vector2(x, y);
            ir_dots[i].anchorMax = new Vector2(x, y);

            if (ir[i, 2] != -1)
            {
                int index = (int)ir[i,2];
                float xmin = (float)wiimote.Ir.ir[index,3] / 127f;
                float ymin = (float)wiimote.Ir.ir[index,4] / 127f;
                float xmax = (float)wiimote.Ir.ir[index,5] / 127f;
                float ymax = (float)wiimote.Ir.ir[index,6] / 127f;
                ir_bb[i].anchorMin = new Vector2(xmin, ymin);
                ir_bb[i].anchorMax = new Vector2(xmax, ymax);
            }
        }

        float[] pointer = wiimote.Ir.GetPointingPosition();
        ir_pointer.anchorMin = new Vector2(pointer[0], pointer[1]);
		ir_pointer.anchorMax = new Vector2(pointer[0], pointer[1]);

		UpdateControls ();
		UpdateRotationWiimote ();
	}

	void UpdateControls()
	{
		//Shoot if button A is pressed.
		//shootParticles.SetActive(wiimote.Button.a);

		/*
		if (wiimote.current_ext == ExtensionController.MOTIONPLUS) {
			//Reset wiimote rotation if buttons 1 and 2 are pressed.
			if (wiimote.Button.one && wiimote.Button.two) 
			{
				MotionPlusData data = wiimote.MotionPlus;
				data.SetZeroValues();
				model.rot.rotation = Quaternion.FromToRotation(model.rot.rotation*GetAccelVector(), Vector3.up) * model.rot.rotation;
				model.rot.rotation = Quaternion.FromToRotation(model.rot.forward, Vector3.forward) * model.rot.rotation;		
			}
		}
		*/
	}


	void UpdateRotationWiimote()
	{
		model.rot.localRotation = Quaternion.LookRotation (ir_pointer.position,model.rot.position) * Quaternion.Euler(new Vector3(0,0,180*wiimote.Ir.GetPointingPosition()[1]));
	}

	void SearchAndInitialize(){
		WiimoteManager.FindWiimotes();

		if (!WiimoteManager.HasWiimote()) { 
			return; 
		}

		wiimote = WiimoteManager.Wiimotes[0];
		wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);
		wiimote.SetupIRCamera(IRDataType.EXTENDED);
		wiimote.SendPlayerLED(true,true,true,true);

		Debug.Log ("FOUND ONE");
	}

    void OnDrawGizmos()
    {
        if (wiimote == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(model.rot.position, model.rot.position + model.rot.rotation*GetAccelVector()*2);
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
