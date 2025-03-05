using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

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
    public int verticalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().y != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().y > 0) ? ((thePlayer.canWallClimbForward) ? 1 : 0) : -1) : 0;



    private void Awake()
    {
        thisCM = GetComponentInParent<ControllerManager>();
        theInput = new Input1();
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
            thePlayer = thisCM.thePlayer;//LevelSelect场景没这个所以在这里引入而非Awake，下GamePlay同理
            GamePlayInput();//用于切换ActionMap
            //以下均是注册内容
            theInput.Gameplay.Jump.started += ctx =>
            {

                if (thePlayer.thisPR.IsOnGround())
                {
                    if (thePlayer.canAct && thePlayer.canJump)
                    {

                        //Debug.Log("普通跳");
                        thePlayer.ChangeToJumpState();
                        return;
                    }
                    else thePlayer.JumpBufferCheck();
                }
                else if(thePlayer.thisPR.IsOnWall())
                {
                    //Debug.Log("蹬墙跳");
                }
            };
            theInput.Gameplay.Jump.canceled += ctx =>
            {
                //玩家停止跳跃
            };
            theInput.Gameplay.Dash.started += ctx =>
            {
                //玩家冲刺
            };
            theInput.Gameplay.Grab.started += ctx =>
            {
                //玩家抓住墙壁
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
        return theInput.Gameplay.Grab.ReadValue<float>() > .5f;
    }
}
