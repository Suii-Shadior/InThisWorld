using Cinemachine;
using StructForSaveData;
using System.Collections;
using UnityEngine;
using PlayerInterfaces;
using InteractableInterface;
using SubInteractiveEnum;

public class NewPlayerController : MonoBehaviour,ISave<PlayerSaveData>
{



    /* ����״̬���߼�ѭ��
     * ...���뵱ǰ״̬ ==> ���е�ǰ״̬ ==> �˳���ǰ״̬==>������һ״̬...
     * 
     * ǰ��������
     * 1.����״̬���࣬������ҿ���������״̬�������ʼ��״̬���л�״̬�ķ�����
     * 2.����״̬���࣬���幹�캯��������״̬������״̬���˳�״̬�ĳ��ÿɼ̳з���
     * 3.����ÿһ��״̬���࣬�ڸ����м̳й��캯��������״̬������״̬���˳�״̬�Ķ�Ӧ���巽��
     * 4.Ϊÿһ��״̬�����ṩ�����ܹ�ͨ����ҿ�����������п���ʹ�ò�����·����
     * 
     * ʵ��˼·��
     * 1.����ÿһ��״̬����Ķ����Լ�һ��״̬����Ķ��󣬸��ݹ��캯��ȷ����ҿ�����-״̬��-����״̬�໥��ϵ��������״̬֮�䲢��֪����
     * 2.�ڽű�����ʱ�����ó�ʼ����״̬��ֱ�ӿ�ʼ����״̬����
     * 
     * ����Ҫ�㣺
     * 1��ItemUse�Ƿ���һ���취ͨ��״̬������˵��Ϊ�߼����޴����ֻ��ÿһ����Ʒ��������Ӧһ��״̬��
     * 2��canAct,isUnctrollering������
     *  a.isUncontrol��!canAct������һ������״̬�����Ǿ���״̬�µ��ض���ʶ���俪ʼ���������һ��������״̬���л���
     *  b.isUncontrol���ڱ�ʾ��ǰ״̬�²�����Ϊ����Ӧ�߼����ޣ����岻���������ǽ��ʱ��Ķ�ʱ���ڲ���ת��ͨ����ʱ������������һ���������״̬�ͻ�ȡ����
     *  b.!canAct���ڱ�ʾ����״̬�²�����Ϊ�����ڶԻ���������鶯���ȣ���Ϸ������Ȼ���˶���������ҿ������൱���Ĳ�ȷ��ʱ���ڲ��ܲ�����ɫ��ֻ�������ض����������ȡ��
     *  c.ע�⣬isGameplay=>isGamePauseʹ������ͣ����Ϸ����ҲҪһ��ֹͣ�ĳ��ϣ������е�UPdate��FixedUpdate����Ӧ�������ˣ�����UI����ӦInput����
     * 
     */
    #region ���
    [HideInInspector] public Rigidbody2D thisRB;
    [HideInInspector] public PhysicsRelated thisPR;
    [HideInInspector] public PlayerAnimatorController thisAC;
    [HideInInspector] public PlayerFXController thisFX;
    [HideInInspector] public CharacterRelated thisCR;
    [SerializeField] private ControllerManager theCM;
    private DataController theData;
    private EventController theEC;

    public ActionStat actionStats;

    private LevelController theLevel;
    private DialogeController theDC;
    private InputController theInput;
    public Transform thisDP;
    public BoxCollider2D thisBoxCol;
    public PlayerSaveData thisPlayerSaveData;


    #endregion
    #region ״̬�����
    public NewPlayerStateMachine stateMachine { get; private set; }
    public NewPlayerIdleState idleState { get; private set; }
    public NewPlayerRunState runState { get; private set; }
    public NewPlayerRiseState jumpState { get; private set; }
    public NewPlayerFallState fallState { get; private set; }
    public NewPlayerSwordState_Attack sward_attackState { get; private set; }
    public NewPlayerUmbrellaState umbrellaState { get; private set; }
    public NewPlayerHandleState handleState { get; private set; }
    //public NewPlayerApexState apexState { get; private set; }

    #endregion
    #region 


