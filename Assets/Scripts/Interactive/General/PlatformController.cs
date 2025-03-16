using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D),typeof(CompositeCollider2D))]
public class PlatformController : MonoBehaviour
{
    #region 组件
    [HideInInspector]public enum PlatformInteractiveType{ disappearable_sensor, disappearable_regular, switchable_pair, movable_single,moveable_round,moveable_sensor,adjustable,rotateable}
    private NewPlayerController thePlayer;
    private CompositeCollider2D thisComCol;
    private Rigidbody2D thisRB;

    #endregion

    #region 变量
    [Header("————使用说明————\n1、选择本脚本适用的开关类型，即thisPlatformType\n2、设定该对象是否初始状态is Hidden")]
    [Space(3)]


    [Header("Platform Setting")]
    public PlatformInteractiveType thisPlatformType;

    [Header("Platform Info")]
    public bool isHidden;

    [Header("Units Related")]
    private List<PlatformUnit> units = new List<PlatformUnit>();

    [Header("Movable Related")]
    public float moveSpeed;
    public Transform theStartPoint;
    public Transform theEndPoint;
    public Transform theNowPoint;
    public Transform theDestinalPoint;
    public bool isDestinationOrienting;
    public bool hasArrived;
    public float hasArrivedCounter;
    public float hasArrivedDuration;
    public Vector3 offsetVec;
    public bool isHorizontalMove;

    public bool isHault;
    public float haultCounter;
    public float haultDuration;



    [Header("Disappear Related")]
    public bool canDisappearOrReappear;//用于防止在Player落下的时候刚触发但平台碰撞体还没消失的时候又触发一次消失
    public bool needToDisappear;
    public bool needToReappear;
    public float disappearCounter;
    public float disappearDuration;
    public float reappearCounter;
    public float reappearDuration;
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
            case PlatformInteractiveType.movable_single://可移动平台，单向移动+重置
                MoveablePlatform_SingleStart();
                break;
            case PlatformInteractiveType.moveable_round://可移动平台，往返移动
                MoveablePlatform_RoundStart();
                break;
            case PlatformInteractiveType.moveable_sensor://可移动平台，即踩上后马上向目标地点移动，玩家离开一定时间后重置
                MoveablePlatform_SensorStart();
                break;
            case PlatformInteractiveType.disappearable_sensor://消失平台的默认，即踩上后短暂时间消失，一定时间后重置
;               DisappearPlatform_SensorStart();
                break;
            case PlatformInteractiveType.disappearable_regular://消失平台，往复消失
                DisappearPlatform_RegularStart();
                break;
            case PlatformInteractiveType.adjustable:

                break;
            case PlatformInteractiveType.rotateable:

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
            case PlatformInteractiveType.movable_single://可移动平台的第二种，单向移动+重置
                MoveablePlatform_SingleUpdate();
                break;
            case PlatformInteractiveType.moveable_round://可移动平台的默认，往返移动
                MoveablePlatform_RoundUpdate();
                break;
            case PlatformInteractiveType.moveable_sensor:
                MoveablePlatform_SensorUpdate();
                
                break;
            case PlatformInteractiveType.disappearable_sensor://消失平台的默认，即踩上后短暂时间消失，一定时间后重置
                DisappearPlatform_SensorUpdate();
                break;
            case PlatformInteractiveType.disappearable_regular:
                DisappearPlatform_RegularUpdate();
                break;
            case PlatformInteractiveType.adjustable:

            case PlatformInteractiveType.rotateable:

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


    #region Interactive相关

