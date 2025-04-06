using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using InteractiveAndInteractableEnums;

public class LevelerController : MonoBehaviour
{
    #region 组件

    private Animator thisAnim;
    private CombineInteractableManager theCombineManager;
    #endregion



    [Header("————使用说明————\n1、选择本脚本适用的操纵杆类型，即thisLevelerType")]
    [Space(3)]
    #region 变量
    [Header("Leveler Setting")]
    public levelerType thisLevelerType;
    [Header("Leveler Info")]
    public bool isInteracted;
    public bool canBeInteracted;

    [Header("Rotater Related")]//涉及对象为定向旋转平台，包含顺时针旋转、逆时针旋转、停止三种状态
    [Header("2、添加旋转平台对象，即rotatePlatforms\n3、添加复位时间")]
    public PlatformController[] rotatePlatforms;
    private float canBeInteractedCounter;
    public float canBeInteractedDuration;


    [Header("Elevator Related")]//涉及对象为电梯，包含上升、下降、停止三种状态
    [Header("2、添加电梯对象，elevatorPlatform\n3、将本对象设为电梯的子对象")]
    public PlatformController elevatorPlatform;



    [Header("Animator Related")]
    private const string ISINTERACTINGSTR = "isInteracting";
    private const string ISALTINTERACTINGSTR = "isAltInteracting";
    private const string CANBEINTERACTED = "canBeInteracted";
    #endregion


    #region 初始化相关
    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        theCombineManager = GetComponentInParent<CombineInteractableManager>();//获取父类主要是实现多对象之间的同步
    }
    private void Start()//Leveler起始只在初始状态，不用进行任何初始化
    {

    }



    #endregion

    #region Update相关
    private void Update()//Leveler即使触发了都会很快复位，仅触发交互让相应主体再自身脚本中运行
    {
        if (!canBeInteracted)
        {
            if (canBeInteractedCounter > 0)
            {
                canBeInteractedCounter -= Time.deltaTime;
            }
            else
            {
                canBeInteracted = true;
                thisAnim.SetBool(CANBEINTERACTED, true);
            }
        }
        else
        {
            //Debug.Log("正常等待");
        }
    }

    #endregion

    #region Interact相关
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AttackArea>())
        {
            switch (thisLevelerType) 
            {
                case levelerType.attackable_rotater:
                    Attackable_RotaterInteract(other.GetComponent<AttackArea>());//根据攻击的方向区分旋转方向
                    break;
                case levelerType.attackable_elevator:
                    Attackable_ElevatorInteract(other.GetComponent<AttackArea>());//根据攻击的方向区分升降
                    break;
            }

        }
    }

    private void Attackable_RotaterInteract(AttackArea _theAttack)
    {
        if (canBeInteracted)
        {
            if (_theAttack.thePlayer.transform.position.x > transform.position.x)
            {
                //Debug.Log("左手一个慢动作");
                ClockwiseRotate();
            }
            else
            {
                //Debug.Log("右手慢动作重播");
                AntiClockwiseRotate();
            }
        }
    }
    private void Attackable_ElevatorInteract(AttackArea _theAttack)
    {
        if (canBeInteracted)
        {
            if (_theAttack.thePlayer.transform.position.x > transform.position.x)
            {
                //Debug.Log("左手一个慢动作");
                ElevatorUpwardMove();
            }
            else
            {
                //Debug.Log("右手慢动作重播");
                ElevaterDownwardMove();
            }
        }
    }

    #endregion


    #region 小方法与外部调用
    private void ClockwiseRotate()
    {
        foreach(PlatformController rotatePlatform in rotatePlatforms)
        {
            rotatePlatform.ClockwiseRotate();
        }
        theCombineManager.LevelersInteract();
    }
    private void AntiClockwiseRotate()
    {
        foreach (PlatformController rotatePlatform in rotatePlatforms)
        {
            rotatePlatform.AntiClockwiseRotate();
        }
        theCombineManager.LevelersAltInteract();
    }
    private void ElevatorUpwardMove()
    {
        if(Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theEndPoint.position) < .01F)
        {
            //Debug.Log("最上面了");
        }
        else if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theRotatePovit_ElevatorPoint.position) < .01F)
        {
            elevatorPlatform.theDestinalPoint.position = elevatorPlatform.theEndPoint.position;
        }
        else if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theStartPoint.position) < .01F)
        {
            elevatorPlatform.theDestinalPoint.position = elevatorPlatform.theRotatePovit_ElevatorPoint.position;
        }
    }
    private void ElevaterDownwardMove()
    {
        if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theEndPoint.position) < .01F)
        {
            elevatorPlatform.theDestinalPoint.position = elevatorPlatform.theRotatePovit_ElevatorPoint.position;
        }
        else if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theRotatePovit_ElevatorPoint.position) < .01F)
        {
            elevatorPlatform.theDestinalPoint.position = elevatorPlatform.theStartPoint.position;
        }
        else if (Vector2.Distance(elevatorPlatform.theNowPoint.position, elevatorPlatform.theStartPoint.position) < .01F)
        {
            Debug.Log("最下面了");
        }
    }
    public void JustInteract()
    {
        thisAnim.SetTrigger(ISINTERACTINGSTR);
        thisAnim.SetBool(CANBEINTERACTED, false);
        canBeInteracted = false;
        canBeInteractedCounter = canBeInteractedDuration;
    }

    public void JustAltInteract()
    {
        thisAnim.SetBool(CANBEINTERACTED, false);
        thisAnim.SetTrigger(ISALTINTERACTINGSTR);
        canBeInteracted = false;
        canBeInteractedCounter = canBeInteractedDuration;
    }
    #endregion
}
