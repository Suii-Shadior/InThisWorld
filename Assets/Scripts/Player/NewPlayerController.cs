using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
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
    private NewPlayerStateMachine stateMachine;
    private NewPlayerIdleState idleState;
    private NewPlayerRunState runState;
    private NewPlayerRiseState jumpState;
    private NewPlayerFallState fallState;

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
    public bool needTurnAround;//需要通过该标识进行转身
    //isUncontroling用于表示非uncontrolState中部分行为受限的表示，比如蹬墙跳时候的短时间内不能转身，通常满足了一定条件后该状态就会取消，但是又不改变当前状态；
    //uncontrolState用于表示当前状态下角色所有行为受限。比如受伤后的短暂时间内什么都不能干，通常经过了一段时间后该状态就会取消
    //unAct用于使用在暂停，或者复活、剧情动画时玩家没有可能在相当长的不确定时间内不能操作角色，只有特定的行为完成后才能取消
    public bool isUncontroling;
    public int horizontalInputVec;
    public int verticalInputVec;

    [Header("当前状态于数值")]
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

    [Header("行动能力")]
    public bool jumpAbility;
    public bool umbrellaAbility;
    public bool dashAbility;


    [Header("行动状态")]
    public bool canHorizontalMove;
    public bool canVerticalMove;
    public bool canJump;
    public bool canDash;
    public bool canHold;
    public bool canWallJump;
    public bool canAttack;

    [HideInInspector] public bool canTurnAround;
    [HideInInspector] public bool canWallClimbForward;//要记录下蔚蓝怎么处理的

    [Header("技能状态")]
    public bool canBabble;
    public bool canCooldown;

    [Header("水平移动")]
    public float normalmoveAccleration;
    //public float normalmoveSpeed;
    public float normalmoveSpeedMax;
    public float normalmoveThresholdSpeed;


    [Header("空中")]
    public float airmoveSpeed;
    public float airmoveSpeedMax;
    public float airmoveThresholdSpeed;
    public float airFallSpeedMax;

    [Header("跳跃")]
    public float jumpForce;
    public float peakSpeed;
    public float canJumpCounter;
    public float canJumpLength;
    private float jumpBufferLength;

    [Header("雨伞")]
    public float umbrellaMoveSpeed;
    public float umbrellaMoveSpeedMax;
    public float umbrellaFallSpeedMax;

    [Header("攻击")]
    public int attackCounter;
    public float continueAttackCounter;
    public float continueAttackDuration;
    public float attackCooldownCounter;
    public float attackCooldownDuration;
    public Vector2[] attackMoveVec2s;//不同的攻击动作不同的攻击惯性



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
    #endregion


    private void Awake()
    {
        thisRB = GetComponent<Rigidbody2D>();
        thisPR = GetComponent<PhysicsRelated>();
        thisAC = GetComponentInChildren<PlayerAnimatorController>();
        thisFX = GetComponentInChildren<PlayerFXController>();
        thisCR = GetComponentInChildren<CharacterRelated>();

    }
    // Start is called before the first frame update
    private void Start()
    {
        stateMachine = new NewPlayerStateMachine();
        idleState = new NewPlayerIdleState(this, stateMachine, "isIdling");
        runState = new NewPlayerRunState(this, stateMachine, "isHorizontalMoving");
        jumpState = new NewPlayerRiseState(this, stateMachine, "isJumping");
        fallState = new NewPlayerFallState(this, stateMachine, "isFalling");
        theLevel = theCM.theLevel;
        theDC = theCM.theDC;
        theInput = theCM.theInput;
        OriginPlayerData();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void FixedUpdate()
    {
        MovementVecUpdate();
    }

    private void OriginPlayerData()
    {
        thisRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        thisRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        isGameplay = true;
        canAct = false;
        verticalFallSpeedMax = airFallSpeedMax;
        stateMachine.Initialize(fallState);//可能需要做个方法来判断当前应该处于什么状态
        //mushroomPoint = thisPR.theGroundCheckpoint;
    }



    #region 改变状态的代码
    public void StateOver()
    {
        if (thisPR.IsOnGround()) ChangeToIdleState();
        else stateMachine.ChangeState(fallState);
    }
    public void ChangeToIdleState()
    {
        stateMachine.ChangeState(idleState);
    }


    public void ChangeToHorizontalMoving()
    {
        Debug.Log("这里?");
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
    //public void ChangeToUmbrellaState()
    //{
    //    if (umbrellaAbility && canBabble)
    //        stateMachine.ChangeState(babbleState);
    //}

    //public void ChangeToUncontrolState()
    //{
    //    stateMachine.ChangeState(uncontrolState);
    //}


    //public void ChangeToDeadState()
    //{
    //    stateMachine.ChangeState(deadState);
    //}
    public void PlayerReset()
    {
        theLevel.Respawn();
    }
    #endregion


    #region Gameplay方法
    #region Can判断
    public void WhetherCanJumpOrWallJump()//因为共用跳跃键，互斥，所以在一个判断
    {
        if (canJumpCounter > 0f)
        {

            if (!thisPR.IsOnGround())
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
            }
            else
            {
                canJump = true;
                canWallJump = false;
            }
            canJumpCounter -= Time.deltaTime;
        }
        else
        {
            //脱台跳计时置0至此来实现无法跳跃
            //所以这里可以进行跳跃下落和打伞时候的蹭墙判断
            canJump = false;
            canWallJump = false;
        }

    }

    public void WhetherCanAttack()
    {
        if (attackCooldownCounter<0)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
    }


    public void RefreshCanJump()
    {
        canJumpCounter = canJumpLength;
    }

    #endregion 

    #region 动作
    public void FaceDirUpdate()
    {

        faceDir = (int)transform.localScale.x;
        if (horizontalInputVec > 0)
        {
            faceRight = true;
            faceDir = faceRight ? 1 : -1;
        }

        else if (horizontalInputVec < 0)
        {
            faceRight = false;
            faceDir = faceRight ? 1 : -1;
        }
        else
        {
            //Debug.Log("朝向不变");
        }
        transform.localScale = new Vector3(faceDir, 1, 1);
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
            if (canJump)
            {
                //if (stateMachine.currentState == holdState || stateMachine.currentState == wallfallState) stateMachine.ChangeState(walljumpState);
                //else 
                ChangeToJumpState();
                //Debug.Log("缓存跳跃成功");
                break;
            }
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




    #endregion

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

        if (uncontrolCounter >= 0)
        {
            uncontrolCounter -= Time.deltaTime;
        }
        else
        {
            isUncontrol = false;
        }
        if (continueAttackCounter > 0)
        {
            continueAttackCounter -= Time.deltaTime;
        }

        if (attackCooldownCounter > 0)
        {
            attackCooldownCounter -= Time.deltaTime;
        }
        else
        {
            canAttack = false;
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



    private void MovementVecUpdate()
    {
        horizontalInputVec = theCM.theInput.horizontalInputVec;
        verticalInputVec = theCM.theInput.verticalInputVec;
    }

}

