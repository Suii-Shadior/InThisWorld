using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using CMRelatedEnum;

public class PhysicsRelated : MonoBehaviour
{
    #region 组件及其他
    private Rigidbody2D thisRB;
    private Collider2D thisCol;
    public PhysicsConfig physicsConfig;
    #endregion
    [Header("Physical State")]
    public PlayerPhysicalState nowGrivatyState;

    [Header("Physics Setting")]
    public Transform theGroundCheckpoint;
    public Transform theWallCheckpoint;
    public Transform theForwardCheckpoint;
    public Transform theBackWallCheckpoint;
    public Transform theHeadCheckpoint;
    [Tooltip("通常地面")]
    public LayerMask theCeil;
    [Tooltip("所有需要认定为可以被站立的平台或者对象都应该包含其中")]
    public LayerMask theFloor;
    [Tooltip("通常地面？")]
    public LayerMask theWall;
    //public LayerMask theTrueGround;

    [Header("PhysicsCheck")]
    [SerializeField] private bool isRaycastGround;
    private bool wasRaycastGround;
    [SerializeField] private bool isWall;
    private bool wasWall;
    [SerializeField] private bool isForward;
    [SerializeField] private bool isBackWall;
    private bool wasBackWall;
    [SerializeField]private bool isHead;
    private bool wasHead;
    private RaycastHit2D theRaycastCol;




    [Header("PhysicsChange")]
    public PhysicsMaterial2D normalMat;
    public PhysicsMaterial2D slipperMat;
    public bool gravityLock;
    //public bool needGroundChange;
    //public bool needWallChange;
    //public bool needAirChange;
    [Header("PhysicsChange Detail")]
    private bool riseGravityed;
    private bool fallGravityed;


    [Header("Other")]
    public bool hasWall;
    public float hasWallCounter;
    private bool hasForward;
    public float hasForwardCounter;
    public bool hasRaycastGround;
    public float hasRaycastGroundCounter;



    private void Awake()
    {
        /*
         * Work1.组件获取
         * 
         * 
         */
        thisRB = GetComponent<Rigidbody2D>();
        thisCol = GetComponent<Collider2D>();
    }
    public void PRUpdata()
    {
        /* 本方法用于完成物理检测及更新相关内容，适用于脚本普通帧
         * 
         * TODO：完善一套可以服用在不同对象Controller上的PR脚本
         * 
         * Step1.所有物理检测运行
         * Step2.根据物理检测及运动状态判定当前的物理状态
         * Step3.更新物理状态
         * 
         */
        PhysicsRelated_Check();
        PhysicsRelated_StateJudgement();
        PhysicsRelated_ParaUpdata();
    }


    //private void GroundedCheck()
    //{
    //    WhetherHadGrounded();//设定离地时间，用来来调节离地瞬间的单纯图层检测的误判
    //    wasGrounded = isGrounded;
    //    isGrounded = hasGrounded && Physics2D.OverlapCircle(theGroundCheckpoint.position, physicsConfig.theGroundCheckRadius, theTrueGround);//通过图层检测及前述离地时间判断本帧的触地情况
    //}

    #region 检测相关方法
    private void RaycastGroundCheck()
    {
        /* 本方法用于三射线地面检测，适用于脚本帧运行
         * 
         * Step1.通过计时器来解决离地时检测方法在范围内仍然能够检测到的方法存在的问题
         * Step2.判断是否应该进行射线检测
         *  a.若是，三射线分别检测，以中心射线为最高优先级，获取是否检测到地面，以及获取检测对象碰撞体
         *  b.若否，不进行检测，直接设定为并未检测到地面，同时检测对象碰撞体置空
         */

        WhetherHadRaycastGround();

        wasRaycastGround = isRaycastGround;
        if (hasRaycastGround)
        {
            Vector2 centerPos = new Vector2(
                theGroundCheckpoint.position.x + physicsConfig.raysCheckStep,
                theGroundCheckpoint.position.y
            );
            Vector2 leftPos = new Vector2(
                theGroundCheckpoint.position.x - physicsConfig.raysCheckStep,
                theGroundCheckpoint.position.y
            );
            Vector2 rightPos = new Vector2(
                theGroundCheckpoint.position.x + physicsConfig.raysCheckStep,
                theGroundCheckpoint.position.y
            );
            RaycastHit2D theBodyRaycastCheckCol = Physics2D.Raycast(centerPos, Vector2.down, physicsConfig.raycastLength, theFloor);
            RaycastHit2D theLeftRaycastCheckCol = Physics2D.Raycast(leftPos, Vector2.down, physicsConfig.raycastLength, theFloor);
            RaycastHit2D theRightRaycastCheckCol = Physics2D.Raycast(rightPos, Vector2.down, physicsConfig.raycastLength, theFloor);

            if (theBodyRaycastCheckCol.collider!=null){
                isRaycastGround = true;
                theRaycastCol = theBodyRaycastCheckCol;
            }
            else if(theLeftRaycastCheckCol.collider != null)
            {
                isRaycastGround = true;
                theRaycastCol = theLeftRaycastCheckCol;
            }
            else if (theRightRaycastCheckCol.collider != null)
            {
                isRaycastGround = true;
                theRaycastCol = theRightRaycastCheckCol;
            }
            else
            {
                isRaycastGround = false;
                theRaycastCol = new RaycastHit2D();
            }

        }
        else
        {
            isRaycastGround = false;
            theRaycastCol = new RaycastHit2D();
        }
    }