    #region ����
    [Header("ͨ��")]
    private bool isGameplay;
    private bool canAct;
    private bool isUncontrol;
    private float uncontrolCounter;

    public Vector2 lastSavePos;
    //public bool faceRight = true;
    public int faceDir = 1;
    public bool needTurnAround;
    public int horizontalInputVec;
    public int verticalInputVec;

    [Header("����״̬��ֵ")]
    public string nowState;
    public float horizontalMoveSpeedAccleration;
    public float horizontalMoveSpeed;
    public float horizontalMoveSpeedMax;
    public float horizontalmoveThresholdSpeed;
    //public float verticalFallAccleration;
    public float verticalFallSpeedMax;
    public float verticalMoveSpeed;
    public float verticalMoveSpeedMax;
    public float verticalThresholdSpeed;

    [Header("���߽�����ֵ")]
    public float ItemUse1Cooldown;




    [Header("�ж�����")]
    public bool attackAbility;
    public bool umbrellaAbility;
    public bool candleAbility;


    [Header("�ж�����")]
    public bool canHorizontalMove;
    public bool canVerticalMove;
    public bool canJump;
    public bool canWallJump;//TODO�����ܲ���Ҫ
    public bool canItemUse1;
    public bool canItemUse2;
    public bool canInteract;

    [Header("�߼�����")]
    public bool canTurnAround;


    [Header("��������")]
    public PlayerConfig playerConfig;


    [Header("����")]
    private bool isFastFalling;
    private float fastFallTime;
    private float fastfallReleaseSpeed;
    //public bool isPastApexThreshold;



    [Header("��Ծ")]

    public bool releaseDuringRising;//��falseʱ����ζ���������Ծ�����Ұ�ס����Ծ������tureʱ����ζ����Ҳ�û����Ծ��������Ծ�Ѿ��½��ˡ������������������ɿ�����Ծ��
    private float coyoteJumpCounter; //coyote




    [Header("����ʹ��")]
    public float ItemUse1Counter;
    public float ItemUse2Counter;
    [Header("�������")]
    public float sword_ContinueAttackCounter;
    public int attackSignal;





    [Header("����")]
    public float invinsibleCounter;


    [Header("�����븴��")]
    public float sitCounter;

    [Header("����̨")]
    public IHandle theHandle;

    [Header("�������")]
    public IInteract theInteractable;



    #endregion

    #region ����
    private const string PLAYERSAVEDATAIDSTR = "thePlayerData";
    #endregion
    private void Awake()
    {
        //��ȡ���
        thisRB = GetComponent<Rigidbody2D>();
        thisPR = GetComponent<PhysicsRelated>();
        thisAC = GetComponentInChildren<PlayerAnimatorController>();
        thisFX = GetComponentInChildren<PlayerFXController>();
        thisCR = GetComponentInChildren<CharacterRelated>();
        thisBoxCol = GetComponent<BoxCollider2D>();

    }

    private void OnEnable()
    {
        /*
         * 
         * 
         * Work1.��ȡCM�������Playerû����CM�£�����ֻ���������ȡCM��������
         * Work2.�������
         *��
         * 
         * 
         */
        theLevel = theCM.theLevel;
        theDC = theCM.theDC;
        theInput = theCM.theInput;
        theData = theCM.theData;
        theEC = theCM.theEvent;

        thisRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        thisRB.constraints = RigidbodyConstraints2D.FreezeRotation;

        PlayerOriginalSetting();
    }

    private void Start()
    {
        /*
         * Work1.��ʼ��״̬����ÿ��״̬ 
         * Work2.����״̬����ʼ����
         */
        PlayerStatesOringinSetting();
        SceneLoad_SetPlayerState();
    }

    // Update is called once per frame
    private void Update()
    {
        /* 
         * Work1.��������Ԥ����
         * Work2.���ݵ�ǰ״̬���ָ��£�
         * Work2-a.����Gameplay
         *  Step1.������������������
         *  Step2.���н׶�����Ʒ������ʱ
         *  Step3.���н׶�����ȴ��ʱ
         *  Step4.������ϵͳ��ʱ//TODO�������Ƿ�ϲ�
         *  Step5.����״̬�µ�Update
         * Work2-b.����Gamepause��GameMenu
         *  Step1.���ݵ�ǰָ��λ�ø���ָ�붯��
         * 
         */
        Gameplay_InputProcess();
        if (isGameplay)
        {

            thisPR.PRUpdata();
            ItemUseUpdate();
            GameplayCooldownCount();
            //�������ܵ�ϵͳ��ؼ�ʱ
            stateMachine.currentState.Update();

        }
    }

