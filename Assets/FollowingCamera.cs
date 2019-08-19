using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public GameObject target;

    private float y;

    private void Start()
    {}

    void FixedUpdate()
    {
        float t = target.transform.rotation.eulerAngles.y;
        y = transform.rotation.eulerAngles.y;
        float d = t - y;
        if (d > 180)
            d = d - 360;
        if (d < -180)
            d = 360 + d;
        y += d * 0.1f;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(25, y, 0);
        transform.position = target.transform.position + transform.rotation * new Vector3(0, 0, -5);

        if (Input.GetKeyDown(KeyCode.C))
        {
        }
    }
}
