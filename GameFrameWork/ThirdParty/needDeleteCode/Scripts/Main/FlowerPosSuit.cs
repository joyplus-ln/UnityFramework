using UnityEngine;

public class FlowerPosSuit : MonoBehaviour
{
    public Vector3 pointSmall;
    public Vector3 pointLarge;

    private Vector3 finalPos;

    private float baseSmall = 1.333f;
    private float baseLarge = 2.165f;
    private float baseRange = 0.832f;

    // Use this for initialization
    private void Start()
    {
        float ScreenHW = (float)Screen.height / Screen.width;

        if (ScreenHW < baseSmall)
        {
            transform.position = pointSmall;
        }
        else if (ScreenHW > baseLarge)
        {
            transform.position = pointLarge;
        }
        else
        {
            transform.position = pointSmall * (baseLarge - ScreenHW) / baseRange + pointLarge * (ScreenHW - baseSmall) / baseRange;
        }
    }
}