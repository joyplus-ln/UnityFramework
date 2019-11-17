using UnityEngine;
using System.Collections;
/////////////////////////////////////////////////////////////////////////
//摄影机 震动器
/////////////////////////////////////////////////////////////////////////

public class CameraShakerFall : CameraShaker
{
    // Use this for initialization
    new public void Start()
    {
        // cam = GetComponent<BaseCamera>();
		base.Start();
    }

    // Update is called once per frame
    new public void Update()
    {
		base.Update();
        // if (isShake)
        // {
            // isShake = false;
            // shakeStopTime = Time.time + shakeLastTime;
        // }
        // Shake();
    }

    //public void Setup(float shakeLastTime, Vector3 noise, bool isShake)
    //{
    //    this.shakeLastTime = shakeLastTime;
    //    this.noise = noise;
    //    this.isShake = isShake;
    //}

    new protected void Shake()
    {
        if (Time.time < shakeStopTime)
        {
            noise *= ((shakeStopTime - Time.time)/shakeLastTime);
            shakePos.Set(GetRandom(noise.x), GetRandom(noise.y), GetRandom(noise.z));
        }
        else
        {
            shakePos.Set(0, 0, 0);
        }
        cam.shakePos = shakePos;
    }
}
