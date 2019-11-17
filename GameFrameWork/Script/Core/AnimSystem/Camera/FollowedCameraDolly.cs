using UnityEngine;
using System.Collections;
/////////////////////////////////////////////////////////////////
// FollowedCamera 推拉功能.
// 方案:
// 让cam.offsetPos沿cam.offsetPos的方向双向移动,
// 之后摄影机将向offsetPos移动
// 算法: 
// 1 根据distance值计算出targetPos方向的min(最小向量),max(最大向量).
// 2 计算targetPos与min的点乘,若<0:
// 	若 > 0 : 当targetPos的长度超出[min,max],对其进行限制.
// 	若 < 0 : targetPos置为min.
/////////////////////////////////////////////////////////////////
public class FollowedCameraDolly : MonoBehaviour
{
    // 距离观察目标的距离范围[最小值,最大值]
    public Vector2 distance = new Vector2(4, 20);
    // 滚轮速度
    public float scrollSpeed = 400;
    // 滚动方向反转?
    public bool isScrollRevert;
	//重新计算初始数据.
    public bool isRefresh;
	
	public Vector2 rotateX = new Vector2(30,45); //x轴角度.
	public float smoothSpeed = 0.11f; //向目标点,平滑移动速度
	public float smoothRotateSpeed = 0.11f; //向目标方位,平滑旋转速度

    FollowedCamera cam;
    Vector3 offsetDir;
    Vector3 dollyDir;

    Vector3 min;
    Vector3 max;
    float minLength;
    float maxLength;
	
	public float ratio; //当前位置在[min,max]的比例,[0,1]
	public float smoothRatio;
	public Vector3 targetPos; //目标位置,要平移到此位置
	
    // Use this for initialization
    void Start()
    {
        cam = GetComponent<FollowedCamera>();
		targetPos = cam.offsetPos;
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        if (!enabled)
            return;
        if (isRefresh)
        {
            isRefresh = false;
            Refresh();
        }
		
		Rotate();
        Dolly();
    }

    void Refresh()
    {
        offsetDir = cam.offsetPos.normalized;
        min = offsetDir * distance.x;
        max = offsetDir * distance.y;
        minLength = min.magnitude;
        maxLength = max.magnitude;
    }

    void Dolly()
    {
        float v = GetValue();
        
        dollyDir = offsetDir * v * scrollSpeed * Time.deltaTime;
		targetPos += dollyDir;
		ClampTargetPos();
		cam.offsetPos = Vector3.Lerp(cam.offsetPos,targetPos,smoothSpeed);
    }
	
    void ClampTargetPos()
    {
        float m = targetPos.magnitude;
		float len = (maxLength - minLength);
		ratio = (m - minLength) / len;
		smoothRatio = Mathf.Lerp(smoothRatio,ratio,smoothRotateSpeed);
		
        float dirDot = Vector3.Dot(targetPos, min);
        if (dirDot > 0)
        {
            if (m > maxLength)
                targetPos = max;
            else if (m < minLength)
                targetPos = min;
        }
        else
        {
            targetPos = min;
        }
    }
	
	void Rotate(){
		var euler = cam.tr.localEulerAngles;
		euler.x = Mathf.LerpAngle(rotateX.x,rotateX.y,smoothRatio);
		cam.tr.localRotation = Quaternion.Slerp(cam.tr.localRotation,Quaternion.Euler(euler),smoothRotateSpeed);
	}

    float GetValue()
    {
		float v = Input.GetAxis("Mouse ScrollWheel");
		v *= isScrollRevert ? -1 : 1;
        return v;
    }
	
	void OnDrawGizmos(){
		Gizmos.DrawSphere(targetPos,0.2f);
	}
}
