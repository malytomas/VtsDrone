using UnityEngine;

public class DroneController : MonoBehaviour
{
    public bool acrobaticMode = false;
    public bool armed = false;
    public float axisPitch = 0;
    public float axisRoll = 0;
    public float axisYaw = 0;
    public float axisThrottle = 0;

    protected PID pitchRatePid;
    protected PID rollRatePid;
    protected PID yawRatePid;
    protected float throttle = 0;

    protected PID pitchStabPid;
    protected PID rollStabPid;
    protected PID yawStabPid;
    protected float yaw = 0;
    public PID altitudeStabPid;

    private DroneMotors dm;
    private DroneSensors ds;

    void Start()
    {
        pitchRatePid = new PID(0.02f, 0.0001f, 0.2f);
        rollRatePid = new PID(0.02f, 0.0001f, 0.2f);
        yawRatePid = new PID(0.02f, 0.0001f, 0.2f);

        pitchStabPid = new PID(0.05f, 0.00005f, 0.2f);
        rollStabPid = new PID(0.05f, 0.00005f, 0.2f);
        yawStabPid = new PID(0.05f, 0.0002f, 0.2f);
        altitudeStabPid = new PID(0.1f, 0.0f, 5.0f);

        dm = GetComponent<DroneMotors>();
        ds = GetComponent<DroneSensors>();
    }

    void FixedUpdate()
    {
        if (!armed)
        {
            reset();
            return;
        }

        float pitchInput = axisPitch;
        float rollInput = axisRoll;
        float yawInput = axisYaw;
        float throttleInput = axisThrottle;

        if (acrobaticMode)
        {
            pitchInput *= 1.5f;
            rollInput *= 1.5f;
            yawInput *= 1.5f;
            throttleInput *= 0.01f;
            pitchStabPid.reset();
            rollStabPid.reset();
            yawStabPid.reset();
            yaw = ds.yaw;
            altitudeStabPid.reset();
        }
        else
        {
            pitchInput = pitchStabPid.update(pitchInput * 60, ds.pitch);
            rollInput = rollStabPid.update(rollInput * 60, ds.roll);
            yaw += yawInput * 1.5f;
            yawInput = yawStabPid.update(yaw, ds.yaw);
            throttleInput = altitudeStabPid.update(throttleInput, ds.altitudeRate);
        }

        float pitchResponse = pitchRatePid.update(pitchInput, ds.pitchRate);
        float rollResponse = rollRatePid.update(rollInput, ds.rollRate);
        float yawResponse = yawRatePid.update(yawInput, ds.yawRate);

        throttle += throttleInput;
        throttle = Mathf.Clamp01(throttle);

        dm.throttle[0] = throttle - rollResponse - pitchResponse - yawResponse;
        dm.throttle[1] = throttle + rollResponse - pitchResponse + yawResponse;
        dm.throttle[2] = throttle - rollResponse + pitchResponse + yawResponse;
        dm.throttle[3] = throttle + rollResponse + pitchResponse - yawResponse;
    }

    public void reset()
    {
        pitchRatePid.reset();
        rollRatePid.reset();
        yawRatePid.reset();
        pitchStabPid.reset();
        rollStabPid.reset();
        yawStabPid.reset();
        yaw = ds.yaw;
        altitudeStabPid.reset();
        throttle = 0;
        for (int i = 0; i < 4; i++)
            dm.throttle[i] = 0;
    }
}

// taken from https://forum.unity.com/threads/pid-controller.68390/
// and modified
[System.Serializable]
public class PID
{
    public float pFactor, iFactor, dFactor;

    private float integral;
    private float lastError;

    public PID(float pFactor, float iFactor, float dFactor)
    {
        this.pFactor = pFactor;
        this.iFactor = iFactor;
        this.dFactor = dFactor;
        reset();
    }

    public float update(float setpoint, float actual)
    {
        float present = setpoint - actual;
        integral += present;
        float deriv = present - lastError;
        lastError = present;
        return present * pFactor + integral * iFactor + deriv * dFactor;
    }

    public void reset()
    {
        integral = 0;
        lastError = 0;
    }
}
