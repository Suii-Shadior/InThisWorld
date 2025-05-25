using Cinemachine;
using StructForSaveData;
using System.Collections;
using UnityEngine;
using PlayerInterfaces;
using InteractableInterface;
using SubInteractiveEnum;

public class NewPlayerController : MonoBehaviour,ISave<PlayerSaveData>
{



    /* 有限状态机逻辑循环
     * ...进入当前状态 ==> 运行当前状态 ==> 退出当前状态==>进入下一状态...
     * 
     * 前提条件：
     * 1.定义状态机类，用于玩家控制器管理状态，定义初始化状态和切换状态的方法；
     * 2.定义状态父类，定义构造函数、进入状态、运行状态、退出状态的常用可继承方法
     * 3.定义每一个状态子类，在该类中继承构造函数、进入状态、运行状态、退出状态的对应具体方法
     * 4.为每一个状态子类提供用于能够通过玩家控制器获得所有可能使用参数的路径。
     * 
     * 实现思路；
     * 1.定义每一个状态子类的对象，以及一个状态机类的对象，根据构造函数确定玩家控制器-状态机-具体状态相互关系，各具体状态之间并不知晓。
     * 2.在脚本运行时，设置初始具体状态。直接开始运行状态机。
     * 
     * 其他要点：
     * 1、ItemUse是否有一个办法通用状态？还是说因为逻辑差别巨大可能只能每一个物品交互都对应一个状态？
     * 2、canAct,isUnctrollering辨析：
     *  a.isUncontrol和!canAct均不是一个具体状态，而是具体状态下的特定标识，其开始或结束并不一定伴随着状态的切换。
     *  b.isUncontrol用于表示当前状态下部分行为及相应逻辑受限，含义不定。比如蹬墙跳时候的短时间内不能转身，通常计时结束或满足了一定条件后该状态就会取消；
     *  b.!canAct用于表示具体状态下不能行为。用于对话、复活、剧情动画等，游戏场景仍然在运动，但是玩家可能在相当长的不确定时间内不能操作角色，只有满足特定条件后才能取消
     *  c.注意，isGameplay=>isGamePause使用在暂停等游戏场景也要一并停止的场合，即所有的UPdate，FixedUpdate都不应该运作了（除了UI和相应Input）、
     * 
     */
    #region 组件
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
    #region 状态机相关
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


    #region 变量
    [Header("通用")]
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

    [Header("基础状态数值")]
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

    [Header("道具交互数值")]
    public float ItemUse1Cooldown;




    [Header("行动能力")]
    public bool attackAbility;
    public bool umbrellaAbility;
    public bool candleAbility;


    [Header("行动可行")]
    public bool canHorizontalMove;
    public bool canVerticalMove;
    public bool canJump;
    public bool canWallJump;//TODO：可能不需要
    public bool canItemUse1;
    public bool canItemUse2;
    public bool canInteract;

    [Header("逻辑可行")]
    public bool canTurnAround;


    [Header("参数配置")]
    public PlayerConfig playerConfig;


    [Header("空中")]
    private bool isFastFalling;
    private float fastFallTime;
    private float fastfallReleaseSpeed;
    //public bool isPastApexThreshold;



    [Header("跳跃")]

    public bool releaseDuringRising;//当false时，意味着玩家在跳跃上升且按住了跳跃键；当ture时，意味着玩家并没有跳跃，或者跳跃已经下降了、或者在上升过程中松开了跳跃键
    private float coyoteJumpCounter; //coyote




    [Header("道具使用")]
    public float ItemUse1Counter;
    public float ItemUse2Counter;
    [Header("具体参数")]
    public float sword_ContinueAttackCounter;
    public int attackSignal;





    [Header("受伤")]
    public float invinsibleCounter;


    [Header("死亡与复活")]
    public float sitCounter;

    [Header("操纵台")]
    public IHandle theHandle;

    [Header("交互相关")]
    public IInteract theInteractable;



    #endregion

    #region 常量
    private const string PLAYERSAVEDATAIDSTR = "thePlayerData";
    #endregion
    private void Awake()
    {
        //获取组件
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
         * Work1.获取CM组件（因Player没有在CM下，所以只有在这里获取CM相关组件）
         * Work2.组件配置
         *，
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
         * Work1.初始化状态机和每个状态 
         * Work2.进行状态机初始设置
         */
        PlayerStatesOringinSetting();
        SceneLoad_SetPlayerState();
    }

    // Update is called once per frame
    private void Update()
    {
        /* 
         * Work1.进行输入预处理
         * Work2.根据当前状态区分更新，
         * Work2-a.若是Gameplay
         *  Step1.物理条件及参数更新
         *  Step2.进行阶段下物品交互计时
         *  Step3.进行阶段下冷却计时
         *  Step4.其他的系统计时//TODO：考虑是否合并
         *  Step5.具体状态下的Update
         * Work2-b.若是Gamepause或GameMenu
         *  Step1.根据当前指标位置更新指针动画
         * 
         */
        Gameplay_InputProcess();
        if (isGameplay)
        {

            thisPR.PRUpdata();
            ItemUseUpdate();
            GameplayCooldownCount();
            //其他可能的系统相关计时
            stateMachine.currentState.Update();

        }
    }

