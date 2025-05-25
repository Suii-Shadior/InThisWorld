using System.Collections;
using SwitchFactoryRelated;
using UnityEngine;
using InteractiveAndInteractableEnums;
using InteractableInterface;

public class SwitchController : MonoBehaviour, IInteract
{
    #region 组件
    private Animator thisAnim;
    public CombineInteractableManager theCombineManager;
    #endregion



    [Header("————使用说明————\n1、选择本脚本适用的开关类型，即thisSwitchType\n2、设定该对象是否初始状态is Triggered及是否是主要开关is Primary Switch")]
    [Space(3)]
    #region 变量
    [Header("Swtich Setting")]
    public InteractableConfigSO interactableConfigSO;
    public switchTpye thisSwitchType;
    public ISwitch currentSwitch;
    public bool isPrimarySwitch;//通过此项来判断用于初始化的对象范围，避免重复及冲突

    [Header("Switch Info")]
    public bool isTriggered;
    public bool canTriggered;

    [Header("Alternative Related")]//涉及对象只有开关对应的两种状态逻辑
    [Header("3、添加复位时间，即canTriggeredDuration\n4、添加二择开关状态相应对象")]

    public float canTriggeredCounter;
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
        //对于Switch的涉及对象进行初始化，放在Awake主要是在PlaytformUnit会在Start根据Platform的内容也进行初始化，所以这个必须在Start之前
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                currentSwitch = new PairerFactory().CreateSwitcher(this);
                break;
            case switchTpye.autoresetable_elevator:
                currentSwitch = new ElevatorSwitchFactory().CreateSwitcher(this);
                break;
        }
        currentSwitch?.SceneLoad_Awake();
    }

    void Start()
    {
        //对于Switch本身可以在Start进行初始化
        //二择开关本身一就要自行设置初始或者根据存档，这里仅仅是动画同步
        currentSwitch?.SceneLoad_Start();
    }


    void Update()
    {
        currentSwitch?.SceneExist_Updata();
    }




    #region 接口相关

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController thePlayer))
        {
            SetPlayer(thePlayer);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController thePlayer))
        {

            ClearPlayer(thePlayer);
        }
    }
    public void SetPlayer(NewPlayerController _thePlayer)
    {
        if (_thePlayer.theInteractable == null)
        {
            //Debug.Log("加入交互对象");
            _thePlayer.theInteractable = this;
        }
    }

    public void ClearPlayer(NewPlayerController _thePlayer)
    {
        if (_thePlayer.theInteractable == this.GetComponent<IInteract>())
        {
            _thePlayer.theInteractable = null;
            //Debug.Log("移除交互对象");
        }
    }
    public void Interact()//根据不同类型进行不同的交互内容
    {
        if (canTriggered)
        {
            if (isTriggered)
            {
                currentSwitch?.Interact1();
            }
            else
            {
                currentSwitch?.Interact2();
            }
        }
    }
    #endregion

    #region 小方法与外部调用




    #endregion

    #region 动画机相关方法
    public void SetAnimTriggered()
    {
        thisAnim.SetBool(interactableConfigSO.Switch_TRIGGEREDSTR, true);
    }

    public void SetAnimUntriggered()
    {
        thisAnim.SetBool(interactableConfigSO.Switch_UNTRIGGEREDSTR, true);
    }

    public void SetAnimTrigger()
    {
        thisAnim.SetTrigger(interactableConfigSO.Switch_TRIGGERINGSTR);
    }


    #endregion
}