    private void FixedUpdate()
    {
        /*
         * 
         * Step1.�������������������������
         * Step2.���������������״̬�����������������������
         * Step3.����״̬�µ�FixedUpdate
         * Step4.���г������//TODO�������Ƿ�Ż�Updata
         * 
         */
        if (isGameplay)
        {
            stateMachine.currentState.FixedUpdate();
            //TurnAround();
            FaceDirUpdate();
        }

    }





    #region Gameplay����(���������е��÷���)


    public void InteractRelated_SaveItem(Vector2 _savePointPos)
    {
        /* �����������ڱ���ʱ�������״̬��λ��,�����ڱ��潻������ʱ
         * 
         * Step1.����ҵ�״̬���õ���Ӧ״̬
         * Step2.�������ΪUnact����ؼ�ʱ
         * Step3.����ҵ�λ���赽��Ӧλ��
         * 
         */
        transform.position= new Vector3(_savePointPos.x, _savePointPos.y, 0);
        
    }

    public void ResetRelated_UnKnockable(Vector2 _resetPointPos)
    {
        /* ������������Gameplayʱ����������ڹ̶�λ�ã�����������ܵ�ĳЩ���ܼ򵥻��˵��˺�ʱ
         * 
         * Step1.�����λ�����õ���Ӧ
         * Step2.�����״̬���õ���Ӧ״̬
         * Step3.�������ΪUnact����ؼ�ʱ
         * 
         */
        transform.position = new Vector3(_resetPointPos.x, _resetPointPos.y, 0);

    }




    #endregion
    #region ��ʼ�����


    public void PlayerOriginalSetting()
    {

        /* ���������ڿ������ĳ�ʼ���������ڻص���ҳ��ʱ�ĵ���
         * 
         * Step1. ע���Ծ�ɴ洢��
         * Step2.���ڴ�浵�л�ȡ��Ҵ浵����
         */
        RegisterSaveable(GetComponent<ISaveable>());
        SceneLoad_CorresponceWithSaveData();


    }

    private void PlayerStatesOringinSetting()
    {
        stateMachine = new NewPlayerStateMachine();
        idleState = new NewPlayerIdleState(this, stateMachine, "isIdling");
        runState = new NewPlayerRunState(this, stateMachine, "isHorizontalMoving");
        jumpState = new NewPlayerRiseState(this, stateMachine, "isJumping");
        fallState = new NewPlayerFallState(this, stateMachine, "isFalling");
        sward_attackState = new NewPlayerSwordState_Attack(this, stateMachine, "isAttacking");
        umbrellaState = new NewPlayerUmbrellaState(this, stateMachine, "isUmbrellaing");
        handleState = new NewPlayerHandleState(this, stateMachine, "isIdling");
    }



#endregion

    #region Can�ж����
    public void WhetherCanJumpOrWallJump()
    {

        /* �������жϵ�ǰ�Ƿ��ܹ�������Ծ���ǽ������ͨ���͵�ǽ��������Ծ��������can�ж�Ҳ���⣩���������ھ���״̬��Update
         * 
         * Step1.�ж��Ӻ��ʱ�Ƿ�Ϊδ��0�����ǣ��ܹ�ȷ��������Ծ�����������Step2�жϣ�ͬʱ�Ӻ��ʱ����������һ�����ܽ�����Ծ����
         * Step2.�жϵ�ǰ�Ƿ�ʵ�ڵ����ϡ����ǣ��������ͨ��Ծ�����������Step3�ж�
         * Step3.�жϵ�ǰ�Ƿ���ǽ���ϣ����ǣ�����е�ǽ�����������ܽ�����Ծ��
         * 
         */
        if (coyoteJumpCounter > 0f)
        {
            if (!thisPR.IsOnFloored())
            {
                if (thisPR.IsOnWall())
                {
                    canJump = false;
                    canWallJump = true;
                }
                else
                {
                    canJump = true;
                    canWallJump = false;
                }
                coyoteJumpCounter -= Time.deltaTime;
                //Debug.Log(coyoteJumpCounter);
            }
            else
            {
                canJump = true;
                canWallJump = false;
            }
        }
        else
        {
            canJump = false;
            canWallJump = false;
        }

    }
    public void RefreshCanJump()
    {
        //���������������¿�ʼ�Ӻ��ʱ��ʵ��ˢ����Ծ��ʵ��Ч���������ڲ�����س����µĲ��ֻ�����ɵ�Ч��
        coyoteJumpCounter = playerConfig.coyoteJumpLength;
    }

