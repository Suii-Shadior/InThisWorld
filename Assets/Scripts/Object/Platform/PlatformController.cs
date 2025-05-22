using PlatformFactoryRelated;
using PlatformInterfaces;
using SubInteractiveEnum;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CompositeCollider2D))]
public class PlatformController : MonoBehaviour
{
    #region 组件
    public NewPlayerController thePlayer;
    private CompositeCollider2D thisComCol;
    private Rigidbody2D thisRB;

    #endregion

    #region 变量
    [Header("————使用说明————\n1、选择本脚本适用的开关类型，即thisPlatformType\n2、设定该对象是否初始状态is Hidden\n3、根据种类同进行设定，详情看对应Related")]
    [Space(3)]


    [Header("Platform Setting")]
    public PlatformInteractiveType thisPlatformType;
    public PlatformFactory thisFactory;
    public bool isHidden;
    public bool canBeHaulted;
    public float haultedDuration;
    [Header("Platform Info")]
    public IPlatform currentPlatform;
    public bool isHaulted;
    public float haultedCounter;
    public bool hasSensored;

    [Header("Units Related")]
    [HideInInspector]public List<PlatformUnit> units = new List<PlatformUnit>();

    [Header("Movable Related")]//可移动平台，包括单向Singler，往返Rounder，触发Sensor，及开关复合电梯Elevator
    [Header("4、添加移动速度，即moveSpeed\n5、更改终点位置，即theEndPoint的位置\n6、若sensor，添加到达时间\n7、若elevator，有些内容共用")]
    public float moveSpeed;//elevator共用
    public Transform theStartPoint;
    public Transform theEndPoint;
    public Transform theNowPoint;
    public Transform theDestinalPoint;
    public Vector3 offsetVec;//用于移动时计算偏移量

    public bool isDestinationOrienting;
    public bool hasArrived;//elevator,hander共用
    public float hasArrivedCounter;
    public float hasArrivedDuration;
    public int handlerInput;
    public bool hasOringinPosed;
    public float perBackStepCounter;
    public float perBackStepDuration;

    [Header("Disappear Related")]//消失平台，包括触发Sensor，定时Regular，及开关符合二择Pair
    [Header("4、添加消失时间和重现，即disappearDuration和reappearDuration")]
    public bool canDisappearOrReappear;//用于防止在Player落下的时候刚触发但平台碰撞体还没消失的时候又触发一次消失
    public bool needToDisappear;
    public bool needToReappear;
    public float disappearCounter;
    public float disappearDuration;
    public float reappearCounter;
    public float reappearDuration;

    [Header("Rotater Related")]//旋转平台，包括可控rotater
    [Header("4、添加旋转时间，即\n5、更改旋转中点位置，即theRotatePovit的位置\n6、若为elevator，有些内容共用")]
    public Transform theRotatePovit_ElevatorPoint;//elevator共用
    public float rotationDuration;//旋转总计需要的时间
    public bool isRotating;
    public float nowAngle;//累计角度，应该要存档
    public float rotateStep;//中间变量
    public float hadRotated;
    public int isClockwise;

    //[Header("Switchable Related")]


    #endregion


    private void Awake()
    {
        thisComCol = GetComponent<CompositeCollider2D>();
        thisRB = GetComponent<Rigidbody2D>();

        GetComponentsInChildren(units);//这个用于列表 
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.movable_singler://可移动平台，单向移动+重置
                thisFactory= new SinglerFactory();
                break;
            case PlatformInteractiveType.moveable_rounder://可移动平台，往返移动
                thisFactory = new RounderFactory();
                break;
            case PlatformInteractiveType.moveable_toucher://可移动平台，即踩上后马上向目标地点移动，玩家离开一定时间后重置
                thisFactory = new ToucherFactory();
                break;
            case PlatformInteractiveType.disappearable_sensor://消失平台的默认，即踩上后短暂时间消失，一定时间后重置
                thisFactory = new SensorFactory();
                break;
            case PlatformInteractiveType.disappearable_regular://消失平台，往复消失
                thisFactory = new RegularorFactory();
                break;
            case PlatformInteractiveType.disappearable_presser:
                thisFactory = new PresserFactory();
                break;
            case PlatformInteractiveType.switchable_elevator://可移动平台，呼叫+可控移动
                thisFactory = new ElevatorFactory();
                break;
            case PlatformInteractiveType.switchable_pair:
                thisFactory = new PairerFactory();
                break;
            case PlatformInteractiveType.moveable_handler:
                thisFactory = new HandlerFactory();
                break;
            case PlatformInteractiveType.rotateable_rotater://旋转平台，可控旋转
                thisFactory = new RotaterFactory();
                break;
        }
        currentPlatform = thisFactory?.CreatePlatform(this);
        //units = GetComponentsInChildren<PlatformUnit>();//这个用于数组
    }

    private void Start()//根据类型不同进行不同初始化
    {
       
    }

    private void Update()//根据类型不同进行不同的Update，主要是判断和移动
    {
        currentPlatform.SceneExist_Updata();
    }
    private void FixedUpdate()//根据类型不同进行不同的FixUpdate，目前没有使用
    {
    }






    #region 各种小方法及外部调用


    public void SetPlayer(NewPlayerController _player)
    {
        thePlayer = _player;
    }
    public NewPlayerController GetPlayer()
    {
        return thePlayer;
    }
    public CompositeCollider2D GetComCol()
    {
        return thisComCol;
    }

    public void CallThisPlatform(Transform _destinationPoint)//TODO：Elevator使用，要优化
    {
        theDestinalPoint.position = _destinationPoint.position;
    }

    public void ResetThisPlatform()//解谜相关
    {
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.rotateable_rotater:
                RotatablePlatform_AttackReset();
                break;
        }
    }


    private void RotatablePlatform_AttackReset()
    {
        transform.RotateAround(theRotatePovit_ElevatorPoint.position, Vector3.forward, -nowAngle);
        nowAngle = 0f;
    }





    #endregion

}
