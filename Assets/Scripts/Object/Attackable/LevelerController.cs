using AttackableInterfaces;
using InteractiveAndInteractableEnums;
using IPhysicalAttackableFactoryRelated;
using UnityEngine;

public class LevelerController : MonoBehaviour
{
    #region 组件

    private Animator thisAnim;
    public CombineInteractableManager theCombineManager { get; private set; }
    #endregion



    [Header("————使用说明————\n1、选择本脚本适用的操纵杆类型，即thisLevelerType")]
    [Space(3)]
    #region 变量
    [Header("Leveler Setting")]
    public AttackableConfigSO attackConfigSO;
    public levelerType thisLevelerType;
    public LevelerFactory thisFactory;

    [Header("Leveler Info")]
    public bool isInteracted;
    public bool canBeInteracted;
    private IPhysicalAttackable currentLeveler;

    [Header("Rotater Related")]//涉及对象为定向旋转平台，包含顺时针旋转、逆时针旋转、停止三种状态
    [Header("2、添加旋转平台对象，即rotatePlatforms\n3、添加复位时间")]
    public PlatformController[] rotatePlatforms;
    private float canBeInteractedCounter;



    [Header("Elevator Related")]//涉及对象为电梯，包含上升、下降、停止三种状态
    [Header("2、添加电梯对象，elevatorPlatform\n3、将本对象设为电梯的子对象")]
    public PlatformController elevatorPlatform;




    #endregion


    #region 初始化相关
    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        theCombineManager = GetComponentInParent<CombineInteractableManager>();//获取父类主要是实现多对象之间的同步
        switch (thisLevelerType)
        {
            case levelerType.attackable_rotater:
                thisFactory = new RotaterFactory();
                break;
            case levelerType.attackable_elevator:

                thisFactory = new ElevatorFactory();
                break;
            default:
                break;

        }
        currentLeveler = thisFactory?.CreateLeveler(this);
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
                thisAnim.SetBool(attackConfigSO.Leveler_CANBEINTERACTED, true);
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
        if (other.TryGetComponent<AttackArea>(out AttackArea attackArea))
        {
            currentLeveler?.BePhysicalAttacked(attackArea);

        }
    }



    #endregion


    #region 小方法与外部调用


    public void JustInteract()
    {
        thisAnim.SetTrigger(attackConfigSO.Leveler_ISINTERACTINGSTR);
        SetInteracted();
    }

    public void JustAltInteract()
    {
        thisAnim.SetBool(attackConfigSO.Leveler_CANBEINTERACTED, false);
        SetInteracted();
    }

    private void SetInteracted()
    {
        thisAnim.SetTrigger(attackConfigSO.Leveler_ISALTINTERACTINGSTR);
        canBeInteracted = false;
        canBeInteractedCounter = attackConfigSO.Leveler_canBeInteractedDuration;
    }

    #endregion
}
