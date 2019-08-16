using UnityEngine;

public class DroneSensors : MonoBehaviour
{
    // degrees
    public float pitch = 0;
    public float yaw = 0;
    public float roll = 0;
    public float altitude = 0;
    public float pitchRate = 0;
    public float yawRate = 0;
    public float rollRate = 0;
    public float altitudeRate = 0;

    private float altitudeLast = 0;
    new private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 ea = rigidbody.angularVelocity * 180 / Mathf.PI * Time.fixedDeltaTime;
        ea = Quaternion.Inverse(rigidbody.rotation) * ea;
        pitchRate = (ea.x + 180) % 360 - 180;
        yawRate = (ea.y + 180) % 360 - 180;
        rollRate = (ea.z + 180) % 360 - 180;
        pitch += pitchRate;
        yaw += yawRate;
        roll += rollRate;
        altitudeLast = altitude;
        altitude = transform.position.y;
        altitudeRate = altitude - altitudeLast;
    }

    public void hardReset()
    {
        pitch = 0;
        yaw = 0;
        roll = 0;
        altitude = 0;
        pitchRate = 0;
        yawRate = 0;
        rollRate = 0;
        altitudeRate = 0;
        altitudeLast = 0;
    }
}
