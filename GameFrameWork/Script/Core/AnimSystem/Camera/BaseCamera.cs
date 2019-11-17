using UnityEngine;
using System.Collections;

public class BaseCamera : MonoBehaviour
{
    public Vector3 shakePos;
    public Transform target;

    public Transform tr;
    // Use this for initialization
    public void Start()
    {
        tr = transform;
    }

}
