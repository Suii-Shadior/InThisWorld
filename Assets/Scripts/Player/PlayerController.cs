using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region 组件
    [HideInInspector] public Rigidbody2D thisRB;
    [HideInInspector] public PhysicsRelated thisPR;
    [HideInInspector] public PlayerAnimatorController thisAC;
    [HideInInspector] public PlayerFXController thisFX;
    [HideInInspector] public CharacterRelated thisCR;
    [SerializeField] private ControllerManager theCM;
    private LevelController theLevel;
    private DialogeController theDC;
    private InputController theInput;
    public Transform thisDP;

    #endregion
    #region 状态机相关
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallFallState wallFallState { get; private set; }
    public PlayerWallJumpState walljumpState { get; private set; }
    public PlayerHoldState holdState { get; private set; }
    public PlayerWallClimbState wallClimbState { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerUncontrolState uncontrolState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerBabbleState babbleState { get; private set; }
    #endregion
    #region 变量
    [Header("通用")]
    public bool isGameplay;
    public bool canAct;//无法进行操作，用于复活重置、剧情动画等完全无法进行任何操作的场景
    public float canActCounter;
    public float hurtUnactDuration;
    public float deadUnactDuration;
    public bool faceRight = true;//faceRight是可以通过输入或者外部情况进行转换的对象，是条件
    public int faceDir = 1;//faceDir是通过多个条件进行判断的面向，是结果
    public bool needTurnAround;//对于敌人以及演出时的玩家，需要通过该标识进行转身
    public int horizontalInputVec; 
    public int verticalInputVec;

    [Header("当前状态于数值")]
    public string nowState;
    public float horizontalMoveSpeed;
    public float horizontalMoveSpeedMax;
    public float horizontalmoveThresholdSpeed;
    public float verticalFallSpeedMax;
    public float verticalMoveSpeed;
    public float verticalMoveSpeedMax;
    public float verticalThresholdSpeed;
    
    [Header("行动能力")]
    public bool jumpAbility;
    public bool babbleAbility;
    public bool dashAbility;
    public bool holdAbility;

    [Header("行动状态")]
    public bool canTurnAround;
    public bool canHorizontalMove;
    public bool canVerticalMove;
    public bool canJump;
    public bool canDash;
    public bool canHold;
    public bool canWallFall;
    public bool canWallJump;
    public bool canWallClimb;
    public bool canWallClimbForward;//要记录下蔚蓝怎么处理的
    public bool canAttack;
    public bool canBabble;
    public bool canCooldown;    

    [Header("水平移动")]
    public float normalmoveSpeed;
    public float normalmoveSpeedMax;
    public float normalmoveThresholdSpeed;


    [Header("空中")]
    public float airmoveSpeed;
    public float airmoveSpeedMax;
    public float airFallSpeedMax;

    [Header("跳跃")]
    public float jumpForce;
    public float peakSpeed;
    public float canJumpCounter;
    public float canJumpLength;
    private float jumpBufferLength;

    [Header("冲刺")]
    public float dashCooldownCounter;
    public float dashCooldownLength;
    public float dashSpeed;
    public float dashDurationCounter;
    public float dashDurationLength;
    public float dashDir;
    public float dashEndDecrease;
    public bool dashEnd;

    [Header("墙上动作")]
    public float wallFallSpeed;
    public float wallFallSpeedMax;
    public float wallClimbSpeed;
    public float wallClimbSpeedMax;

    [Header("蹬墙跳")]
    public float wallJumpForce;
    public float wallJumpPostCounter;//用于walljump后进行操作锁定，
    public float wallJumpPostLength;

    [Header("惯性")]
    public bool keepInertia;


    [Header("雨伞")]
    public float babbleCounter;
    public float babbleDuration;
    public float babbleCooldownCounter;
    public float babbleCooldownDuration;

    [Header("攻击")]
    public int attackCounter;
    public float continueAttackCounter;
    public float continueAttackDuration;
    public Vector2[] attackMoveVec2s;
    public Transform attackCheckPoint;
    public float attackCheckRadius;


    [Header("限制控制")]//极难进行操作，被强制移动或击飞受伤时使用
    public float konckedBackCounter;
    public float knockedBackLength;
    public float knockedBackForce;
    public bool isUncontrol;
    public float uncontrolCounter;
    public float uncontrolDuration;


    [Header("受伤")]
    public float invinsibleCounter;
    public float invinsibleLength;

    [Header("死亡与复活")]
    public float sitCounter;
    //public float sitLength;
    
    //[Header("伸缩手")]
    //public GameObject ropePrefab;
    //public GameObject indicationPrefab;
    //public RopeController theRope;
    //public bool hadPulled;
    //public float indicationCounter;
    //public float indicationDuration;
    //public float ropeDisappearCounter;
    //public float ropeDisappearDuration;
    //public Vector2 ropeDir;

    //public PullTargetController thePullTarget;
    //public float pullingCounter;
    //public float pullingDuration;
    //public float pullingForce;
    //public float pullingSpeedMax;
    //public bool keepInertia;
    //public float keepInertiaCounter;
    //public float keepInertiaDuration;


    //public bool isPullingTowards;
    //public float pullForceMax;
    //public float pullForce;

    //public float pullCooldownCounter;
    //public float pullCooldownDuration;


    //[Header("遥控器")]
    //public BoomController theBoom;
    //public float boomUncontrolDuration;
    //public float boomCooldownCounter;
    //public float boomCooldownDuration;

    //[Header("熄灯器")]
    //[Header("Mashroom More")]

    #endregion






    private void Awake()
    {
        thisRB = GetComponent<Rigidbody2D>();
        thisPR = GetComponent<PhysicsRelated>();
        thisAC = GetComponentInChildren<PlayerAnimatorController>();
        thisFX = GetComponentInChildren<PlayerFXController>();
        thisCR = GetComponentInChildren<CharacterRelated>();
        //thisNorCol = GetComponent<CircleCollider2D>();
        //thisTempCol = GetComponentInChildren<CapsuleCollider2D>();

    }

    private void Start()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "isIdling");
        moveState = new PlayerMoveState(this, stateMachine, "isHorizontalMoving");
        jumpState = new PlayerJumpState(this, stateMachine, "isAiring");
        wallFallState = new PlayerWallFallState(this, stateMachine, "isWallFalling");
        walljumpState = new PlayerWallJumpState(this, stateMachine, "isJumping");//等相应素材
        holdState = new PlayerHoldState(this, stateMachine, "isHolding");
        wallClimbState = new PlayerWallClimbState(this, stateMachine, "isWallClimbing");
        attackState = new PlayerAttackState(this, stateMachine, "isAttacking");
        dashState = new PlayerDashState(this, stateMachine, "isDashing");
        airState = new PlayerAirState(this, stateMachine, "isAiring");
        uncontrolState = new PlayerUncontrolState(this, stateMachine, "isAiring");//临时用
        babbleState = new PlayerBabbleState(this, stateMachine, "isBabbling");
        deadState = new PlayerDeadState(this, stateMachine, "isDeading");

        theLevel = theCM.theLevel;
        theDC = theCM.theDC;
        theInput = theCM.theInput;
        OriginPlayerData();
    }

    // Update is called once per frame
    private void Update()
    {
        MovementVecUpdate();
        if (isGameplay)
        {
            GameplayCooldownCount();
            CanActCount();
        }
    }


    private void FixedUpdate()
    {
        if (isGameplay)
        {
            thisPR.PRUpdate();
            stateMachine.currentState.Update();
            FaceDirUpdate();
        }
    }

    public void TurnAround()
    {
        if (canTurnAround)
        {
            faceRight = !faceRight;
            canTurnAround = true;
        }
    }

    public void Unact(float _unactDuration)
    {
        canAct = false;
        canActCounter = _unactDuration;
    }



    public virtual void CanActCount()
    {
        if (canActCounter > 0 && !canAct)
        {
            canActCounter -= Time.deltaTime;
        }
        else
        {
            if (!theDC.isDialogue)
                canAct = true;
        }
    }


    private void OriginPlayerData()
    {
        thisRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        thisRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        isGameplay = true;
        canAct = false;
        if (theLevel.currentSceneName == "Level1")
        {
            airmoveSpeed = 0;

        }
        verticalFallSpeedMax = airFallSpeedMax;
        stateMachine.Initialize(airState);//可能需要做个方法来判断当前应该处于什么状态
        //mushroomPoint = thisPR.theGroundCheckpoint;
        SkillFresh();
    }

    private void MovementVecUpdate()
    {
        horizontalInputVec = theCM.theInput.horizontalInputVec;
        verticalInputVec = theCM.theInput.verticalInputVec;
    }

    public void SkillFresh()//强制刷新所有技能
    {
        if (babbleAbility) canBabble = true;
    }

    public void StateEndSkillFresh()
    {
        if (babbleAbility && babbleCooldownCounter < 0) canBabble = true;
    }

    public void TurnOffStandBox()
    {
        //if (不需要临时碰撞体) thisTempCol.enabled = false;
        //thisNorCol.isTrigger = false;
    }
    public void TurnOnStandBox()
    {
        //if (需要临时碰撞体) thisTempCol.enabled = true;
        //thisNorCol.isTrigger = true;
    }

    public void DisablizeColliders()
    {
        //thisTempCol.enabled = false;
        //thisNorCol.enabled = false;
    }

    public void AblizeColliders()
    {
        //thisTempCol.enabled = false;
        //thisNorCol.enabled = true;
        //thisNorCol.isTrigger = false;

    }




    #region 外部改变状态方法
    public void StateOver()
    {
        if (thisPR.IsOnGround()) stateMachine.ChangeState(moveState);
        else stateMachine.ChangeState(airState);
    }

    public void ChangeToHorizontalMoving()
    {
        Debug.Log("这里?");
        stateMachine.ChangeState(moveState);
    }

    public void ChangeToAirState()
    {
        stateMachine.ChangeState(airState);
    }
    public void ChangeToJumpState()
    {
        stateMachine.ChangeState(jumpState);

    }
    public void ChangeToBabbleState()
    {
        if (babbleAbility && canBabble)
            stateMachine.ChangeState(babbleState);
    }

    public void ChangeToUncontrolState()
    {
        stateMachine.ChangeState(uncontrolState);
    }


    public void ChangeToDeadState()
    {
        stateMachine.ChangeState(deadState);
    }

    public void ChangeToIdleState()
    {
        stateMachine.ChangeState(idleState);
    }



    public void PlayerReset()
    {
        theLevel.Respawn();
    }

    public void AirmoveRecover()
    {
        airmoveSpeed = 20;
    }
    #endregion
    #region 个性方法
    public void ActivateLevelInteractiveable()
    {
        //触发画面内可遥控机关
    }
    #endregion

    #region Gameplay方法
    #region Can判断
    public void WhetherCanJump()
    {
        if (canJumpCounter >= 0f)
        {
            canJumpCounter -= Time.deltaTime;
            if (stateMachine.currentState == attackState) 

            {
                canJump = false;
            }
            else
            {
                Debug.Log("???");
                canJump = true;
            }
        }
        else
        {
            canJump = false;
        }

    }


    public void WhetherCanHold()
    {
        if (thisPR.isWall && stateMachine.currentState != dashState) canHold = true;
        else canHold = false;
    }
    public void WhetherCanWallFall()
    {
        if (!thisPR.IsOnGround() && thisPR.isWall && thisRB.velocity.y <= 0.1 && stateMachine.currentState != dashState) canWallFall = true;
        else canWallFall = false;
    }

    public void WhetherCanDash()//Todo:待修改
    {
        if (dashCooldownCounter < 0)
        {
            if (stateMachine.currentState == attackState) canDash = false;
            else canDash = true;
        }
        else
        {
            dashCooldownCounter -= Time.deltaTime;
            canDash = false;
        }
    }
    public void WhetherCanWallVeritalForward()
    {
        if (thisPR.isBackWall)
        {
            canWallClimbForward = true;
        }
        else
        {
            canWallClimbForward = false;
        }
    }
    //public void WhetherCanBladeAtttack()
    //{
    //    if (!thisPR.isWall && !isAttack && !isPosForce)
    //    {

    //    }
    //}


    public void RefreshCanJump()
    {
        canJumpCounter = canJumpLength;
    }
    public void RefreshCanDash()
    {
        canDash = true;
    }

    #endregion 

    #region 动作
    public void FaceDirUpdate()//单看不合理，但是结合MoveToDir来看可以实现滑行？
    {
        if (stateMachine.currentState != uncontrolState)
        {
            if (canTurnAround)
            {
                if (horizontalInputVec > 0) faceRight = true;//MoveToDir,用于判断玩家想要移动的方向
                else if (horizontalInputVec < 0) faceRight = false;
            }
            faceDir = (int)transform.localScale.x;
            if (thisRB.velocity.x > 0 && faceRight) faceDir = 1;
            else if (thisRB.velocity.x < 0 && !faceRight) faceDir = -1;
            transform.localScale = new Vector3(faceDir, 1, 1);
        }
    }



    public void JumpBufferCheck()
    {
        StartCoroutine(JumpBufferCheckCo(jumpBufferLength));
    }
    private IEnumerator JumpBufferCheckCo(float counter)
    {
        while (counter > 0)
        {
            //Debug.Log("开始检测缓存");
            //if (canJump)
            //{
            //    if (stateMachine.currentState == holdState || stateMachine.currentState == wallfallState) stateMachine.ChangeState(walljumpState);
            //    else stateMachine.ChangeState(jumpState);
            //    //Debug.Log("缓存跳跃成功");
            //    break;
            //}
            yield return new WaitForSeconds(Time.deltaTime);
            //counter -= Time.deltaTime;
        }
        //if(counter<=0) Debug.Log("结束检测缓存");
    }
    public void Fall()
    {

        if (thisRB.velocity.y < -verticalFallSpeedMax)
        {
            thisRB.velocity += new Vector2(0, -verticalFallSpeedMax - thisRB.velocity.y);
            //Debug.Log(thisRB.velocity.y);
        }

    }

    #region WallAct相关

    public void Hold()
    {
        ClearVelocity();
        verticalFallSpeedMax     = 0f;
        thisPR.GravityLock(0);
        thisPR.isHolding = true;//这两个不是在PR其实也是锁0重力 看看需不需要优化
        canTurnAround = false;
    }

    public void EndHold()
    {
        verticalFallSpeedMax = airFallSpeedMax;
        thisPR.GravityUnlock();
        thisPR.isHolding = false;
        canTurnAround = true;
    }

    public void WallJump()
    {
        wallJumpPostCounter = wallJumpPostLength;
        thisPR.GravityLock(2f);
        thisRB.AddForce(new Vector2(-faceDir, 2) * wallJumpForce, ForceMode2D.Impulse);
        canTurnAround = false;
        faceRight = !faceRight;
        thisPR.LeaveWall();
    }
    public void WallJumpdEnd()
    {
        thisPR.GravityUnlock();
        canHorizontalMove = true;
        WhetherCanHold();
        canTurnAround = true;
    }


    public void ClimbPrepare()
    {
        if (stateMachine.currentState == wallClimbState)
        {

            thisPR.GravityLock(0);
            canTurnAround = false;
            thisPR.isHolding = true;
            verticalMoveSpeed = wallClimbSpeed;
            verticalMoveSpeedMax = wallClimbSpeedMax;
        }
        //else if (stateMachine.currentState == ropeClimbState)
        //{
        //    climbSpeed = wallClimbSpeed;
        //    climbSpeedMax = wallClimbSpeedMax;
        //}
    }

    public void ClimbEnd()
    {
        {
            if (stateMachine.currentState == wallClimbState)
            {

                thisPR.GravityUnlock();
                canTurnAround = true;
                thisPR.isHolding = false;
            }
            //else if (stateMachine.currentState == ropeClimbState)
            //{
            //    climbSpeed = wallClimbSpeed;
            //    climbSpeedMax = wallClimbSpeedMax;
            //}
        }
    }

    public void Climb()
    {
        if (Mathf.Abs(thisRB.velocity.y + verticalInputVec * verticalMoveSpeed * Time.deltaTime) < verticalMoveSpeedMax)//在考虑到的情况中，该方案和上一句效果相同
        {
            if (Mathf.Abs(thisRB.velocity.y) < verticalThresholdSpeed) thisRB.velocity += new Vector2(0f, verticalInputVec * (verticalThresholdSpeed + verticalMoveSpeed * Time.deltaTime));
            else
                thisRB.velocity += new Vector2(0f, verticalInputVec * verticalMoveSpeed * Time.deltaTime);
        }
    }
    #endregion


    #region Attack相关



    public void AtttackOnGround()
    {
        canAttack = false;
        thisAC.currentAttack = thisAC.bladeAttackOnGoundIdentity[attackCounter];

    }



    public void BladeAttackEnd()
    {
        thisPR.GravityUnlock();
        continueAttackCounter = continueAttackDuration;
    }








    #endregion

    #region Dash相关
    public void Dash()
    {
        //Debug.Log("冲！");
        ClearVelocity();
        thisPR.GravityLock(0f);
        dashEnd = false;
        thisRB.velocity += new Vector2(-thisRB.velocity.x + dashDir * dashSpeed, 0);
        //canDoubleJumpTemp = canDoubleJump;
        dashCooldownCounter = dashCooldownLength;
        dashDurationCounter = dashDurationLength;
        //isDashing = true;

    }
    public void DashKeep()
    {
        if (dashDurationCounter > 0.1f)
        {

            //thisRB.velocity += new Vector2(faceDir * dashSpeed * Time.deltaTime, -thisRB.velocity.y);
            dashDurationCounter -= Time.deltaTime;
        }
        else if (dashDurationCounter > 0f)
        {
            dashEnd = true;
            dashDurationCounter -= Time.deltaTime;
        }
        else
        {
            if (thisPR.IsOnGround())
            {
                stateMachine.ChangeState(idleState);
                return;
            }
            else if (thisPR.isWall)
            {
                Debug.Log("判断了的");
                WhetherHoldOrWallFall();
            }
            else
            {
                stateMachine.ChangeState(airState);
                return;
            }
        }
    }
    public void DashEnd()
    {

        thisAC.DashTrigger();
        dashEnd = false;
        thisRB.velocity += new Vector2(dashEndDecrease * -thisRB.velocity.x, -thisRB.velocity.y);
        thisPR.GravityUnlock();
        //canDoubleJump = canDoubleJumpTemp;
    }
    #endregion


    #region
    public void DeadEnter()
    {
        canAct = false;

        theLevel.Respawn();
    }

    public void DeadEnd()
    {

    }
    public void ChangeToIdle() => stateMachine.ChangeState(idleState);
    //public void ChangeToStand() => stateMachine.ChangeState(standUpState);
    #endregion

    #region 暂时不用的动作
    //public void NewAttack()
    //{
    //    ClearVelocity();
    //    Debug.Log("进入时蓄力" + accumulateCounter);
    //    accumulateCounter = 0;
    //    attackCombo = 0;
    //    Debug.Log("开始蓄力蓄力" + accumulateCounter);
    //}
    //public void Attack()
    //{

    //    if (attackCombo < 2 && canAttack)
    //    {
    //        canAttack = false;
    //        attackCombo++;
    //    }

    //    thisAC.currentAttack = thisAC.AttackIdentity[attackCombo];
    //    thisAC.AttackTrigger();
    //}
    //public void Accumulate()
    //{
    //    if (Input.GetKey(KeyCode.Z) && attackCombo == 0) accumulateCounter += Time.deltaTime;

    //}
    //public void Slash()
    //{
    //    canAttack = false;
    //    canDash = false;
    //    isSlash = true;
    //    thisAC.currentAttack = thisAC.AttackIdentity[0];
    //    thisAC.AttackTrigger();
    //}
    //public void AttackEnd()
    //{
    //    attackCombo = 0;
    //    isSlash = false;
    //    thisAC.AttackTrigger();
    //}
    //private void Slide()
    //{
    //    isCrouching = false;
    //    isSliding = true;
    //    slideSpeed = slideStartSpeed;
    //    //int slideDir = SlideDir();
    //    slideCounter = slideLength;
    //    //Debug.Log("开始滑铲" + "  "+ slideCounter);
    //    thisRB.velocity += new Vector2(-thisRB.velocity.x + slideStartSpeed * Time.deltaTime, 0f);
    //}
    //private void SlideKeep()
    //{
    //    if (slideCounter > 0f)
    //    {
    //        thisRB.velocity += new Vector2(slideSpeed * faceRight * Time.deltaTime, 0f);
    //        slideSpeed = Mathf.Lerp(slideSpeed, 0f, slideLerp);
    //        slideCounter -= Time.deltaTime;
    //    }
    //    else
    //    {
    //        SlideEnd();
    //        ClearVelocity();
    //    }
    //}
    //private void SlideEnd()
    //{
    //    isSliding = false;
    //    if (thisPR.isGround)
    //    {
    //        WhetherCrouch();
    //    }
    //    else
    //    {
    //        isCrouching = false;
    //        //Debug.Log("结束滑铲");
    //        moveSpeed = wanderSpeed;
    //        moveSpeedMax = runSpeedMax;
    //    }
    //}
    //private void Crouch()
    //{
    //    //isCrouching = true;
    //    moveSpeed = crouchSpeed;
    //    moveSpeedMax = crouchSpeedMax;
    //}
    //private void RiseUp()
    //{
    //    //Debug.Log("起身");
    //    isCrouching = false;
    //    moveSpeed = wanderSpeed;
    //    moveSpeedMax = runSpeedMax;
    //}



    #endregion
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
    #region PR相关方法

    public void WhetherHoldOrWallFall()
    {

        if (stateMachine.currentState == wallFallState)
        {
            if (canAct && canHold && theInput.WhetherZPressing())
            {
                if (canVerticalMove && verticalInputVec != 0)
                {
                    stateMachine.ChangeState(wallClimbState);
                    return;
                }
                else
                {
                    stateMachine.ChangeState(holdState);
                    return;
                }
            }
            //wallJump会掉头且有冻结时间canHold在一定时间内false,为了在墙上或墙边按下跳跃能够跳出来而不是在跳跃瞬间又判定抓住
            else if (!canAct || !canWallFall || horizontalInputVec != faceDir)
            {
                stateMachine.ChangeState(airState);
                return;
            }
        }
        else if (stateMachine.currentState == wallClimbState)
        {
            if (canAct && canHold && theInput.WhetherZPressing())
            {
                if (!canVerticalMove || verticalInputVec == 0)
                {
                    stateMachine.ChangeState(holdState);
                    return;
                }
            }
            else if (canAct && canWallFall && horizontalInputVec == faceDir)
            {
                stateMachine.ChangeState(wallFallState);

            }
            else
            {
                stateMachine.ChangeState(airState);
                return;
            }
        }
        else if (stateMachine.currentState == holdState)
        {
            if (canAct && canVerticalMove && verticalInputVec != 0)
            {
                stateMachine.ChangeState(wallClimbState);
                return;
            }
            else if (canAct && canWallFall && horizontalInputVec == faceDir)
            {
                stateMachine.ChangeState(wallFallState);
                return;
            }
            else
            {
                stateMachine.ChangeState(airState);
                return;
            }
        }
        else if (stateMachine.currentState == jumpState || stateMachine.currentState == walljumpState || stateMachine.currentState == airState)
        {
            if (canAct && canHold && theInput.WhetherZPressing())
            {
                if (canVerticalMove && verticalInputVec != 0)
                {
                    stateMachine.ChangeState(wallClimbState);
                    return;
                }
                else
                {
                    stateMachine.ChangeState(holdState);
                    return;
                }
            }
            if (canAct && canWallFall && horizontalInputVec == faceDir)
            {
                stateMachine.ChangeState(wallFallState);
                return;
            }
            else
            {
                stateMachine.ChangeState(airState);
                return;
            }
        }
        else
        {
            stateMachine.ChangeState(airState);
            return;
        }
    }
    public void IsPeak()//用来判断空中Player所在的位置，帮助PR在空中实现重力的调整
    {
        if (Mathf.Abs(thisRB.velocity.y) < peakSpeed)
        {
            //Debug.Log("我在飞！");
            thisPR.isPeak = true;
        }
        else
        {
            thisPR.isPeak = false;
        }
    }


    #endregion
    #region 功能性方法
    private void GameplayCooldownCount()
    {
        if (babbleCooldownCounter > 0)
        {
            babbleCooldownCounter -= Time.deltaTime;
        }
        else
        {
            if (canCooldown)
                canBabble = true;
        }
        if (uncontrolCounter >= 0)
        {
            uncontrolCounter -= Time.deltaTime;
        }
        else
        {
            isUncontrol = false;
        }
        if (dashCooldownCounter > 0)
        {
            dashCooldownCounter -= Time.deltaTime;
        }
        else
        {

        }

        if (invinsibleCounter > 0)
        {
            invinsibleCounter -= Time.deltaTime;
        }
    }

    public void CurrentStateOver() => stateMachine.currentState.CurrentStateEnd();
    //public void ChangeToKnockBack() => stateMachine.ChangeState(knockedBackState);



    public void KnockedBack(Vector2 _knockedBackDir)
    {
        float finalForce = knockedBackForce / thisRB.mass;
        thisRB.AddForce(finalForce * _knockedBackDir, ForceMode2D.Impulse);
        //Debug.Log("真击退了" + finalForce * _knockedBackDir.x);
        // SetVelocity(finalForce * _knockedBackDir.x, finalForce * _knockedBackDir.y);
        isUncontrol = true;
        uncontrolCounter = uncontrolDuration;

    }
    #endregion

    #endregion

    #region VideoPlay方法


    public void VisualTurnAround()
    {

        faceRight = !faceRight;
        faceDir = -faceDir;
    }

    //private void StopAction()
    //{
    //    switch (thePlayerState)
    //    {
    //    }
    //}
    #endregion

    #region Character相关方法





    public void TakeDamage(int _damageValue)//只处理与对象数值相关的内容
    {
        //thisFX.StartCoroutine(thisFX.FlashEffect());
        if (invinsibleCounter <= 0 && thisCR.currentHP > 0)
        {
            thisCR.currentHP -= _damageValue;
            if (thisCR.currentHP > 0) Hurt();
            else stateMachine.ChangeState(deadState);
        }
        //Debug.Log("对玩家造成" + _damageValue + "点伤害");
    }



    public void Hurt()
    {

        //动画、音乐特效
        canActCounter = hurtUnactDuration;
        invinsibleCounter = invinsibleLength;

    }
    #endregion
}
