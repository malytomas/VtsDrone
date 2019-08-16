using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public GameObject target;

    private void Start()
    {}

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(25, target.transform.rotation.eulerAngles.y, 0);
        transform.position = target.transform.position + transform.rotation * new Vector3(0, 0, -5);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
        }
    }
}
