using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour,IInteract
{
    #region 组件及其他
    [HideInInspector]public enum SwitchTpye { swichabelPlatform_pair}
    private Animator thisAnim;
    private CombineInteractableManager theCombineManager;
    #endregion
    [Header("————使用说明————\n1、选择本脚本适用的开关类型，即thisSwitchType\n2、根据设计在数组中添加开关状态下对应的对象\n3、设定该对象是否初始状态is Triggered及是否是主要开关is Primary Switch")]
    [Space(3)]

    #region 变量
    [Header("Swtich Setting")]
    public SwitchTpye thisSwitchType;
    public bool isPrimarySwitch;//通过此项来判断用于初始化的对象范围，避免重复及冲突
    public float canInteractedDuration;
    [Header("Switch Info")]
    public bool isTriggered;
    public bool canInteracted;
    private float canInteractedCounter;
    [Header("Combine Related")]
    public PlatformController[] triggeredPlatforms;
    public PlatformController[] unTriggeredPlatforms;
    [Header("Animator Related")]
    private const string TRIGGEREDSTR = "isTriggered";
    private const string UNTRIGGEREDSTR = "isUntriggered";
    private const string TRIGGERINGSTR = "isTriggering";
    private const string UNTRIGGERINGSTR = "isUntriggering";

    #endregion

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        theCombineManager = GetComponentInParent<CombineInteractableManager>();//获取父类主要是实现多对象之间的同步
        SceneLoadSetting_PlatformControllers();//对于Switch的涉及对象进行初始化，放在Awake主要是在PlaytformUnit会在Start根据Platform的内容也进行初始化，所以这个必须在Start之前

    }

    void Start()
    {
        SceneLoadSetting_SwitchItself();//对于Switch本身可以在Start进行初始化
    }

    void Update()//根据开关类型进行不同的Update
    {
        switch (thisSwitchType)
        {
            case SwitchTpye.swichabelPlatform_pair:
                SwitchablePlatform_PairUpdate();
                break;

        }
    }
    private void FixedUpdate()
    {
        
    }

    #region 初始化相关
    private void SceneLoadSetting_SwitchItself()//根据开关类型和设计内容对本身内容进行初始化
    {
        //需要用存档记录
        if (isTriggered)
        {
            thisAnim.SetBool(TRIGGEREDSTR, true);
            //thisAnim.SetBool(UNTRIGGEREDSTR, false);
        }
        else
        {
            //thisAnim.SetBool(TRIGGEREDSTR, false);
            thisAnim.SetBool(UNTRIGGEREDSTR, true);
        }
    }

    private void SceneLoadSetting_PlatformControllers()//根据开关类型和设计内容对所有的涉及对象进行初始化
    {
        switch (thisSwitchType)
        {
            case SwitchTpye.swichabelPlatform_pair:
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
                            triggeredPlatform.isHidden =true;
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
                break;

        }
    }
    #endregion

    #region Update相关
    private void SwitchablePlatform_PairUpdate()
    {
        if (!canInteracted)
        {
            if (canInteractedCounter>0)
            {
                canInteractedCounter -= Time.deltaTime;
            }
            else
            {
                canInteracted = true;
            }

        }
        else
        {

            //Debug.Log("正常运作");
        }
    }
    #endregion

    #region Interact接口相关
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            if (other.GetComponent<NewPlayerController>().theInteractable==null&& other.GetComponent<NewPlayerController>())
            {
                //Debug.Log("加入交互对象");
                other.GetComponent<NewPlayerController>().theInteractable = this;
                //显示交互指示
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>().theInteractable == this.GetComponent<IInteract>())
        {
            other.GetComponent<NewPlayerController>().theInteractable = null;
            //Debug.Log("移除交互对象");
        }
    }
    public void Interact()//根据不同类型进行不同的交互内容
    {
        switch (thisSwitchType) 
        {
            case SwitchTpye.swichabelPlatform_pair:
                //Debug.Log("交互1");
                SwitchablePlatform_PairInteract();
                break;
        }
    }
    private void SwitchablePlatform_PairInteract()
    {
        if (canInteracted)
        {
            if (isTriggered)
            {
                foreach (PlatformController _triggeredPlatform in triggeredPlatforms)
                {
                    _triggeredPlatform.HideThisPlatform();
                }
                foreach (PlatformController _triggeredPlatform in unTriggeredPlatforms)
                {
                    _triggeredPlatform.ReappearThisPlatform();
                }

            }
            else
            {
  
                foreach (PlatformController triggeredPlatform in triggeredPlatforms)
                {
                    triggeredPlatform.ReappearThisPlatform();
                }
                foreach (PlatformController triggeredPlatform in unTriggeredPlatforms)
                {
                    triggeredPlatform.HideThisPlatform();
                }

            }
            theCombineManager.SwitchsTrigger();
        }
    }
















    #endregion

    #region 用于外部调用
    public void JustTrigger()
    {
        if (isTriggered)
        {
            isTriggered = false;
            thisAnim.SetTrigger(UNTRIGGERINGSTR);
        }
        else
        {
            isTriggered = true;
            thisAnim.SetTrigger(TRIGGERINGSTR);
        }
        canInteracted = false;
        canInteractedCounter = canInteractedDuration;
    }
    #endregion

}
