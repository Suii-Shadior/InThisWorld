using SubInteractiveEnum;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CompositeCollider2D))]
public class PlatformController : MonoBehaviour
{
    #region 组件
    private NewPlayerController thePlayer;
    private CompositeCollider2D thisComCol;
    private Rigidbody2D thisRB;

    #endregion

    #region 变量
    [Header("————使用说明————\n1、选择本脚本适用的开关类型，即thisPlatformType\n2、设定该对象是否初始状态is Hidden\n3、根据种类同进行设定，详情看对应Related")]
    [Space(3)]


    [Header("Platform Setting")]
    public PlatformInteractiveType thisPlatformType;
    public bool isHidden;
    private bool canBeHaulted;
    public float haultedDuration;
    [Header("Platform Info")]
    [SerializeField] private bool isHaulted;
    [SerializeField] private float haultedCounter;
    public bool hasSensored;

    [Header("Units Related")]
    private List<PlatformUnit> units = new List<PlatformUnit>();

    [Header("Movable Related")]//可移动平台，包括单向Singler，往返Rounder，触发Sensor，及开关复合电梯Elevator
    [Header("4、添加移动速度，即moveSpeed\n5、更改终点位置，即theEndPoint的位置\n6、若sensor，添加到达时间\n7、若elevator，有些内容共用")]
    public float moveSpeed;//elevator共用
    public Transform theStartPoint;
    public Transform theEndPoint;
    public Transform theNowPoint;
    public Transform theDestinalPoint;
    public Vector3 offsetVec;//用于移动时计算偏移量

    private bool isDestinationOrienting;
    private bool hasArrived;//elevator,hander共用
    private float hasArrivedCounter;
    public float hasArrivedDuration;
    public int handlerInput;
    public bool hasOringinPosed;
    public float perBackStepCounter;
    public float perBackStepDuration;

    [Header("Disappear Related")]//消失平台，包括触发Sensor，定时Regular，及开关符合二择Pair
    [Header("4、添加消失时间和重现，即disappearDuration和reappearDuration")]
    public bool canDisappearOrReappear;//用于防止在Player落下的时候刚触发但平台碰撞体还没消失的时候又触发一次消失
    private bool needToDisappear;
    private bool needToReappear;
    private float disappearCounter;
    public float disappearDuration;
    private float reappearCounter;
    public float reappearDuration;

    [Header("Rotater Related")]//旋转平台，包括可控rotater
    [Header("4、添加旋转时间，即\n5、更改旋转中点位置，即theRotatePovit的位置\n6、若为elevator，有些内容共用")]
    public Transform theRotatePovit_ElevatorPoint;//elevator共用
    public float rotationDuration;//旋转总计需要的时间
    private bool isRotating;
    private float nowAngle;//累计角度，应该要存档
    private float rotateStep;//中间变量
    private float hadRotated;
    private int isClockwise;

    //[Header("Switchable Related")]


    #endregion


    private void Awake()
    {
        thisComCol = GetComponent<CompositeCollider2D>();
        thisRB = GetComponent<Rigidbody2D>();

        GetComponentsInChildren(units);//这个用于列表 
        //units = GetComponentsInChildren<PlatformUnit>();//这个用于数组
    }

