using UnityEngine;

public class DroneUserInput : MonoBehaviour
{
    private DroneSensors ds;
    private DroneMotors dm;
    private DroneController dc;
    new private Rigidbody rigidbody;

    void Start()
    {
        ds = GetComponent<DroneSensors>();
        dm = GetComponent<DroneMotors>();
        dc = GetComponent<DroneController>();
        rigidbody = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (Input.GetButtonDown("Armed"))
            dc.armed = !dc.armed;

        if (Input.GetButtonDown("Acrobatic"))
            dc.acrobaticMode = !dc.acrobaticMode;

        if (Input.GetButtonDown("Reset"))
        {
            transform.rotation = Quaternion.identity;
            rigidbody.rotation = Quaternion.identity;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            ds.hardReset();
            dm.hardReset();
            dc.reset();
            dc.armed = false;
        }

        if (!dc.armed)
        {
            dc.axisPitch = 0;
            dc.axisYaw = 0;
            dc.axisRoll = 0;
            dc.axisThrottle = 0;
            return;
        }

        dc.axisRoll = -Input.GetAxis("AD");
        dc.axisPitch = Input.GetAxis("SW");
        if (dc.acrobaticMode)
        {
            dc.axisYaw = Input.GetAxis("QE");
            dc.axisThrottle = Input.GetAxis("OP");
        }
        else
        {
            dc.axisYaw = Input.GetAxis("OP");
            dc.axisThrottle = Input.GetAxis("QE");
        }
    }
}
