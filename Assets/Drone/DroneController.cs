using UnityEngine;

public class DroneController : MonoBehaviour
{
    public bool acrobaticMode = false;
    public bool armed = false;
    public float axisPitch = 0;
    public float axisYaw = 0;
    public float axisRoll = 0;
    public float axisThrottle = 0;

    protected PID pitch;
    protected PID yaw;
    protected PID roll;
    protected float throttle = 0;

    private DroneMotors dm;
    private DroneSensors ds;

    void Start()
    {
        pitch = new PID(0.02f, 0.0001f, 0.05f);
        yaw = new PID(0.02f, 0.0001f, 0.05f);
        roll = new PID(0.02f, 0.0001f, 0.05f);

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

        float pitchChange = axisPitch;
        float yawChange = axisYaw;
        float rollChange = axisRoll;
        float throttleChange = axisThrottle;

        if (acrobaticMode)
        {
            pitchChange *= 1.5f;
            yawChange *= 1.5f;
            rollChange *= 1.5f;
            throttleChange *= 0.01f;
        }
        else
        {
            // todo
        }

        float pitchResponse = pitch.update(pitchChange, ds.pitchRate);
        float yawResponse = yaw.update(yawChange, ds.yawRate);
        float rollResponse = roll.update(rollChange, ds.rollRate);

        throttle += throttleChange;
        throttle = Mathf.Clamp01(throttle);

        dm.throttle[0] = throttle - rollResponse - pitchResponse - yawResponse;
        dm.throttle[1] = throttle + rollResponse - pitchResponse + yawResponse;
        dm.throttle[2] = throttle - rollResponse + pitchResponse + yawResponse;
        dm.throttle[3] = throttle + rollResponse + pitchResponse - yawResponse;
    }

    public void reset()
    {
        pitch.reset();
        yaw.reset();
        roll.reset();
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