    private void FixedUpdate()
    {
        /*
         * 
         * Step1.根据物理情况调整相关物理参数
         * Step2.根据物理情况区分状态，用作更改物理参数的依据
         * Step3.具体状态下的FixedUpdate
         * Step4.进行朝向更新//TODO：考虑是否放回Updata
         * 
         */
        if (isGameplay)
        {
            stateMachine.currentState.FixedUpdate();
            //TurnAround();
            FaceDirUpdate();
        }

    }





    #region Gameplay方法(包含交互中调用方法)


    public void InteractRelated_SaveItem(Vector2 _savePointPos)
    {
        /* 本方法用于在保存时重设玩家状态和位置,适用于保存交互调用时
         * 
         * Step1.将玩家的状态设置到相应状态
         * Step2.玩家设置为Unact及相关计时
         * Step3.将玩家的位置设到相应位置
         * 
         */
        transform.position= new Vector3(_savePointPos.x, _savePointPos.y, 0);
        
    }

    public void ResetRelated_UnKnockable(Vector2 _resetPointPos)
    {
        /* 本方法用于在Gameplay时将玩家重置在固定位置，适用于玩家受到某些不能简单击退的伤害时
         * 
         * Step1.将玩家位置设置到相应
         * Step2.将玩家状态设置到相应状态
         * Step3.玩家设置为Unact及相关计时
         * 
         */
        transform.position = new Vector3(_resetPointPos.x, _resetPointPos.y, 0);

    }




    #endregion
    #region 初始化相关


