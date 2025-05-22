using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveAndInteractableEnums;
using PlayerInterfaces;

public class SwitchController : MonoBehaviour,IInteract
{
    #region 组件
    private Animator thisAnim;
    private CombineInteractableManager theCombineManager;
    #endregion



    [Header("————使用说明————\n1、选择本脚本适用的开关类型，即thisSwitchType\n2、设定该对象是否初始状态is Triggered及是否是主要开关is Primary Switch")]
    [Space(3)]
    #region 变量
    [Header("Swtich Setting")]
    public InteractableConfigSO interactableConfigSO;
    public switchTpye thisSwitchType;
    public bool isPrimarySwitch;//通过此项来判断用于初始化的对象范围，避免重复及冲突

    [Header("Switch Info")]
    public bool isTriggered;
    public bool canTriggered;

    [Header("Alternative Related")]//涉及对象只有开关对应的两种状态逻辑
    [Header("3、添加复位时间，即canTriggeredDuration\n4、添加二择开关状态相应对象")]

    private float canTriggeredCounter;
    public PlatformController[] triggeredPlatforms;
    public PlatformController[] unTriggeredPlatforms;

    [Header("Autoresetable Related")]//涉及对象只有一种状态逻辑，但是逻辑中可能存在多个可控结果
    [Header("3、添加该开关的对应电梯位置，即thisElevatorArrivalPoint\n4、添加电梯对象")]
    public Transform thisElevatorArrivalPoint;
    public PlatformController theElevator;




