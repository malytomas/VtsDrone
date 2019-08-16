using UnityEngine;

public class DroneMotors : MonoBehaviour
{
    public float[] throttle = new float[4]; // FL, FR, RL, RR
    public float forcePerThrottle = 10;
    public float torquePerThrottle = 10;
    public float rotationsPerThrottle = 1000;

    private float[] throttleAcc = new float[4]; // FL, FR, RL, RR
    private Transform[] rotors = new Transform[4];
    new private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rotors[0] = transform.Find("drone/rotor_fl");
        rotors[1] = transform.Find("drone/rotor_fr");
        rotors[2] = transform.Find("drone/rotor_rl");
        rotors[3] = transform.Find("drone/rotor_rr");
    }

    void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
            throttle[i] = Mathf.Clamp01(throttle[i]);

        for (int i = 0; i < 4; i++)
        {
            int negator = i == 0 || i == 3 ? 1 : -1;
            throttleAcc[i] = throttleAcc[i] * 0.95f + throttle[i] * 0.05f;
            Vector3 force = transform.up * forcePerThrottle * throttleAcc[i];
            Vector3 offset = transform.position
                + transform.right * (i % 2 == 0 ? -1 : 1) * 0.28f
                + transform.forward * (i / 2 == 0 ? 1 : -1) * 0.28f;
            rigidbody.AddForceAtPosition(force, offset);
            rigidbody.AddTorque(transform.up * throttleAcc[i] * -negator * torquePerThrottle);
            rotors[i].localEulerAngles = rotors[i].localEulerAngles
                + new Vector3(0, throttleAcc[i] * negator * rotationsPerThrottle, 0);
        }
    }

    public void hardReset()
    {
        for (int i = 0; i < 4; i++)
        {
            throttle[i] = 0;
            throttleAcc[i] = 0;
        }
    }
}
