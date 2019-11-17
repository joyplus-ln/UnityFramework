using UnityEngine;
using System.Collections;
/////////////////////////////////////////////////////////////////////////
//摄影机 震动器
/////////////////////////////////////////////////////////////////////////

public class CameraShaker : MonoBehaviour
{
    public BaseCamera cam;
    public bool isShake;
	public bool isUseAtten; //使用衰减?
    public float shakeLastTime = 0.1f;
	public Vector3 noise = Vector3.one;
	
	
    protected float shakeStopTime;
	protected float shakeStartTime;
    protected Vector3 shakePos;
	protected float atten;
    // Use this for initialization
    public void Start()
    {
        cam = GetComponent<BaseCamera>();
    }

    // Update is called once per frame
    public void Update()
    {
        if (isShake)
        {
            isShake = false;
            shakeStopTime = Time.time + shakeLastTime;
			shakeStartTime = Time.time;
        }
        Shake();
    }
	
	public void Setup(float shakeLastTime,Vector3 noise,bool isShake)
	{
		this.shakeLastTime = shakeLastTime;
		this.noise = noise;
		this.isShake = isShake;
	}
	
    public void Shake()
    {
        if (Time.time < shakeStopTime)
        {
            shakePos.Set(GetRandom(noise.x), GetRandom(noise.y), GetRandom(noise.z));
			if(isUseAtten)
				shakePos *= GetAtten();
        }
        else
        {
            shakePos.Set(0, 0, 0);
        }
		
        cam.shakePos = shakePos;
    }

    public float GetRandom(float f)
    {
        return Random.Range(-f, f);
    }
	
	public float GetAtten()
	{
		return 1f - (Time.time - shakeStartTime) / shakeLastTime; //线性衰减.
	}
	
	//接收通知.
	public void OnShake(){
		isShake = true;
	}
}
