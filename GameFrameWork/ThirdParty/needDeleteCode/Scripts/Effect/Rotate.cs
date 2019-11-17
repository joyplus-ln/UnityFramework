using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotateAxis = Vector3.up;

    public float degreesPerSecond = 45f;

    public bool worldSpace = false;

    private Transform xform;

    private void Awake()
    {
        xform = transform;
    }

    private void Update()
    {
        xform.Rotate(rotateAxis, degreesPerSecond * Time.deltaTime, (worldSpace) ? Space.World : Space.Self);
    }
}