    public void PlayerOriginalSetting()
    {

        /* 本方法用于控制器的初始化，适用于回到主页面时的调用
         * 
         * Step1. 注册活跃可存储表
         * Step2.从内存存档中获取玩家存档内容
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

    #region Can判断相关
    public void WhetherCanJumpOrWallJump()
    {

        /* 本方法判断当前是否能够进行跳跃或蹬墙跳（普通跳和蹬墙跳共用跳跃键，故其can判断也互斥），适用于在具体状态下Update
         * 
         * Step1.判断延后计时是否为未到0。若是，能够确定可以跳跃动作，则进入Step2判断，同时延后计时继续；若否，一定不能进行跳跃动作
         * Step2.判断当前是否实在地面上。若是，则进行普通跳跃；若否，则进入Step3判断
         * Step3.判断当前是否在墙壁上，若是，则进行蹬墙跳；若否，则不能进行跳跃。
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
        //本方法用于在重新开始延后计时，实现刷新跳跃的实际效果，适用于并非落地场景下的部分机关造成的效果
        coyoteJumpCounter = playerConfig.coyoteJumpLength;
    }

    public void CoyoteCounterZero()
    {
        //本方法用于进行跳跃动作后的延后计时置0，适用于普通跳跃和蹬墙跳进入状态
        coyoteJumpCounter = 0f;
    }

    public void WhetherCanItemUse1()
    {
        /* 本方法判断当前是否能够进行道具使用1，适用于在具体状态下Update
         * 
         * 判断冷却计时是否未到0，若是，确定可以进行道具使用1；若否，则不能进行
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
        /* 本方法用于判断当前是否可以进行场景交互，适用于在按下交互键时使用
         * 
         * Step1.判断当前是不是处于某些通常来说不能进行场景交互的状态。例如道具使用，
         * Step2.判断当前是否存在可交互对象足够稳健的状态//TODO：这个地方还有很多空间可以调整
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

    #region 触发器检测（胶囊碰撞体）
    
    
    /* 在Player中判断的要点:
     * 1、需要传递Player到对应的对象，因各自对象逻辑不同，且又分散在各自脚本中，不方便管理，在此集中管理
     * 2、基本步骤是按照脚本类型，首先使其获取本脚本，然后沿用工厂模式进行设计
     * 3、之后可能对PR系统大改//TODO：同样本脚本也会随着大改
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


    private void OnTriggerStay2D(Collider2D other)//Enter在瞬间可能ComCol还没检测到
    {
        if (other.TryGetComponent<PlatformController>(out PlatformController thePlatform))
        {
            if (//thePlatform.GetPlayer() == null && 
                thePlatform.GetComCol() == thisPR.RayHit().collider)
            {
                thePlatform.SetPlayer(this);
                if (!thePlatform.hasSensored && (thePlatform.thisPlatformType == PlatformInteractiveType.moveable_toucher || thePlatform.thisPlatformType ==PlatformInteractiveType.disappearable_sensor))
                {
                    //Debug.Log("进入");
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

    #region 其他与Gameplay相关的方法
    public void FaceDirUpdate()
    {
        /* 本方法用于在Gameplay时确定面朝方向，通过scale实现动画及功能Y轴翻转，适用于Update
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
                //Debug.Log("不需要变化")
            }
            transform.localScale = new Vector3(faceDir, 1, 1);

        }
        else
        {
            //Debug.Log("不能转身");
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
            //Debug.Log("开始检测缓存");
            if (canJump)
            {
                
                    Debug.Log("缓存跳跃成功");
                    ChangeToJumpState();
                    break;//这里没有用return，而是用Break跳出这段循环；
            }
            counter -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        //if(counter<=0) Debug.Log("结束检测缓存");
    }






    #endregion

    #region 改变状态的代码（只会外部调用）
    public void StateOver()
    {
        /* 本方法用于停止当前状态，用于并不清楚可以从不确定的操作状态进入无操作状态
         * 
         * Stage1.攻击时用于通过条件判断退出攻击状态
         * Stage2.操纵机关时用于通过按键退出操纵状态
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




    #region 输入Input相关方法

    private void Gameplay_InputProcess()
    {
        /* 本方法用于在Gameplay时对输入进行预处理，
         * Step1。根据不同的具体状态分别进行不同的预处理
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
        /* 本方法用于对非特定状态下的方向输入进行预处理
         * 
         * Step1.判断当前是否isOnFloored，若是，进入Step2-a，若否，进入Step2-b
         * Step2-a.当水平输入方向与当前角色面朝方向一致且isOnWalled，则忽视其输入；否则原输入
         * Step2-b.当水平输入方向与当前角色面朝方向一致且isOnWalled或isForward，忽视当前输入；否则原输入
         * Step3.垂直输入方向直接原输入
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
        // 本方法用于Handle特定状态下对方向输入进行预处理，因此处也无法确定对应Handle的对象逻辑，故直接等于原输入，在具体的Interact脚本内进行再次预处理
        if (canAct)
        {
            horizontalInputVec = theInput.horizontalInputVec;
            verticalInputVec = theInput.verticalInputVec;
        }
    }

    #endregion


    #region RB相关方法
    public void ClearVelocity()
    {
        thisRB.velocity += new Vector2(-thisRB.velocity.x, -thisRB.velocity.y);
    }

    public void ClearYVelocity()//用于清除PlayerY轴上的速度
    {
        thisRB.velocity += new Vector2(0, -thisRB.velocity.y);

    }
    public void ClearXVelocity()//用于清除PlayerX轴上的速度
    {
        thisRB.velocity += new Vector2(-thisRB.velocity.x, 0);
    }
    public void HalfYVelocity()
    {
        thisRB.velocity += new Vector2(0, -thisRB.velocity.y / 2);
    }
    public void GroundVelocityLimit()//用于限制玩家移动速度上限
    {
        if (Mathf.Abs(thisRB.velocity.x) > horizontalMoveSpeedMax)
        {
            //Debug.Log("慢！");
            thisRB.velocity += new Vector2(faceDir * horizontalMoveSpeedMax - thisRB.velocity.x, 0f);
        }

    }

    public void GroundInertialClear()//用于清除玩家Idle状态下的残留惯性
    {
        if (Mathf.Abs(thisRB.velocity.x) < horizontalmoveThresholdSpeed) thisRB.velocity += new Vector2(-thisRB.velocity.x, 0f);
    }
    public void InertiaXVelocity()
    {
        //Debug.Log(Mathf.Lerp(Mathf.Abs(thisRB.velocity.x), moveSpeedMax, .1f));
        thisRB.velocity = new Vector2(faceDir * Mathf.Lerp(Mathf.Abs(thisRB.velocity.x), horizontalMoveSpeedMax, .1f), 0f);
        //Debug.Log("在惯性减速" + thisRB.velocity.x);
    }
    #endregion







    #region 接口实现
    #region 可存储内容对象相关方法

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
        //Debug.Log("PlayerSavaData保存成功");
    }
    #endregion


    #endregion

    #region 功能性方法

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
        /* 本方法用于道具相关计时，适用于普通帧
         * 
         * Work1.连续攻击计时
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
        ////Debug.Log("真击退了" + finalForce * _knockedBackDir.x);
        //// SetVelocity(finalForce * _knockedBackDir.x, finalForce * _knockedBackDir.y);
        //isUncontrol = true;
        //uncontrolCounter = uncontrolDuration;

    }

    public bool isStandOnPlatform()
    {

        // 本方法用于判断是否站立在Platform上，进一步用于解决落在下落平台上的bug
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
        // 本方法用于在初始阶段从SaveData写入脚本中
        attackAbility = thisPlayerSaveData.attackAblity;
        umbrellaAbility = thisPlayerSaveData.umbrellaAbility;
        candleAbility = thisPlayerSaveData.umbrellaAbility;
        lastSavePos = thisPlayerSaveData.GetLastSavePoint();
        InteractRelated_SaveItem(lastSavePos);
    }
    private void SceneLoad_SetPlayerState()
    {
        /* 本方法用于将玩家本身状态进行初始化
         * 
         * Step1.启用gameplay
         * Step2.玩家状态设为初始状态
         * Step3.将canAct设为是
         * 
         * 
         */
        isGameplay = true;
        canAct = true;
        stateMachine.Initialize(fallState);//可能需要做个方法来判断当前应该处于什么状态
        //mushroomPoint = thisPR.theGroundCheckpoint;
    }

    #endregion


}

