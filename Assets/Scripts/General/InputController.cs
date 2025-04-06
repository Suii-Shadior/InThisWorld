using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class InputController : MonoBehaviour
{
    private Input1 theInput;
    private ControllerManager thisCM;//所有Controller的集中管理对象 游戏内个对象可以通过这唯一实例获得各种controller
    //private PlayerController thePlayer;
    private NewPlayerController thePlayer;
    private LevelController theLevel;//管理关卡的
    private UIController theUI;
    private DialogeController theDC;

    public int horizontalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().x != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().x > 0) ? 1 : -1) : 0;
    public int verticalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().y != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().y > 0) ? 1 : -1) : 0;

    public bool jumpWasPressed;//=>theInput.Gameplay.Jump.WasPressedThisFrame();
    public bool jumpIsHeld;//=>theInput.Gameplay.Jump.IsPressed();
    public bool jumpWasReleased;//=>theInput.Gameplay.Jump.WasReleasedThisFrame();






    private void Awake()
    {
        thisCM = GetComponentInParent<ControllerManager>();
        theInput = new Input1();
        thePlayer = thisCM.thePlayer;//Tip;根据场景不同可能不能放在这
        theLevel = thisCM.theLevel;
        theUI = thisCM.theUI;
        theDC = thisCM.theDC;
    }
    private void OnEnable()
    {
        theInput.Enable();
    }
    private void OnDisable()
    {
        theInput.Disable();
    }
    void Start()
    {
        if (theLevel.currentSceneName != "MainMenu")
        {
            
            GamePlayInput();//Gameplay输入
            //以下均是注册内容
            theInput.Gameplay.Jump.started += ctx =>
            {
                if (thePlayer.canAct)
                {
                    if(thePlayer.canJump)
                    {
                        //Debug.Log("普通跳");
                        thePlayer.ChangeToJumpState();
                        return;
                    }else if (thePlayer.canWallJump)
                    {
                        //Debug.Logs("蹬墙跳）;
                    }
                    else thePlayer.JumpBufferCheck();

                }
            };

            theInput.Gameplay.Jump.canceled += ctx =>
            {
                //玩家停止跳跃

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
                    //Debug.Log("不再上升");
                    thePlayer.HalfYVelocity();
                }
                
            };


            theInput.Gameplay.Exchange.started += ctx =>
            {
                //玩家冲刺
            };
            theInput.Gameplay.Attack.started += ctx =>
            {
                if (thePlayer.canAct&&thePlayer.canAttack)
                {
                    //Debug.Log("按了");
                    if(theInput.Gameplay.Movement.ReadValue<Vector2>().y > 0)
                    {
                        thePlayer.attackCounter = 4;
                    }
                    else if (!thePlayer.thisPR.IsOnFloored()&&theInput.Gameplay.Movement.ReadValue<Vector2>().y < 0)
                    {
                        thePlayer.attackCounter = 3;
                    }
                    else
                    {
                        if (thePlayer.continueAttackCounter > 0&&thePlayer.attackCounter==1)
                        {
                            thePlayer.attackCounter = 2;
                        }
                        else
                        {
                            thePlayer.attackCounter = 1;
                        }
                    }
                    thePlayer.thisAC.SetAttackCounter();
                    thePlayer.ChangeToAttackState();

                }
            };
            theInput.Gameplay.Interact.started += ctx =>
            {
                if (thePlayer.CurrentState() == thePlayer.handleState)
                {
                    thePlayer.StateOver();//不确定这里有没有风险
                }
                else if (thePlayer.theInteractable != null)
                {
                    thePlayer.theInteractable.Interact();
                }
            };
            theInput.Gameplay.Pause.started += ctx =>
            {
                if (theLevel.currentSceneName != "MainMenu")
                {
                    if (!theLevel.isLevelLoading)
                    {
                        if (theLevel.isPausing)
                        {
                            //theUI.GamePlayResume();
                            theLevel.GamePlayResume();
                            GamePlayInput();
                        }
                        else
                        {
                            //theUI.GamePlayPause();
                            theLevel.GamePlayPause();
                            UIInput();
                        }
                    }
                }
            };
        }
        else
        {
            //主菜单的Input
        }
    }


    private void Update()
    {
        
        //if (!thePlayer.releaseDuringRising && theInput.Gameplay.Jump.IsPressed())
        //{
        //    thePlayer.holdingCounter += Time.deltaTime;
        //    if (thePlayer.holdingCounter > thePlayer.apexThresholdLength)
        //    thePlayer.isPastApexThreshold = true;

        //}

    }

    public void GamePlayInput()
    {

        theInput.Disable();
        theInput.Gameplay.Enable();
    }
    public void LevelSelectInput()
    {
        theInput.Disable();
        //theInput.LevelSelect.Enable();
    }
    public void UIInput()//UI用的
    {
        theInput.Disable();
        theInput.UI.Enable();
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


    public bool WhetherZPressing()
    {
        return theInput.Gameplay.Attack.ReadValue<float>() > .5f;
    }
    public bool WhetherXPressing()
    {
        return theInput.Gameplay.Exchange.ReadValue<float>() > .5f;
    }
    public bool WhetherCPressing()
    {
        return theInput.Gameplay.Jump.ReadValue<float>() > .5f;
    }
    public bool WhetherSPressing()
    {
        return theInput.Gameplay.Interact.ReadValue<float>() > .5f;
    }

}
