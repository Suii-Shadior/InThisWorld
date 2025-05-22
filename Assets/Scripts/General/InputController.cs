using CMRelatedEnum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class InputController : MonoBehaviour
{

    [Header("将方向键输入从案件注册剥离的原因：\n" +
        "1、方向键输入内容根据不同的情况下往往产生的逻辑作用不同，如果用事件则需要大量的标识进行辅助判断当前应该干什么，放在本脚本里面太过困难且容易变得可读性极差。\n" +
        "2、故方向键的按键统一输入，在需要实现的脚本内进行预处理")]
    #region 组件
    private ControllerManager thisCM;
    private Input1 theInput;
    private NewPlayerController thePlayer;
    private LevelController theLevel;//管理关卡的
    private UIController theUI;
    private DialogeController theDC;
    #endregion

    /*
     * 核心思路：
     * 1、在核心场景加载时启用一个默认InputMap
     * 2、通过按键或按钮使用启用停用函数在不同InputMap间进行切换
     * 
     * 
     * 
     
     */

    #region 变量
    public int horizontalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().x != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().x > 0) ? 1 : -1) : 0;
    public int verticalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().y != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().y > 0) ? 1 : -1) : 0;
    #endregion








    private void Awake()
    {
        /*
         * Work1.获取组件
         * Work2.初始化一个Input对象
         * 
         */
        thisCM = GetComponentInParent<ControllerManager>();
        thePlayer = thisCM.thePlayer;//Tip;根据场景不同可能不能放在这
        theLevel = thisCM.theLevel;
        theUI = thisCM.theUI;
        theDC = thisCM.theDC;

        theInput = new Input1();
    }
    private void OnEnable()
    {
        /*
         * Work1.启用Input对象
         * Work2.根据传来的PlayerPrefs内容确定输入内容//TODO：还没做
         */
        theInput.Enable();
    }
    void Start()
    {
        /*
         * Work1.在核心场景加载时启用一个默认InputMap——可能会变
         * Work2.注册各个InputMap的按键
         */
        GamemenuInput();
        

        GamePlayInputRegister();
        GamePauseInputRegister();
        MainMenuInputRegister();
    }
    

    private void Update()
    {

    }
    private void OnDisable()
    {        
        /*
         * Work1.停用Input对象
         */
        theInput.Disable();
    }



    #region 小方法和外部调用
    public void GamePlayInput()
    {
        //Gameplay阶段的运行输入启用
        theInput.Disable();
        theInput.Gameplay.Enable();
    }
    public void GamepauseInput()
    {
        //Gameplay阶段的暂停输入启用
        theInput.Disable();
        theInput.PauseMenu.Enable();
    }
    public void GamemenuInput()
    {
        //主菜单阶段的运行输入启用
        theInput.Disable();
        theInput.MainMenu.Enable();
    }

    private void GamePlayInputRegister()
    {
        /* Gameplay阶段的按键注册（排除方向键）
         * 
         * 在该阶段的按键注册，通常为玩家行为内容。常用结构：广义行为can判断=>具体行为can判断/分支/兜底
         * Work1.跳跃键按下（跳跃/蹬墙跳），松开（若在起跳过程中，则减少上升惯性）
         * Work1.道具交互1键按下（使用道具交互A）
         * Work1.场景交互键按下（使用当前可激活交互）
         * 
         * Work2.暂停键
         * Work2.地图键
         * 
         * Work3.切换键按下（进入计时），松开(特殊交互/调出，关闭道具选择框） //TODO
         * Work3.道具交互2键按下（使用道具交互B） //TODO
         * Work3.快捷切换键1//TODO
         * Work3.快捷切换键2 //TODO
         * 
         */
        theInput.Gameplay.Jump.started += ctx =>
        {
            if (thePlayer.GetCanAct())
            {
                if (thePlayer.canJump)
                {
                    //Debug.Log("普通跳");
                    thePlayer.ChangeToJumpState();
                    return;
                }
                else if (thePlayer.canWallJump)
                {
                    //Debug.Logs("蹬墙跳）;
                }
                else thePlayer.JumpBufferCheck();
            }
        };
        theInput.Gameplay.Jump.canceled += ctx =>
        {
            if (thePlayer.CurrentState() == thePlayer.jumpState)
            {
                //if (thePlayer.isPastApexThreshold)
                //{
                //    转到Apex状态
                //}
                //else
                //{
                //    转到Fall状态
                //}
                Debug.Log("松了？");
                thePlayer.HalfYVelocity();
            }

        };
        theInput.Gameplay.ItemUse1.started += ctx =>
        {
            if (thePlayer.GetCanAct())
            {
                if (thePlayer.canItemUse1)
                {
                    //在这里调用物品交互1统一接口？
                    if (theInput.Gameplay.Movement.ReadValue<Vector2>().y > 0)
                    {
                        thePlayer.attackSignal = 4;
                    }
                    else if (!thePlayer.thisPR.IsOnFloored() && theInput.Gameplay.Movement.ReadValue<Vector2>().y < 0)
                    {
                        thePlayer.attackSignal = 3;
                    }
                    else
                    {
                        if (thePlayer.sword_ContinueAttackCounter > 0 && thePlayer.attackSignal == 1)
                        {
                            thePlayer.attackSignal = 2;
                        }
                        else
                        {
                            thePlayer.attackSignal = 1;
                        }
                    }
                    thePlayer.thisAC.SetAttackCounter();
                    thePlayer.ChangeToAttackState();

                }
                //Debug.Log("按了");
            }
        };
        theInput.Gameplay.Interact.started += ctx =>
        {
            if (thePlayer.GetCanAct())
            {
                if (thePlayer.canInteract)
                {
                    if (thePlayer.CurrentState() == thePlayer.handleState)
                    {
                        thePlayer.StateOver();//不确定这里有没有风险
                    }
                    else
                    {
                        thePlayer.theInteractable.Interact();
                    }

                }
            }
        };
        theInput.Gameplay.Exchange.started += ctx =>
        {
            //玩家切换
        };
        theInput.Gameplay.Pause.started += ctx =>
        {
            //这里直接调用的UI内按键的统筹方法，主要是为了复用
            if(theInput.Gameplay.enabled)
            {
                //Debug.Log("暂停");
                theUI.FuncCall_GamePlay_Pause();
            }

        };
    }
    private void GamePauseInputRegister()
    {
        theInput.PauseMenu.Resume.started += ctx =>
        {
            //这里直接调用的UI内按键的统筹方法，主要是为了复用
            if (theInput.PauseMenu.enabled)
            {
                //Debug.Log("恢复");
                theUI.FuncCall_GamePause_Resume();
            }
        };
    }
    private void MainMenuInputRegister()
    {
        /* Gamemenu阶段的按键注册
         * 
         * Work1.确认键
         * Work1.退出键
         */
    }

    public void DialoguingInput()
    {
        if (theDC.isPrinting)
        {
            theDC.QuickPrint();
        }
        else
        {
            theDC.NextSentence();
        }
    }

    #region 小方法和外部调用
    public bool WhetherZPressing()
    {
        return theInput.Gameplay.ItemUse1.ReadValue<float>() > .5f;
    }
    public bool WhetherXPressing()
    {
        return theInput.Gameplay.ItemUse2.ReadValue<float>() > .5f;
    }
    public bool WhetherCPressing()
    {
        return theInput.Gameplay.Jump.ReadValue<float>() > .5f;
    }
    public bool WhetherSPressing()
    {
        return theInput.Gameplay.Exchange.ReadValue<float>() > .5f;
    }
    public bool WhetherFPressing()
    {
        return theInput.Gameplay.Interact.ReadValue<float>() > .5f;

    }
    #endregion


    #endregion

}
