using UnityEngine;
using System.Collections;
/////////////////////////////////////////////////////////////////////////
//简单式跟随摄影机.
/////////////////////////////////////////////////////////////////////////

public class FollowedCamera : BaseCamera
{
    public float speed = 4f;
    public Vector3 offsetPos = new Vector3(-7f, 12f, -7f);
	public Color gizmosColor = Color.red;
	
    // Use this for initialization
    // new public void Start()
    // {
        // base.Start();
    // }

    // Update is called once per frame
    public void Update()
    {
        if (target)
        {
            tr.position = Vector3.Lerp(tr.position, target.position + offsetPos + shakePos, speed * Time.deltaTime);
        }
    }
	
	//设置 摄影机盯住的物体.
	void OnSetTarget(GameObject go)
	{
		if(go)
			target = go.transform;
	}
	
	void OnDrawGizmos(){
		Gizmos.color = gizmosColor;
		if(target)
			Gizmos.DrawRay(target.position,offsetPos);
	}
}