    public void CoyoteCounterZero()
    {
        //���������ڽ�����Ծ��������Ӻ��ʱ��0����������ͨ��Ծ�͵�ǽ������״̬
        coyoteJumpCounter = 0f;
    }

    public void WhetherCanItemUse1()
    {
        /* �������жϵ�ǰ�Ƿ��ܹ����е���ʹ��1���������ھ���״̬��Update
         * 
         * �ж���ȴ��ʱ�Ƿ�δ��0�����ǣ�ȷ�����Խ��е���ʹ��1���������ܽ���
         *  
         */
        if (ItemUse1Counter<=0)
        {
            canItemUse1 = true;
        }
        else
        {
            canItemUse1 = false;
        }
    }

    public void WhetherCanInteract()
    {
        /* �����������жϵ�ǰ�Ƿ���Խ��г����������������ڰ��½�����ʱʹ��
         * 
         * Step1.�жϵ�ǰ�ǲ��Ǵ���ĳЩͨ����˵���ܽ��г���������״̬���������ʹ�ã�
         * Step2.�жϵ�ǰ�Ƿ���ڿɽ��������㹻�Ƚ���״̬//TODO������ط����кܶ�ռ���Ե���
         * 
         */
        if (CurrentState()!= sward_attackState)
        {
            if (theInteractable != null &&thisPR.IsOnFloored())
            {
                canInteract = true;
            }
            else
            {
                canInteract = false;
            }
        }
    }


    #endregion 