    #endregion

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        theCombineManager = GetComponentInParent<CombineInteractableManager>();//主要用于同步多个switch的状态
        SceneLoadSetting_PlatformControllers();//对于Switch的涉及对象进行初始化，放在Awake主要是在PlaytformUnit会在Start根据Platform的内容也进行初始化，所以这个必须在Start之前

    }

    void Start()
    {
        SceneLoadSetting_SwitchItself();//对于Switch本身可以在Start进行初始化

    }

    #region 初始化相关
    private void SceneLoadSetting_PlatformControllers()//pair要对开关状态进行区分，但是elevator不用考虑这个，保持该格式仅为了未来拓展性和格式一致性
    {
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                Alternative_PairAwake();//即使有个primary也只是用于对平台的初始化，而其他的二择开关本身一就要自行设置初始或者根据存档
                break;

        }
    }

    private void Alternative_PairAwake()
    {
        if (isPrimarySwitch)
        {
            if (isTriggered)
            {
                foreach (PlatformController triggeredPlatform in triggeredPlatforms)
                {
                    triggeredPlatform.isHidden = false;
                }
                foreach (PlatformController triggeredPlatform in unTriggeredPlatforms)
                {
                    triggeredPlatform.isHidden = true;
                }
            }
            else
            {
                foreach (PlatformController triggeredPlatform in triggeredPlatforms)
                {
                    triggeredPlatform.isHidden = true;
                }
                foreach (PlatformController triggeredPlatform in unTriggeredPlatforms)
                {
                    triggeredPlatform.isHidden = false;
                }
            }
        }
    }

    private void SceneLoadSetting_SwitchItself()//二择开关本身一就要自行设置初始或者根据存档，这里仅仅是动画同步。=
    {
        //需要用存档记录
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                Alternative_PairStart();
                break;
            case switchTpye.autoresetable_elevator:
                AutoResetable_ElevatorStart();
                break;

        }
    }

    private void Alternative_PairStart()//
    {
        if (isTriggered)
        {
            thisAnim.SetBool(interactableConfigSO.Switch_TRIGGEREDSTR, true);
        }
        else
        {
            thisAnim.SetBool(interactableConfigSO.Switch_UNTRIGGEREDSTR, true);
        }
    }

    private void AutoResetable_ElevatorStart()
    {
        thisAnim.SetBool(interactableConfigSO.Switch_UNTRIGGEREDSTR, true);
    }

    #endregion

    #region Update相关
    void Update()
    {
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                Alternative_PairUpdate();//交互之后固定时间复位
                break;
            case switchTpye.autoresetable_elevator:
                AutoResetable_ElevatorUpdate();//交互后检测到达后复位
                break;



        }
    }
    private void Alternative_PairUpdate()
    {
        if (!canTriggered)
        {
            if (canTriggeredCounter > 0)
            {
                canTriggeredCounter -= Time.deltaTime;
            }
            else
            {
                canTriggered = true;
            }
        }
        else
        {
            //Debug.Log("正常运作");
        }
    }

    private void AutoResetable_ElevatorUpdate()
    {
        if (isPrimarySwitch && theElevator.hasArrived)
        {
            theCombineManager.SwitchReset();
        }
    }

    #endregion

    #region 接口相关

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            if (other.GetComponent<NewPlayerController>().theInteractable==null)
            {
                //Debug.Log("加入交互对象");
                other.GetComponent<NewPlayerController>().theInteractable = this;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            if (other.GetComponent<NewPlayerController>().theInteractable == this.GetComponent<IInteract>())
            {
                other.GetComponent<NewPlayerController>().theInteractable = null;
                //Debug.Log("移除交互对象");
            }

        }
    }
    #endregion

    #region 小方法与外部调用
    public void hadAutoReset()//用于elevator到达后各开关复位
    {
        canTriggered = true;
        isTriggered = false;
        thisAnim.SetTrigger(interactableConfigSO.Switch_UNTRIGGERINGSTR);
    }
    public void Interact()//根据不同类型进行不同的交互内容
    {
        switch (thisSwitchType) 
        {
            case switchTpye.alternative_pair:
                Alternative_PairInteract();//进行二择平台的状态转换
                break;
            case switchTpye.autoresetable_elevator:
                AutoResetable_ElevatorInteract();//呼叫电梯到达
                break;
        }
    }
    private void Alternative_PairInteract()
    {
        if (canTriggered)
        {
            if (isTriggered)
            {
                foreach (PlatformController _triggeredPlatform in triggeredPlatforms)
                {
                    _triggeredPlatform.currentPlatform.Interact1();
                }
                foreach (PlatformController _triggeredPlatform in unTriggeredPlatforms)
                {
                    _triggeredPlatform.currentPlatform.Interact2();
                }

            }
            else
            {
  
                foreach (PlatformController _triggeredPlatform in triggeredPlatforms)
                {
                    _triggeredPlatform.currentPlatform.Interact2();
                }
                foreach (PlatformController _triggeredPlatform in unTriggeredPlatforms)
                {
                    _triggeredPlatform.currentPlatform.Interact1();
                }

            }
            theCombineManager.SwitchsTrigger();
        }
    }

    private void AutoResetable_ElevatorInteract()
    {
        if (canTriggered)
        {
            if (!isTriggered)
            {
                //Debug.Log("电梯快来！");
                theElevator.CallThisPlatform(thisElevatorArrivalPoint);
                theCombineManager.SwitchsTrigger();
            }
            else
            {
                //Debug.Log("暂时无法再交互");
            }
        }
    }


    public void JustTrigger()//用于某开关后各个开关的的同步
    {
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                Alternative_PairJustTrigger();
                break;
            case switchTpye.autoresetable_elevator:
                AutoResetable_ElevatorJustTrigger();
                break;

        }


    }

    private void Alternative_PairJustTrigger()
    {
        if (isTriggered)
        {
            isTriggered = false;
            thisAnim.SetTrigger(interactableConfigSO.Switch_UNTRIGGERINGSTR);
        }
        else
        {
            isTriggered = true;
            thisAnim.SetTrigger(interactableConfigSO.Switch_TRIGGERINGSTR);
        }
        canTriggered = false;
        canTriggeredCounter = interactableConfigSO.canTriggeredDuration;
    }

    private void AutoResetable_ElevatorJustTrigger()
    {
        isTriggered = true;
        canTriggered = false;
        thisAnim.SetTrigger(interactableConfigSO.Switch_TRIGGERINGSTR);

    }

    #endregion

}