    private void Start()//根据类型不同进行不同初始化
    {
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.movable_singler://可移动平台，单向移动+重置
                MoveablePlatform_SingleStart();
                break;
            case PlatformInteractiveType.moveable_rounder://可移动平台，往返移动
                MoveablePlatform_RoundStart();
                break;
            case PlatformInteractiveType.moveable_sensor://可移动平台，即踩上后马上向目标地点移动，玩家离开一定时间后重置
                MoveablePlatform_SensorStart();
                break;
            case PlatformInteractiveType.disappearable_sensor://消失平台的默认，即踩上后短暂时间消失，一定时间后重置
                ; DisappearPlatform_SensorStart();
                break;
            case PlatformInteractiveType.disappearable_regular://消失平台，往复消失
                DisappearPlatform_RegularStart();
                break;
            case PlatformInteractiveType.disappearable_presser:
                DisappearPlatform_PresserStart();
                break;
            case PlatformInteractiveType.switchable_elevator://可移动平台，呼叫+可控移动
                SwitchablePlatform_ElevatorStart();
                break;
            case PlatformInteractiveType.moveable_handler:
                MoveablePlatform_HandlerStart();
                break;
            case PlatformInteractiveType.rotateable_rotater://旋转平台，可控旋转
                RotateablePlatform_RotaterStart();
                break;
            default://默认，包括由其他对象协助进行初始化的平台
                //Debug.Log("开关平台初始化");

                break;
        }
    }

    private void Update()//根据类型不同进行不同的Update，主要是判断和移动
    {
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.movable_singler:
                MoveablePlatform_SingleUpdate();
                break;
            case PlatformInteractiveType.moveable_rounder:
                MoveablePlatform_RoundUpdate();
                break;
            case PlatformInteractiveType.moveable_sensor:
                MoveablePlatform_SensorUpdate();
                break;
            case PlatformInteractiveType.moveable_handler:
                MoveablePlatform_HandlerUpdate();
                break;
            case PlatformInteractiveType.disappearable_sensor:
                DisappearPlatform_SensorUpdate();
                break;
            case PlatformInteractiveType.disappearable_regular:
                DisappearPlatform_RegularUpdate();
                break;
            case PlatformInteractiveType.switchable_elevator:
                SwitchablePlatform_ElevatorUpdate();
                break;
            case PlatformInteractiveType.rotateable_rotater:
                RotateablePlatform_AttackUpdate();
                break;
            default://默认，包括不会进行规律变化的平台
                //Debug.Log("开关平台正常运行");
                //Debug.Log("可移动平台正常运行"）;
                break;
        }
    }
    private void FixedUpdate()//根据类型不同进行不同的FixUpdate，目前没有使用
    {
        switch (thisPlatformType)
        {

            default://默认，包括不会进行移动的平台
                //Debug.Log("开关平台正常运行");
                //Debug.Log("消失平台平台正常运行"）;
                break;
        }

    }




    #region 初始化相关
    private void MoveablePlatform_SingleStart()
    {

        canBeHaulted = false;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
        theDestinalPoint.position = theEndPoint.position;
    }
    private void MoveablePlatform_RoundStart()
    {
        canBeHaulted = false;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
        theDestinalPoint.position = theEndPoint.position;
    }
    private void MoveablePlatform_SensorStart()
    {
        canBeHaulted = true;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
    }

    private void MoveablePlatform_HandlerStart()
    {
        canBeHaulted = false;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
    }
    private void DisappearPlatform_SensorStart()
    {
        canBeHaulted = false;
        canDisappearOrReappear = true;
    }
    private void DisappearPlatform_RegularStart()
    {
        DisappearCount();
        canBeHaulted = false;
        canDisappearOrReappear = true;
    }

    private void DisappearPlatform_PresserStart()
    {
        canBeHaulted = false;
        canDisappearOrReappear = true;
    }
    private void SwitchablePlatform_ElevatorStart()
    {
        canBeHaulted = false;
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theRotatePovit_ElevatorPoint.transform.parent = null;
    }
    private void RotateablePlatform_RotaterStart()
    {
        theRotatePovit_ElevatorPoint.transform.parent = null;
    }


    #endregion

    #region 各种平台Update逻辑
    private void MoveablePlatform_SingleUpdate()
    {
        if (!isHaulted)
        {
            if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
            {
                //当平台到达终点后，计算偏移量后加在平台本身和偏移量计算点上，实现两者的重置
                Vector3 changeVec = theStartPoint.position - theNowPoint.position;
                theNowPoint.position += changeVec;
                transform.position += changeVec;
            }
            else
            {
                offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                theNowPoint.position += offsetVec;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;

                }
            }
        }
        else
        {
            if (haultedCounter > 0)
            {
                haultedCounter -= Time.deltaTime;
            }
            else
            {
                isHaulted = false;
            }
        }
    }

    private void MoveablePlatform_RoundUpdate()
    {
        if (!isHaulted)
        {
            if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
            {
                if (Vector2.Distance(theStartPoint.position, theNowPoint.position) < .01f)
                {
                    theDestinalPoint.position = theEndPoint.position;
                }
                else if (Vector2.Distance(theEndPoint.position, theNowPoint.position) < .01F)
                {
                    theDestinalPoint.position = theStartPoint.position;
                }
            }
            else
            {
                offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                theNowPoint.position += offsetVec;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                }
                else
                {
                    //Debug.Log("有问题");
                }
            }
        }
        else
        {
            if (haultedCounter > 0)
            {
                haultedCounter -= Time.deltaTime;
            }
            else
            {
                isHaulted = false;
            }
        }
    }
    private void MoveablePlatform_SensorUpdate()
    {
        //计时部分
        if (hasArrived)
        {
            if (hasArrivedCounter > 0)
            {
                if (thePlayer == null)
                {
                    hasArrivedCounter -= Time.deltaTime;
                }
                else
                {
                    hasArrivedCounter = hasArrivedDuration;
                }
            }
            else
            {
                SetStartDestination();
            }
        }
        else
        {
            if (isDestinationOrienting)
            {
                //Debug.Log("正常上升")
            }
            else
            {
                //Debug.Log("正常下降")
            }
        }

        //运动判断部分
        if (!isHaulted)
        {
            if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
            {
                if (Vector2.Distance(theStartPoint.position, theNowPoint.position) < .01f)//回到起点
                {
                    if (!hasOringinPosed)
                    {
                        offsetVec = theStartPoint.position - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                        hasOringinPosed = true;
                    }
                    else
                    {
                        //Debug.Log("感应中");

                    }
                }
                else if (Vector2.Distance(theEndPoint.position, theNowPoint.position) < .01F)//到达目的地
                {
                    HssArrivedCount();
                }
            }
            else
            {
                if (hasArrivedCounter <= 0)
                {
                    offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                    theNowPoint.position += offsetVec;
                    transform.position += offsetVec;
                    if (thePlayer != null)
                    {
                        //thePlayer.ClearYVelocity();
                        thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        //Debug.Log(thePlayer.horizontalInputVec);
                        //Debug.Log(thePlayer.thisRB.velocity.x);
                    }
                }
            }

        }
        else
        {
            if (haultedCounter > 0)
            {
                haultedCounter -= Time.deltaTime;
            }
            else
            {
                isHaulted = false;
            }
        }
    }
    private void MoveablePlatform_HandlerUpdate()
    {

        switch (handlerInput)
        {
            case 1:
                if (!hasArrived)
                {
                    hasOringinPosed = false;
                    if (Vector2.Distance(theNowPoint.position, theEndPoint.position) < .01F)
                    {
                        //当平台到达终点后，计算偏移量后加在平台本身和偏移量计算点上，实现两者的重置
                        offsetVec = theEndPoint.position - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                        hasArrived = true;
                    }
                    else
                    {
                        offsetVec = Vector3.MoveTowards(theNowPoint.position, theEndPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                    }


                }
                else
                {
                    Debug.Log("已经到顶了");
                }
                break;
            case -1:
                if (!hasOringinPosed)
                {
                    hasArrived = false;
                    if (Vector2.Distance(theNowPoint.position, theStartPoint.position) < .01F)
                    {
                        //当平台到达终点后，计算偏移量后加在平台本身和偏移量计算点上，实现两者的重置
                        offsetVec = theStartPoint.position - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                        hasOringinPosed = true;
                    }
                    else
                    {
                        offsetVec = Vector3.MoveTowards(theNowPoint.position, theStartPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                        theNowPoint.position += offsetVec;
                        transform.position += offsetVec;
                        if (thePlayer != null)
                        {
                            thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                        }
                    }
                }
                else
                {
                    Debug.Log("已经到底了");
                }
                break;
            case 0:
                if (!hasOringinPosed)
                {
                    hasArrived = false;
                    if (perBackStepCounter >= 0)
                    {
                        perBackStepCounter -= Time.deltaTime;
                    }
                    else
                    {
                        if (Vector2.Distance(theNowPoint.position, theStartPoint.position) < .01F)
                        {
                            //Debug.Log(Vector2.Distance(theNowPoint.position, theStartPoint.position));
                            //当平台到达终点后，计算偏移量后加在平台本身和偏移量计算点上，实现两者的重置
                            offsetVec = theStartPoint.position - theNowPoint.position;
                            theNowPoint.position += offsetVec;
                            transform.position += offsetVec;
                            if (thePlayer != null)
                            {
                                thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                            }
                            hasOringinPosed = true;
                        }
                        else
                        {
                            offsetVec = Vector3.MoveTowards(theNowPoint.position, theStartPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                            theNowPoint.position += offsetVec;
                            transform.position += offsetVec;
                            if (thePlayer != null)
                            {
                                thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                            }
                        }
                        perBackStepCounter = perBackStepDuration;
                    }
                }
                else
                {
                    //Debug.Log("感应中");
                }
                break;
        }
    }
    private void DisappearPlatform_SensorUpdate()
    {
        if (needToDisappear)
        {
            if (disappearCounter > 0)
            {
                disappearCounter -= Time.deltaTime;
            }
            else
            {
                HideThisPlatform();
                canDisappearOrReappear = false;
                needToDisappear = false;
                ReappearCount();
            }
        }
        else if (needToReappear)
        {
            if (reappearCounter > 0)
            {
                reappearCounter -= Time.deltaTime;
            }
            else
            {
                ReappearThisPlatform();
                needToReappear = false;
                canDisappearOrReappear = true;
            }
        }
        else
        {
            //Debug.Log("感应中");
        }
    }
    private void DisappearPlatform_RegularUpdate()
    {
        if (needToDisappear)
        {
            if (disappearCounter > 0)
            {
                disappearCounter -= Time.deltaTime;
            }
            else
            {
                foreach (PlatformUnit unit in units)
                {
                    unit.Hide();
                }
                needToDisappear = false;
                ReappearCount();
            }
        }
        else if (needToReappear)
        {
            if (reappearCounter > 0)
            {
                reappearCounter -= Time.deltaTime;
            }
            else
            {
                foreach (PlatformUnit unit in units)
                {
                    unit.Appear();
                }
                needToReappear = false;
                DisappearCount();
            }
        }
        else
        {
            Debug.Log("有问题");
        }
    }
    private void RotateablePlatform_AttackUpdate()
    {
        if (isRotating)
        {
            transform.RotateAround(theRotatePovit_ElevatorPoint.position, Vector3.forward, isClockwise * rotateStep * Time.deltaTime);
            hadRotated += rotateStep * Time.deltaTime;
            if (hadRotated > 90f)
            {
                isRotating = false;
                nowAngle += isClockwise * 90f;
                transform.RotateAround(theRotatePovit_ElevatorPoint.position, Vector3.forward, (-hadRotated + 90f) * isClockwise * Time.deltaTime);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                AntiClockwiseRotate();
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                ClockwiseRotate();
            }
        }
    }
    private void SwitchablePlatform_ElevatorUpdate()
    {
        if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
        {
            if (Vector2.Distance(theStartPoint.position, theNowPoint.position) < .01f)
            {
                offsetVec = theStartPoint.position - theNowPoint.position;
                theNowPoint.position = theStartPoint.position;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                }
                else
                {
                    //Debug.Log("没人啊");
                }
            }
            else if (Vector2.Distance(theEndPoint.position, theNowPoint.position) < .01F)
            {
                offsetVec = theEndPoint.position - theNowPoint.position;
                theNowPoint.position = theEndPoint.position;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                }
                else
                {
                    //Debug.Log("没人啊");
                }
            }
            else if (Vector2.Distance(theRotatePovit_ElevatorPoint.position, theNowPoint.position) < .01F)
            {
                offsetVec = theRotatePovit_ElevatorPoint.position - theNowPoint.position;
                theNowPoint.position = theRotatePovit_ElevatorPoint.position;
                transform.position += offsetVec;
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                }
                else
                {
                    //Debug.Log("没人啊");
                }
            }
            else
            {
                Debug.Log("有问题");
            }
            hasArrived = true;

        }
        else
        {
            offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
            theNowPoint.position += offsetVec;
            transform.position += offsetVec;
            if (thePlayer != null)
            {
                thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
            }
            else
            {
                //Debug.Log("没人啊");
            }
        }
    }
    #endregion

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
    public void HideThisPlatform()
    {
        foreach (PlatformUnit unit in units)
        {
            unit.Hide();
        }
    }

    public void ReappearThisPlatform()
    {
        foreach (PlatformUnit unit in units)
        {
            unit.Appear();
        }
    }
    public void ClockwiseRotate()
    {
        isRotating = true;
        rotateStep = 90f / rotationDuration;
        isClockwise = 1;
        hadRotated = 0;
    }

    public void AntiClockwiseRotate()
    {
        isRotating = true;
        rotateStep = 90f / rotationDuration;
        isClockwise = -1;
        hadRotated = 0;
    }
    public void CallThisPlatform(Transform _destinationPoint)
    {
        theDestinalPoint.position = _destinationPoint.position;
    }

    public bool WhetherHasArrived()
    {
        return hasArrived;
    }
    public void Interactive_Sensor()
    {
        switch (thisPlatformType)
        {

            case PlatformInteractiveType.disappearable_sensor:
                //Debug.Log("接触了");
                DisappearCount();
                break;
            case PlatformInteractiveType.moveable_sensor:
                HaulThisPlatform();
                if (hasArrived)
                {
                    hasArrivedCounter = hasArrivedDuration;
                }
                else
                {
                    Activate();
                }
                break;
        }
    }
    private void Activate()
    {
        isDestinationOrienting = true;
        theDestinalPoint.position = theEndPoint.position;
    }

    private void SetStartDestination()
    {
        hasArrived = false;
        theDestinalPoint.position = theStartPoint.position;
    }

    private void HssArrivedCount()
    {
        if (isDestinationOrienting)
        {
            hasArrived = true;
            hasArrivedCounter = hasArrivedDuration;
            isDestinationOrienting = false;
        }
    }


    private void DisappearCount()
    {
        needToDisappear = true;
        disappearCounter = disappearDuration;
    }
    private void ReappearCount()
    {
        needToReappear = true;
        reappearCounter = reappearDuration;
    }


    private void HaulThisPlatform()
    {
        if (canBeHaulted)
        {
            isHaulted = true;
            haultedCounter = haultedDuration;
        }
    }


    #endregion

}
