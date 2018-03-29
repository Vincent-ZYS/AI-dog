using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    //单例
    private static CameraController instance;
    public bool isFollowPet = false;
    public static CameraController Instance
    {
        get
        {
            return instance;
        }
        set
        {
            instance = value;
        }
    }
    //镜头转换的速度
    public float speed = 1;
    //目标的位置
    public Vector3 targetPosition;
    //摄像机右偏移的倍数
    public float right_multiple = 0.7f;
    //摄像机Y轴偏移的倍数
    public float up_multiple=0.3f;
    //望向的主角
    public Transform dog;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        targetPosition = transform.position;
    }
    void LateUpdate()
    { 
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
        Quaternion targetRot = Quaternion.LookRotation(dog.position - transform.position+Vector3.right*right_multiple-Vector3.up*up_multiple);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, speed * Time.deltaTime);
    }
    public void SetCamera(Vector3 targetPosition, float speed = 1,float right_multiple=10,float up_multiple=-4.3f)
    {
        this.targetPosition = targetPosition;
        this.speed = speed;
        this.right_multiple = right_multiple;
        this.up_multiple = up_multiple;
       
    }
 

}