    private void WallCheck()//用于实现墙面判定和触发，但是由于上墙离地都是在当前状态中Update持续更新，且不是唯一的条件，所以ifNeed判定在次作用不大，
    {
        /*
         * 本方法用于前方墙壁检测，适用于脚本帧运行
         * 
         * Step1.通过计时器协助来解决离墙计时检测方法存在的问题
         * Step2.进行物理检测
         * 第二句其实等同于if(hasWall)isWall = Physics2D.OverlapCircle(theWallCheckpoint.position, physicsConfig.theWallCheckRadius, theWall);else isWall = false;
         */
        WhetherHadWall();
        wasWall = isWall;
        isWall = hasWall && Physics2D.OverlapCircle(theWallCheckpoint.position, physicsConfig.theWallCheckRadius, theWall);
        

    }

    private void ForwardCheck()
    {
        // 本方法用于前方地面检测，适用于脚本帧运行,因主要用于动画分支/操作条件，无逻辑作用，故不用考虑离地时和上帧比对的问题

        isForward = Physics2D.OverlapCircle(theForwardCheckpoint.position, physicsConfig.theForwardCheckRadius, theFloor);
    }

    private void BackWallCheck()
    {
        //本方法用于背后实体检测，适用于脚本帧运行，因主要用于挤压状态判断，仅仅需要考虑上帧比对问题
        wasBackWall = isBackWall;
        isBackWall = Physics2D.OverlapCircle(theBackWallCheckpoint.position, physicsConfig.theBackWallCheckRadius, theFloor);
    }
    private void HeadCheck()
    {
        //本方法用于头顶实体检测，适用于脚本帧运行
        wasHead = isHead;
        isHead = Physics2D.OverlapCircle(theHeadCheckpoint.position, physicsConfig.theHeadCheckRadius, theCeil);
    }
    private void WhetherHadWall()
    {
        if (!hasWall)
        {
            if (hasWallCounter > 0f) hasWallCounter -= Time.deltaTime;
            else
            {
                hasWall = true;
            }
        }
    }
    private void WhetherHadForward()
    {
        if (!hasForward)
        {
            if (hasForwardCounter > 0f) hasForwardCounter -= Time.deltaTime;
            else
            {
                hasForward = true;
            }
        }
    }
    private void WhetherHadRaycastGround()
    {
        if (!hasRaycastGround)
        {
            if (hasRaycastGroundCounter > 0f) hasRaycastGroundCounter -= Time.deltaTime;
            else
            {
                hasRaycastGround = true;
            }
        }
    }
 
    //private void WhetherHadGrounded()
    //{
    //    if (!hasGrounded)
    //    {
    //        if (hasGroundedCounter > 0f) hasGroundedCounter -= Time.deltaTime;
    //        else
    //        {
    //            hasGrounded = true;
    //        }
    //    }
    //}
    #endregion

    #region 物理参数更新方法
    private void PhysicsRelated_Check()
    {
        //本方法用于所有物理检测的更新
        //GroundedCheck();
        RaycastGroundCheck();
        WallCheck();
        ForwardCheck();
        BackWallCheck();
        HeadCheck();
        PhysicsRelated_StateJudgement();
        PhysicsRelated_ParaUpdata();
    }

    private void PhysicsRelated_StateJudgement()
    {
        //本方法用于当前物理状态的判断
        if (isRaycastGround)
        {
            nowGrivatyState = PlayerPhysicalState.isStanding;
        }
        else
        {
            if (thisRB.velocity.y > physicsConfig.peakSpeed)
            {
                nowGrivatyState = PlayerPhysicalState.isRising;
            }
            else if (thisRB.velocity.y < -physicsConfig.peakSpeed)
            {
                nowGrivatyState = PlayerPhysicalState.isFalling;
            }
            else
            {
                nowGrivatyState = PlayerPhysicalState.isPeak;
            }
        }

    }