    #region Interactive_Moveable
    //Moveable需要获取Player，使player跟随
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.GetComponent<NewPlayerController>())
    //    {
    //        if (other.gameObject.GetComponent<NewPlayerController>().thisPR.theRaycastCol.collider == thisComCol)
    //        {
    //            Debug.Log("进入");
    //            thePlayer = other.GetComponent<NewPlayerController>();
    //        }

    //    }
    //}
    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.GetComponent<NewPlayerController>())
    //    {
    //        Debug.Log("退出");
    //        thePlayer = null;
    //    }
    //}
    #endregion

    #region Interactive_sensor
    public void Interactive_Sensor()
    {
        switch (thisPlatformType)
        {
            case PlatformInteractiveType.disappearable_sensor:
                //Debug.Log("接触了");
                DisappearCount();
                break;
            case PlatformInteractiveType.moveable_sensor:
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

    #endregion

    #endregion

    #region 初始化相关
    private void MoveablePlatform_SingleStart()
    {
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theDestinalPoint.position = theEndPoint.position;
    }
    private void MoveablePlatform_RoundStart()
    {
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
        theDestinalPoint.position = theEndPoint.position;
    }
    private void MoveablePlatform_SensorStart()
    {
        theStartPoint.transform.parent = null;
        theEndPoint.transform.parent = null;
        theNowPoint.transform.parent = null;
        theDestinalPoint.transform.parent = null;
    }

    
    private void DisappearPlatform_SensorStart()
    {
        canDisappearOrReappear = true;
    }

    private void DisappearPlatform_RegularStart()
    {
        DisappearCount();
        canDisappearOrReappear = true;
    }


    #endregion

    #region 各种平台Update逻辑
    private void MoveablePlatform_SingleUpdate()
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
                //Debug.Log(thePlayer.horizontalInputVec);
                //Debug.Log(thePlayer.thisRB.velocity.x);
            }
        }
    }

    private void MoveablePlatform_RoundUpdate()
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
            thisRB.MovePosition(transform.position += offsetVec);
            if (thePlayer != null)
            {
                //thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                thePlayer.transform.position += offsetVec;
                //Debug.Log(thePlayer.horizontalInputVec);
                //Debug.Log(thePlayer.thisRB.velocity.x);
            }
            else
            {
                //Debug.Log("有问题");
            }
        }
    }
    private void MoveablePlatform_SensorUpdate()
    {
        //计时部分
        if (hasArrived)
        {
            if(hasArrivedCounter>0)
            {
                if (thePlayer == null)
                {
                    hasArrivedCounter -=Time.deltaTime;
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
        if (Vector2.Distance(theNowPoint.position, theDestinalPoint.position) < .01F)
        {
            if (Vector2.Distance(theStartPoint.position, theNowPoint.position) < .01f)//回到起点
            {
                //Debug.Log("感应中");
            }
            else if (Vector2.Distance(theEndPoint.position, theNowPoint.position) < .01F)//到达目的地
            {
                HasArrivedCount();
            }
        }
        else
        {
            if (hasArrivedCounter <= 0)
            {
                offsetVec = Vector3.MoveTowards(theNowPoint.position, theDestinalPoint.position, moveSpeed * Time.deltaTime) - theNowPoint.position;
                theNowPoint.position += offsetVec;
                thisRB.MovePosition(transform.position += offsetVec);
                if (thePlayer != null)
                {
                    thePlayer.transform.position += offsetVec + (Vector3)thePlayer.thisRB.velocity * Time.deltaTime;
                    //Debug.Log(thePlayer.horizontalInputVec);
                    //Debug.Log(thePlayer.thisRB.velocity.x);
                }
            }
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

    #endregion

    #region 各种平台FixedUpdate逻辑


    #endregion

    #region 各种小方法

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
    public void Activate()
    {
        isDestinationOrienting = true;
        theDestinalPoint.position = theEndPoint.position;

    }

    public void SetStartDestination()
    {
        hasArrived = false;
        theDestinalPoint.position = theStartPoint.position;
    }

    public void HasArrivedCount()
    {
        if (isDestinationOrienting)
        {
            hasArrived = true;
            hasArrivedCounter = hasArrivedDuration;
            isDestinationOrienting = false;
        }
    }

    public void DisappearCount()
    {
        needToDisappear = true;
        disappearCounter = disappearDuration;
    }
    private void ReappearCount()
    {
        needToReappear = true;
        reappearCounter = reappearDuration;
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
    #endregion












}
