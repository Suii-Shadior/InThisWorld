using CMRelatedEnum;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class InputController : MonoBehaviour
{


    #region ���
    private ControllerManager thisCM;
    private Input1 theInput;
    private NewPlayerController thePlayer;
    private LevelController theLevel;//����ؿ���
    private UIController theUI;

    #endregion

    /* ����������Ҫְ��
     * 1�����������ͼ���������л��߼�
     * 2��ע��ÿ�������ͼ�����¼�
     * 3�����з����Ԥ����
     * 
     * ע��㣺
     * 1�����ڰ����߼��л����������ض����������¼���������ͬʱ�漰��ҡ�UI������������ȶ���������������������UI��������
     * 2�����������������Ӱ���ע������ԭ��
     *  a.������������ݸ��ݲ�ͬ������������������߼����ò�ͬ��������¼�����Ҫ�����ı�ʶ���и����жϵ�ǰӦ�ø�ʲô�����ڱ��ű�����̫�����������ױ�ÿɶ��Լ���
     *  b.�ʷ�����İ���ͳһ���룬����Ҫʵ�ֵĽű��ڽ����ٴ���")
     * 
     * 
     
     */

    #region ����
    public int horizontalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().x != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().x > 0) ? 1 : -1) : 0;
    public int verticalInputVec => (theInput.Gameplay.Movement.ReadValue<Vector2>().y != 0) ? ((theInput.Gameplay.Movement.ReadValue<Vector2>().y > 0) ? 1 : -1) : 0;
    #endregion








    private void Awake()
    {
        /*
         * Work1.��ȡ���
         * Work2.��ʼ��һ��Input����
         * 
         */
        thisCM = GetComponentInParent<ControllerManager>();
        thePlayer = thisCM.thePlayer;//Tip;���ݳ�����ͬ���ܲ��ܷ�����
        theLevel = thisCM.theLevel;
        theUI = thisCM.theUI;


        theInput = new Input1();
    }
    private void OnEnable()
    {
        /*
         * Work1.����Input����
         * Work2.���ݴ�����PlayerPrefs����ȷ����������//TODO����û��
         */
        theInput.Enable();
    }
    void Start()
    {
        /*
         * Work1.�ں��ĳ�������ʱ����һ��Ĭ��InputMap�������ܻ��
         * Work2.ע�����InputMap�İ���
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
        
        //ͣ��Input����
         
        theInput.Disable();
    }



    #region С�������ⲿ����

    #region �ⲿ����-�����л�
    public void GamePlayInput()
    {
        //Gameplay�׶ε�������������
        theInput.Disable();
        theInput.Gameplay.Enable();
    }
    public void GamepauseInput()
    {
        //Gameplay�׶ε���ͣ��������
        theInput.Disable();
        theInput.PauseMenu.Enable();
    }
    public void GamemenuInput()
    {
        //���˵��׶ε�������������
        theInput.Disable();
        theInput.MainMenu.Enable();
    }
    #endregion

    #region �ⲿ����-�������
    //��Ҫ�������״̬�л�ʱ��һЩ�ж�
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

    #region �ڲ�ʹ��-����ע��
    private void GamePlayInputRegister()
    {
        /* Gameplay�׶εİ���ע�ᣨ�ų��������
         * 
         * �ڸý׶εİ���ע�ᣬͨ��Ϊ�����Ϊ���ݡ����ýṹ��������Ϊcan�ж�=>������Ϊcan�ж�/��֧/����
         * Work1.��Ծ�����£���Ծ/��ǽ�������ɿ����������������У�������������ԣ�
         * Work1.���߽���1�����£�ʹ�õ��߽���A��
         * Work1.�������������£�ʹ�õ�ǰ�ɼ������
         * 
         * Work2.��ͣ��
         * Work2.��ͼ��
         * 
         * Work3.�л������£������ʱ�����ɿ�(���⽻��/�������رյ���ѡ��� //TODO
         * Work3.���߽���2�����£�ʹ�õ��߽���B�� //TODO
         * Work3.����л���1//TODO
         * Work3.����л���2 //TODO
         * 
         */
        theInput.Gameplay.Jump.started += ctx =>
        {
            if (thePlayer.GetCanAct())
            {
                if (thePlayer.canJump)
                {
                    //Debug.Log("��ͨ��");
                    thePlayer.ChangeToJumpState();
                    return;
                }
                else if (thePlayer.canWallJump)
                {
                    //Debug.Logs("��ǽ����;
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
                //    ת��Apex״̬
                //}
                //else
                //{
                //    ת��Fall״̬
                //}
               // Debug.Log("���ˣ�");
                thePlayer.HalfYVelocity();
            }

        };
        theInput.Gameplay.ItemUse1.started += ctx =>
        {
            if (thePlayer.GetCanAct())
            {
                if (thePlayer.canItemUse1)
                {
                    //�����������Ʒ����1ͳһ�ӿڣ�
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
                //Debug.Log("����");
            }
        };
        theInput.Gameplay.Interact.started += ctx =>
        {
            if (thePlayer.GetCanAct())
            {
                thePlayer.WhetherCanInteract();
                if (thePlayer.canInteract)
                {
                    if (thePlayer.CurrentState() == thePlayer.handleState)
                    {
                        thePlayer.StateOver();//��ȷ��������û�з���
                    }
                    else
                    {
                        Debug.Log("����");
                        thePlayer.theInteractable.Interact();
                    }

                }
                else
                {
                    Debug.Log("���ܽ���");
                }
            }
        };
        theInput.Gameplay.Exchange.started += ctx =>
        {
            //����л�
        };
        theInput.Gameplay.Pause.started += ctx =>
        {
            //����ֱ�ӵ��õ�UI�ڰ�����ͳ�﷽������Ҫ��Ϊ�˸���
            if(theInput.Gameplay.enabled)
            {
                //Debug.Log("��ͣ");
                theUI.FuncCall_GamePlay_Pause();
            }

        };
    }
    private void GamePauseInputRegister()
    {
        theInput.PauseMenu.Resume.started += ctx =>
        {
            //����ֱ�ӵ��õ�UI�ڰ�����ͳ�﷽������Ҫ��Ϊ�˸���
            if (theInput.PauseMenu.enabled)
            {
                //Debug.Log("�ָ�");
                theUI.FuncCall_GamePause_Resume();
            }
        };
    }
    private void MainMenuInputRegister()
    {
        /* Gamemenu�׶εİ���ע��
         * 
         * Work1.ȷ�ϼ�
         * Work1.�˳���
         */
    }
    #endregion

    #endregion


    #region ��ʱû��ʹ�õķ���
    //public void DialoguingInput()
    //{
    //    if (theDC.isPrinting)
    //    {
    //        theDC.QuickPrint();
    //    }
    //    else
    //    {
    //        theDC.NextSentence();
    //    }
    //}


    #endregion



}
