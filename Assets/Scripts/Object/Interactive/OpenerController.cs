using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using InteractiveAndInteractableEnums;

public class OpenerController:MonoBehaviour
{
    #region 组件
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;
    #endregion
    [Header("————使用说明————\n1、选择本脚本适用的开门器类型，thisOpenerType\n2、添加相应的门对象\n3、根据种类同进行设定，详情看对应Related")]
    [Space(3)]

    [Header("Opener Setting")]
    public openerType thisOpenerType;
    public DoorController theDoor;
    [Header("Opener Info")]
    public bool isPressed;



    [Header("Button Related")]
    public AnimatorController theButtonAnimator;
    [Header("Debris Related")]
    public AnimatorController theDebrisAnimator;
    [Header("Key Related")]
    public AnimatorController theKeyAnimator;
    public GameObject theKeyPrefab;
    [Header("Hider Related")]
    //要用单独的动画
    //public AnimatorController theHiderAnimator;
    public PlatformController theHidePlatform;
    public float ReappearCounter;
    public float ReappearDuration;

    [Header("Animator Related")]
    private const string PRESSEDSTR = "isPressed";
    private const string UNPRESSEDSTR = "isUnpressed";
    private const string PRESSINGSTR = "isPressing";
    private const string UNPRESSINGSTR = "isUnpressing";



    private void Awake()
    {
        thisAnim = GetComponentInChildren<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
    }


    private void Start()
    {
        switch (thisOpenerType)
        {
            case openerType.trigerable_button:
                Opener_ButtonStart();
                break;
            case openerType.triggerable_debris:
                Opener_DebrisStart();
                break;
            case openerType.triggerable_key:
                Opener_KeyStart();
                break;
            case openerType.autoresetable_hider:
                Opener_HiderStart();
                break;
        }
    }

    #region 初始化相关

    private void Opener_ButtonStart()
    {
        thisAnim.runtimeAnimatorController = theButtonAnimator;

        //读取存档
        if (isPressed)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(PRESSEDSTR, true);
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(UNPRESSEDSTR, true);
        }
    }

    private void Opener_DebrisStart()
    {
        Debug.Log("运行了吗？");
        thisAnim.runtimeAnimatorController = theDebrisAnimator;
        thisBoxCol.enabled = true;
        thisAnim.SetBool(UNPRESSEDSTR, true);
    }

    private void Opener_KeyStart()
    {
        thisAnim.runtimeAnimatorController = theKeyAnimator;
        if (isPressed)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(PRESSEDSTR, true);
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(UNPRESSEDSTR, true);
        }
    }

    private void Opener_HiderStart()
    {
        //thisAnim.runtimeAnimatorController = theHiderAnimator;
        thisAnim.runtimeAnimatorController = theButtonAnimator;
        thisBoxCol.enabled = true;
        thisAnim.SetBool(UNPRESSEDSTR, true);

    }

    #endregion

    private void Update()
    {
        switch (thisOpenerType)
        {
            case openerType.trigerable_button:
                break;
            case openerType.triggerable_debris:
                break;
            case openerType.triggerable_key:
                break;
            case openerType.autoresetable_hider:
                Opener_HiderUpdate();
                break;
        }
    }
    #region Update相关

    private void Opener_HiderUpdate()
    {

        if (ReappearCounter > 0)
        {
            ReappearCounter -= Time.deltaTime;
        }
        else
        {
            isPressed = false;
            thisAnim.SetTrigger(UNPRESSINGSTR);
            theHidePlatform.ReappearThisPlatform();

        }
    }
    #endregion


    #region Press相关
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            switch (thisOpenerType)
            {
                case openerType.trigerable_button:
                    Opener_ButtonPress();
                    break;
                case openerType.triggerable_debris:
                    Opener_DebrisPress();
                    break;
                case openerType.triggerable_key:
                    Opener_KeyPress();
                    break;
                case openerType.autoresetable_hider:
                    Opener_HiderPress();
                    break;
            }
        }
    }

    private void Opener_DebrisPress()
    {
        if (!isPressed)
        {
            isPressed = true;
            thisAnim.SetTrigger(PRESSINGSTR);
            theDoor.OpenTheDoor();
        }
    }

    private void Opener_ButtonPress()
    {
        if (!isPressed)
        {
            isPressed = true;
            thisAnim.SetTrigger(PRESSINGSTR);
            theDoor.OpenTheDoor();
        } 

    }

    private void Opener_KeyPress()
    {
        if (!isPressed)
        {
            isPressed = true;
            thisAnim.SetTrigger(PRESSINGSTR);
            Instantiate(theKeyPrefab, transform.position, Quaternion.identity);
        }
    }

    private void Opener_HiderPress()
    {
        if (!isPressed)
        {
            Debug.Log("xiaoshi");
            isPressed = true;
            thisAnim.SetTrigger(PRESSINGSTR);
            theHidePlatform.HideThisPlatform();
            ReappearCounter = ReappearDuration;
        }
    }

    #endregion



    public void Opener_DebrisReset()
    {
        isPressed = false;
        thisAnim.SetTrigger(UNPRESSINGSTR);
    }
}