    private void PhysicsRelated_ParaUpdata()
    {
        //本方法用于物理参数的更新
        switch (nowGrivatyState)
        {
            case PlayerPhysicalState.isRising:
                thisCol.sharedMaterial = slipperMat;
                if (!riseGravityed)
                {
                    thisRB.gravityScale = physicsConfig.riseGravity;
                    riseGravityed = true;
                }
                else
                {
                    if(thisRB.gravityScale< physicsConfig.fallMaxGravity)
                    {
                        thisRB.gravityScale += thisRB.gravityScale * physicsConfig.risegravityMultiplier;

                    }
                    else
                    {
                        thisRB.gravityScale = physicsConfig.fallMaxGravity;
                    }
                }
                thisRB.drag = physicsConfig.airDrag;
                break;
            case PlayerPhysicalState.isFalling:
                thisCol.sharedMaterial = slipperMat;
                if (!fallGravityed)
                {
                    thisRB.gravityScale = physicsConfig.fallGravity;
                    fallGravityed = true;
                }
                else
                {
                    if (thisRB.gravityScale < physicsConfig.fallMaxGravity)
                    {
                        thisRB.gravityScale += thisRB.gravityScale * physicsConfig.fallGravityMultiplier;

                    }
                    else
                    {
                        thisRB.gravityScale = physicsConfig.fallMaxGravity;
                    }
                }
                thisRB.drag = physicsConfig.airDrag;
                break;
            case PlayerPhysicalState.isPeak:
                thisCol.sharedMaterial = slipperMat;
                thisRB.gravityScale = physicsConfig.peakGravity;
                thisRB.drag = physicsConfig.airDrag;
                break;
            case PlayerPhysicalState.isStanding:
                thisCol.sharedMaterial = normalMat;
                if (!gravityLock) thisRB.gravityScale = physicsConfig.normalGravity;
                thisRB.drag = physicsConfig.normalDrag;
                break;
        }   
    }

    #endregion

    #region 外部调用

    public bool IsOnFloored()
    {
        //return isFloored;
        return isRaycastGround;
    }
    public bool WasOnFloored()
    {
        //return wasFloored;
        return wasRaycastGround;
    }
    public bool IsOnWall()
    {
        return isWall;
    }
    public bool IsForwad()
    {
        return isForward;
    }
    public bool IsHead()
    {
        return isHead;
    }

    //public void LeaveGround()
    //{

    //    hasGrounded = false;
    //    hasGroundedCounter = physicsConfig.hasGroundDuration;
    //    riseGravityed = false;
    //    fallGravityed = false;

    //    hasRaycastGround = false;
    //    hasRaycastGroundCounter = physicsConfig.hasRaycastGoundDuration;
    //}

    public void LeaveFloor()
    {
        //hasFloored = false;
        //hasFlooredCounter = physicsConfig.hasFlooredDuration;
        riseGravityed = false;
        fallGravityed = false;
        hasRaycastGround = false;
        hasRaycastGroundCounter = physicsConfig.hasRaycastGoundDuration;
    }

    public void LeaveWall()
    {
        hasWall = false;
        hasWallCounter = physicsConfig.hasWallDuration;
    }
    public void GravityLock(float gravityScale)
    {
        //Debug.Log("锁了");
        thisRB.gravityScale = gravityScale;
        gravityLock = true;
    }
    public void GravityUnlock()
    {

        gravityLock = false;
    }

    public RaycastHit2D RayHit()
    {
        return theRaycastCol;
    }


    #endregion



    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(theGroundCheckpoint.position, physicsConfig.theGroundCheckRadius);
        Gizmos.DrawWireSphere(theWallCheckpoint.position, physicsConfig.theWallCheckRadius);
        Gizmos.DrawWireSphere(theForwardCheckpoint.position, physicsConfig.theForwardCheckRadius);
        Gizmos.DrawWireSphere(theBackWallCheckpoint.position, physicsConfig.theBackWallCheckRadius);
        Gizmos.DrawWireSphere(theHeadCheckpoint.position, physicsConfig.theHeadCheckRadius);
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x + physicsConfig.raysCheckStep, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x + physicsConfig.raysCheckStep, theGroundCheckpoint.position.y - physicsConfig.raycastLength));
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x - physicsConfig.raysCheckStep, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x - physicsConfig.raysCheckStep, theGroundCheckpoint.position.y - physicsConfig.raycastLength));
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x, theGroundCheckpoint.position.y - physicsConfig.raycastLength));
    }
}
