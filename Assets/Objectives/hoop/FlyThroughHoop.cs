using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyThroughHoop : MonoBehaviour
{
    public Vector3 enterPosition;

    private float along(Vector3 p)
    {
        return Vector3.Dot(p - transform.position, transform.up);
    }

    void OnTriggerEnter(Collider c)
    {
        if (!c.gameObject.CompareTag("Player"))
            return;
        enterPosition = c.gameObject.transform.position;
    }

    void OnTriggerExit(Collider c)
    {
        if (!c.gameObject.CompareTag("Player"))
            return;
        // is the object on the opposite side of the plane?
        if ((along(enterPosition) > 0) != (along(c.gameObject.transform.position) > 0))
            Destroy(transform.parent.gameObject);
    }
}