    #region ��������⣨������ײ�壩
    
    
    /* ��Player���жϵ�Ҫ��:
     * 1����Ҫ����Player����Ӧ�Ķ�������Զ����߼���ͬ�����ַ�ɢ�ڸ��Խű��У�����������ڴ˼��й���
     * 2�����������ǰ��սű����ͣ�����ʹ���ȡ���ű���Ȼ�����ù���ģʽ�������
     * 3��֮����ܶ�PRϵͳ���//TODO��ͬ�����ű�Ҳ�����Ŵ��
     * 
     * 
     */

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.GetComponent<FloorController>())
        {
            FloorController theFloor = other.GetComponent<FloorController>();
            theFloor.SetPlayer(this);
            theFloor.currentFloor?.PlayerEnter();
        }
    }


    private void OnTriggerStay2D(Collider2D other)//Enter��˲�����ComCol��û��⵽
    {
        if (other.TryGetComponent<PlatformController>(out PlatformController thePlatform))
        {
            if (//thePlatform.GetPlayer() == null && 
                thePlatform.GetComCol() == thisPR.RayHit().collider)
            {
                thePlatform.SetPlayer(this);
                if (!thePlatform.hasSensored && (thePlatform.thisPlatformType == PlatformInteractiveType.moveable_toucher || thePlatform.thisPlatformType ==PlatformInteractiveType.disappearable_sensor))
                {
                    //Debug.Log("����");
                    thePlatform.currentPlatform.Interact1();
                    thePlatform.hasSensored = true;
                }

            }

        }else if (other.TryGetComponent<FloorController>(out FloorController theFloor))
        {
            theFloor.currentFloor?.PlayerStay(); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlatformController>(out PlatformController thePlatform))
        {
            thePlatform.SetPlayer(null);
            thePlatform.hasSensored = false;
        }
        else if (other.TryGetComponent<FloorController>(out FloorController theFloor))
        {
            other.GetComponent<FloorController>().ClearPlayer();
            theFloor.currentFloor? .PlayerExit();
        }
    }
    #endregion

    #region ������Gameplay��صķ���
    public void FaceDirUpdate()
    {
        /* ������������Gameplayʱȷ���泯����ͨ��scaleʵ�ֶ���������Y�ᷭת��������Update
         * 
         * 
         * 
         * 
         */
        if (canTurnAround)
        {
            faceDir = (int)transform.localScale.x;
            if (horizontalInputVec > 0)
            {
                faceDir = 1;
            }
            else if (horizontalInputVec < 0)
            {

                faceDir = -1;
            }
            else
            {
                //Debug.Log("����Ҫ�仯")
            }
            transform.localScale = new Vector3(faceDir, 1, 1);

        }
        else
        {
            //Debug.Log("����ת��");
        }
        
    }



    public void JumpBufferCheck()
    {
       Coroutine jumpBuffer=  StartCoroutine(JumpBufferCheckCo(playerConfig.jumpBufferLength));
    }
    private IEnumerator JumpBufferCheckCo(float counter)
    {
        while (counter > 0)
        {
            //Debug.Log("��ʼ��⻺��");
            if (canJump)
            {
                
                    Debug.Log("������Ծ�ɹ�");
                    ChangeToJumpState();
                    break;//����û����return��������Break�������ѭ����
            }
            counter -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //if(counter<=0) Debug.Log("������⻺��");
    }






    #endregion

    #region �ı�״̬�Ĵ��루ֻ���ⲿ���ã�
    public void StateOver()
    {
        /* ����������ֹͣ��ǰ״̬�����ڲ���������ԴӲ�ȷ���Ĳ���״̬�����޲���״̬
         * 
         * Stage1.����ʱ����ͨ�������ж��˳�����״̬
         * Stage2.���ݻ���ʱ����ͨ�������˳�����״̬
         * 
         * 
         */
        if (thisPR.IsOnFloored()) ChangeToIdleState();
        else stateMachine.ChangeState(fallState);
    }
    public NewPlayerState CurrentState()
    {
        return stateMachine.currentState;
    }
    public void ChangeToIdleState()
    {
        stateMachine.ChangeState(idleState);
    }
    public void ChangeToHorizontalMoving()
    {
        stateMachine.ChangeState(runState);
    }

    public void ChangeToFallState()
    {
        stateMachine.ChangeState(fallState);
    }
    public void ChangeToJumpState()
    {
        stateMachine.ChangeState(jumpState);

    }
    public void ChangeToAttackState()
    {
        stateMachine.ChangeState(sward_attackState);
    }

    public void ChangeToHandleState()
    {
        stateMachine.ChangeState(handleState);
    }

    //public void ChangeToUmbrellaState()
    //{
    //    if (umbrellaAbility && canBabble)
    //        stateMachine.ChangeState(babbleState);
    //}


    //public void ChangeToKnockBack()
    //{
    //    stateMachine.ChangeState(knockedBackState);
    //}

    //public void ChangeToDeadState()
    //{
    //    stateMachine.ChangeState(deadState);
    //}

    public void SetIsGamePlay(bool _whetherIsGamePlay)
    {
        isGameplay = _whetherIsGamePlay;
    }

    public bool GetIsGamePlay()
    {
        return isGameplay;
    }

    #endregion
    #endregion




    #region ����Input��ط���

    private void Gameplay_InputProcess()
    {
        /* ������������Gameplayʱ���������Ԥ����
         * Step1�����ݲ�ͬ�ľ���״̬�ֱ���в�ͬ��Ԥ����
         * 
         * 
         */
        if(stateMachine.currentState == handleState)
        {
            HnadleVecUpdate();
        }
        else
        {
            MovementVecUpdate();
        }
    }

    private void MovementVecUpdate()
    {
        /* ���������ڶԷ��ض�״̬�µķ����������Ԥ����
         * 
         * Step1.�жϵ�ǰ�Ƿ�isOnFloored�����ǣ�����Step2-a�����񣬽���Step2-b
         * Step2-a.��ˮƽ���뷽���뵱ǰ��ɫ�泯����һ����isOnWalled������������룻����ԭ����
         * Step2-b.��ˮƽ���뷽���뵱ǰ��ɫ�泯����һ����isOnWalled��isForward�����ӵ�ǰ���룻����ԭ����
         * Step3.��ֱ���뷽��ֱ��ԭ����
         * 
         */
        if (canAct)
        {
            if (thisPR.IsOnFloored())
            {
                if (theCM.theInput.horizontalInputVec == faceDir && thisPR.IsOnWall())
                {
                    horizontalInputVec = 0;
                }
                else
                {
                    horizontalInputVec = theInput.horizontalInputVec;
                }
            }
            else
            {
                if (theCM.theInput.horizontalInputVec == faceDir && (thisPR.IsOnWall() || thisPR.IsForwad()))
                {
                    horizontalInputVec = 0;
                }
                else
                {
                    horizontalInputVec = theInput.horizontalInputVec;
                }
            }
            verticalInputVec = theInput.verticalInputVec;
        }
    }

    private void HnadleVecUpdate()
    {
        // ����������Handle�ض�״̬�¶Է����������Ԥ������˴�Ҳ�޷�ȷ����ӦHandle�Ķ����߼�����ֱ�ӵ���ԭ���룬�ھ����Interact�ű��ڽ����ٴ�Ԥ����
        if (canAct)
        {
            horizontalInputVec = theInput.horizontalInputVec;
            verticalInputVec = theInput.verticalInputVec;
        }
    }

    #endregion


    #region RB��ط���
    public void ClearVelocity()
    {
        thisRB.velocity += new Vector2(-thisRB.velocity.x, -thisRB.velocity.y);
    }

    public void ClearYVelocity()//�������PlayerY���ϵ��ٶ�
    {
        thisRB.velocity += new Vector2(0, -thisRB.velocity.y);

    }
    public void ClearXVelocity()//�������PlayerX���ϵ��ٶ�
    {
        thisRB.velocity += new Vector2(-thisRB.velocity.x, 0);
    }
    public void HalfYVelocity()
    {
        thisRB.velocity += new Vector2(0, -thisRB.velocity.y / 2);
    }
    public void GroundVelocityLimit()//������������ƶ��ٶ�����
    {
        if (Mathf.Abs(thisRB.velocity.x) > horizontalMoveSpeedMax)
        {
            //Debug.Log("����");
            thisRB.velocity += new Vector2(faceDir * horizontalMoveSpeedMax - thisRB.velocity.x, 0f);
        }

    }

    public void GroundInertialClear()//����������Idle״̬�µĲ�������
    {
        if (Mathf.Abs(thisRB.velocity.x) < horizontalmoveThresholdSpeed) thisRB.velocity += new Vector2(-thisRB.velocity.x, 0f);
    }
    public void InertiaXVelocity()
    {
        //Debug.Log(Mathf.Lerp(Mathf.Abs(thisRB.velocity.x), moveSpeedMax, .1f));
        thisRB.velocity = new Vector2(faceDir * Mathf.Lerp(Mathf.Abs(thisRB.velocity.x), horizontalMoveSpeedMax, .1f), 0f);
        //Debug.Log("�ڹ��Լ���" + thisRB.velocity.x);
    }
    #endregion







    #region �ӿ�ʵ��
    #region �ɴ洢���ݶ�����ط���

    public string GetISaveID()
    {
        return thisPlayerSaveData.playerSaveDataID;
    }

    public void RegisterSaveable(ISaveable _saveable)
    {
        theEC.SaveableRegisterPublish(_saveable);
    }

    public void UnregisterSaveable(ISaveable _saveable)
    {
        theEC.SaveableUnregisterPublish(_saveable);
    }

    public void LoadSpecificData(SaveData _passingData)
    {
        thisPlayerSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
    }

    public SaveData SaveDatalizeSpecificData()
    {
        SaveData _savedata = new()
        {
            key = thisPlayerSaveData.playerSaveDataID,
            type = TypeRegistry.GetDataTypeFromReverseDict(typeof(PlayerSaveData)),
            value = JsonUtility.ToJson(thisPlayerSaveData)
        };
        return _savedata;
    }

    public PlayerSaveData GetSpecificDataByISaveable(SaveData _passedData)
    {
        PlayerSaveData thedata = JsonUtility.FromJson<PlayerSaveData>(_passedData.value);
        //Debug.Log(thedata.isOpened);
        return thedata;
    }

    public void SaveDataSync()
    {
        thisPlayerSaveData.attackAblity = attackAbility;
        thisPlayerSaveData.umbrellaAbility = umbrellaAbility;
        thisPlayerSaveData.candleAbility = candleAbility;
        thisPlayerSaveData.lastSavePointX = transform.position.x;
        thisPlayerSaveData.lastSavePointY = transform.position.y;
        //Debug.Log("PlayerSavaData����ɹ�");
    }
    #endregion


    #endregion

    #region �����Է���

    public bool GetCanAct()
    {
        return canAct;
    }
    public void Unact()
    {
        canAct = false;
    }

    public void CanAct()
    {
        canAct = true;
    }

    public bool GetIsUncontrol()
    {
        return isUncontrol;
    }

    public void Uncontrol(float _uncontrolDuration)
    {
        isUncontrol = true;
        uncontrolCounter = playerConfig.uncontrolDuration;
    }
    public void UncotrolEnd()
    {
        isUncontrol = false;
        uncontrolCounter = 0;
    }

    public void SetHandle(IHandle theHandler)
    {
        theHandle = theHandler;
    }



    private void GameplayCooldownCount()
    {

        if (uncontrolCounter >= 0)
        {
            uncontrolCounter -= Time.deltaTime;
        }
        else
        {
            isUncontrol = false;
        }

        if (ItemUse1Counter >= 0)
        {
            ItemUse1Counter -= Time.deltaTime;
        }
        else
        {
            canItemUse1 = true;
        }
    }

    private void ItemUseUpdate()
    {
        /* ���������ڵ�����ؼ�ʱ����������ͨ֡
         * 
         * Work1.����������ʱ
         * 
         * 
         * 
         */
        if (sword_ContinueAttackCounter > 0 && stateMachine.currentState != sward_attackState)
        {
            sword_ContinueAttackCounter -= Time.deltaTime;
        }
    }




    public void KnockedBack(Vector2 _knockedBackDir)
    {
        ////float finalForce = knockedBackForce / thisRB.mass;
        //thisRB.AddForce(finalForce * _knockedBackDir, ForceMode2D.Impulse);
        ////Debug.Log("�������" + finalForce * _knockedBackDir.x);
        //// SetVelocity(finalForce * _knockedBackDir.x, finalForce * _knockedBackDir.y);
        //isUncontrol = true;
        //uncontrolCounter = uncontrolDuration;

    }

    public bool isStandOnPlatform()
    {

        // �����������ж��Ƿ�վ����Platform�ϣ���һ�����ڽ����������ƽ̨�ϵ�bug
        if (thisPR.RayHit().collider)
        {
            if (thisPR.RayHit().collider.GetComponent<PlatformController>())
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }

    private void SceneLoad_CorresponceWithSaveData()
    {
        // �����������ڳ�ʼ�׶δ�SaveDataд��ű���
        attackAbility = thisPlayerSaveData.attackAblity;
        umbrellaAbility = thisPlayerSaveData.umbrellaAbility;
        candleAbility = thisPlayerSaveData.umbrellaAbility;
        lastSavePos = thisPlayerSaveData.GetLastSavePoint();
        InteractRelated_SaveItem(lastSavePos);
    }
    private void SceneLoad_SetPlayerState()
    {
        /* ���������ڽ���ұ���״̬���г�ʼ��
         * 
         * Step1.����gameplay
         * Step2.���״̬��Ϊ��ʼ״̬
         * Step3.��canAct��Ϊ��
         * 
         * 
         */
        isGameplay = true;
        canAct = true;
        stateMachine.Initialize(fallState);//������Ҫ�����������жϵ�ǰӦ�ô���ʲô״̬
        //mushroomPoint = thisPR.theGroundCheckpoint;
    }

    #endregion


}

