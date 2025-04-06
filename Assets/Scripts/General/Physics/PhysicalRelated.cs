using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsRelated : MonoBehaviour
{
    private Rigidbody2D thisRB;
    private Collider2D thisCol;
    private NewPlayerController thePlayer;

    [Header("PhysicsCheck")]
    private bool isFloored;
    public bool wasFloored;
    private bool isGrounded;
    public bool wasGrounded;

    public Transform theGroundCheckpoint;
    private bool isWall;
    public bool wasWall;
    public Transform theWallCheckpoint;
    private bool isForward;
    public Transform theForwardCheckpoint;
    public bool isBackWall;
    public Transform theBackWallCheckpoint;
    [SerializeField]private bool isHead;
    public Transform theHeadCheckpoint;


    [Header("3RaycastGroundCheck")]
    public bool isRaycastGround;
    public bool wasRaycastGround;
    private RaycastHit2D theRaycastCol;
    public float footCheckPointOffset;
    public float raycastLength;

    public LayerMask theCeil;
    public LayerMask theFloor;
    public LayerMask theWall;
    public LayerMask theTrueGround;
    public float theGroundCheckRadius;
    public float theWallCheckRadius;
    public float theForwardCheckRadius;
    public float theBackWallCheckRadius;
    public float theHeadCheckRadius;

    [Header("PhysicsChange-")]
    public PhysicsMaterial2D normalMat;
    public PhysicsMaterial2D slipperMat;
    public bool isPeak;//new
    public bool isHolding;
    public bool isRising;
    public bool isFalling;
    public bool gravityLock;
    public bool needGroundChange;
    public bool needWallChange;
    public bool needAirChange;
    [Header("PhysicsChange Detail")]
    private bool riseGravityed;
    private bool fallGravityed;
    public float normalGravity = 2;
    public float peakGravity = 3f;
    public float riseGravity = 4;
    public float risegravityMultiplier = 1.02f;
    public float fallGravity = 4.5f;
    public float fallGravityMultiplier = 1.2f;
    public float normalDrag = 0;
    public float airDrag = 1;

    [Header("Other")]
    //public bool hasDisGround;
    //public float hasDisGroundCounter;
    //public float hasDisGroundLength;
    public bool hasGrounded;
    public float hasGroundedCounter;
    public float hasGroundLength = .1f;
    public bool hasFloored;
    public float hasFlooredCounter;
    public float hasFlooredLength = .1f;

    //public bool hasDisWall;
    //public float hasDisWallCounter;
    //public float hasDisWallLength;
    public bool hasWall;
    public float hasWallCounter;
    public float hasWallLength = .1f;
    private bool hasForward;
    public float hasForwardCounter;
    public float hasForwardLength = .1f;

    public bool hasRaycastGround;
    public float hasRaycastGroundCounter;
    public float hasRaycastGoundLength = .1f;


    private void Awake()
    {
        thisRB = GetComponent<Rigidbody2D>();
        thisCol = GetComponent<Collider2D>();
        thePlayer = GetComponent<NewPlayerController>();
    }

    public void PRUpdate()//为了解决不同脚本的UPdate先后问题，将本方法用于PC调用进行Update
    {
        FlooredCheck();
        GroundedCheck();
        RaycastGroundCheck();
        WallCheck();
        ForwardCheck();
        BackWallCheck();
        HeadCheck();
    }
    public void PRFixUpdate()
    {
        MaterialAndGravityUpdate();
    }

    private void FlooredCheck()//用于实现地面判定和触发，但是由于上墙离地都是在当前状态中Update持续更新，且不是唯一的条件，所以ifNeed判定在次作用不大，
    {
        WhetherHadFloored();
        wasFloored = isFloored;
        isFloored = hasFloored && Physics2D.OverlapCircle(theGroundCheckpoint.position, theGroundCheckRadius, theFloor);

    }

    private void GroundedCheck()
    {
        WhetherHadGrounded();//设定离地时间，用来来调节离地瞬间的单纯图层检测的误判
        wasGrounded = isGrounded;
        isGrounded = hasGrounded && Physics2D.OverlapCircle(theGroundCheckpoint.position, theGroundCheckRadius, theTrueGround);//通过图层检测及前述离地时间判断本帧的触地情况

    }


    private void RaycastGroundCheck()
    {
        WhetherHadRaycastGround();
        RaycastHit2D theBodyRaycastCheckCol = Physics2D.Raycast(new Vector2(theGroundCheckpoint.position.x + footCheckPointOffset, theGroundCheckpoint.position.y), Vector2.down, raycastLength, theFloor);
        RaycastHit2D theLeftRaycastCheckCol = Physics2D.Raycast(new Vector2(theGroundCheckpoint.position.x - footCheckPointOffset, theGroundCheckpoint.position.y), Vector2.down, raycastLength, theFloor);
        RaycastHit2D theRightRaycastCheckCol = Physics2D.Raycast(new Vector2(theGroundCheckpoint.position.x + footCheckPointOffset, theGroundCheckpoint.position.y), Vector2.down, raycastLength, theFloor);

        if (theBodyRaycastCheckCol){
            isRaycastGround = true;
            theRaycastCol = theBodyRaycastCheckCol;
        }
        else if(theLeftRaycastCheckCol)
        {
            isRaycastGround = true;
            theRaycastCol = theLeftRaycastCheckCol;
        }
        else if (theRightRaycastCheckCol)
        {
            isRaycastGround = true;
            theRaycastCol = theRightRaycastCheckCol;
        }
        else
        {
            isRaycastGround = false;
            theRaycastCol = theBodyRaycastCheckCol;
        }
        if(hasRaycastGround&& isRaycastGround)
        {
            isRaycastGround = true;
        }
        else
        {
            isRaycastGround = false;
            theRaycastCol = theBodyRaycastCheckCol;
        }
    }

    private void WallCheck()//用于实现墙面判定和触发，但是由于上墙离地都是在当前状态中Update持续更新，且不是唯一的条件，所以ifNeed判定在次作用不大，
    {
        WhetherHadWall();
        wasWall = isWall;
        isWall = hasWall && Physics2D.OverlapCircle(theWallCheckpoint.position, theWallCheckRadius, theWall);

    }

    private void ForwardCheck()
    {
        //WhetherHadWall();
        isForward = Physics2D.OverlapCircle(theForwardCheckpoint.position, theForwardCheckRadius, theFloor);
    }

    private void BackWallCheck()
    {
        
        isBackWall = Physics2D.OverlapCircle(theBackWallCheckpoint.position, theBackWallCheckRadius, theFloor);
    }


    private void HeadCheck()
    {
        
        isHead = Physics2D.OverlapCircle(theHeadCheckpoint.position, theHeadCheckRadius, theCeil);
    }
    private void MaterialAndGravityUpdate()
    {
        if (!isFloored)
        {
           
            if (isHolding)
            {
                HoldingMaterialAndGravityPara();
            }
            else
            {
                AiringMaterialAndGravityPara();
            }
        }
        else if(thePlayer.stateMachine.currentState== thePlayer.idleState|| thePlayer.stateMachine.currentState == thePlayer.runState)
        {
            GroundingMaterialAndGravityPara();
        }

    }

    private void HoldingMaterialAndGravityPara()
    {
        thisCol.sharedMaterial = slipperMat;
        if (!gravityLock) thisRB.gravityScale = 0f;
        thisRB.drag = airDrag;
    }

    private void AiringMaterialAndGravityPara()
    {
        thisCol.sharedMaterial = slipperMat;
        {
            if (!gravityLock)
            {
                if (isPeak)
                {
                    thisRB.gravityScale = peakGravity;
                }
                else if (isRising)
                {
                    if(!riseGravityed)
                    {
                        thisRB.gravityScale = riseGravity;
                        riseGravityed = true;
                    }
                    else
                    {
                        thisRB.gravityScale += thisRB.gravityScale * risegravityMultiplier;
                    }
                }
                else if (isFalling)
                {
                    if(!fallGravityed)
                    {
                        thisRB.gravityScale = fallGravity;
                        fallGravityed = true;
                    }
                    else
                    {
                        thisRB.gravityScale += thisRB.gravityScale * fallGravityMultiplier;
                        //Debug.Log(thisRB.gravityScale);
                    }
                }
                    
                
            }
        }
        thisRB.drag = airDrag;
    }

    private void GroundingMaterialAndGravityPara()
    {
        thisCol.sharedMaterial = normalMat;
        if (!gravityLock) thisRB.gravityScale = normalGravity;
        thisRB.drag = normalDrag;
    }


    #region 物理条件判断方法
    private void WhetherHadFloored()
    {
        if (!hasFloored)
        {
            if (hasFlooredCounter > 0f) hasFlooredCounter -= Time.deltaTime;
            else
            {
                hasFloored = true;
            }
        }
    }






    private void WhetherHadGrounded()
    {
        if (!hasGrounded)
        {
            if (hasGroundedCounter > 0f) hasGroundedCounter -= Time.deltaTime;
            else
            {
                hasGrounded = true;
            }
        }
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

    #endregion



    #region 外部调用

    public bool IsOnFloored()
    {
        return isFloored;
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

    public void LeaveGround()
    {
        hasGrounded = false;
        hasGroundedCounter = hasGroundLength;
        riseGravityed = false;
        fallGravityed = false;

        hasRaycastGround = false;
        hasRaycastGroundCounter = hasRaycastGoundLength;
    }

    public void LeaveWall()
    {
        hasWall = false;
        hasWallCounter = hasWallLength;
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
        Gizmos.DrawWireSphere(theGroundCheckpoint.position, theGroundCheckRadius);
        Gizmos.DrawWireSphere(theWallCheckpoint.position, theWallCheckRadius);
        Gizmos.DrawWireSphere(theForwardCheckpoint.position, theForwardCheckRadius);
        Gizmos.DrawWireSphere(theBackWallCheckpoint.position, theBackWallCheckRadius);
        Gizmos.DrawWireSphere(theHeadCheckpoint.position, theHeadCheckRadius);
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x + footCheckPointOffset, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x + footCheckPointOffset, theGroundCheckpoint.position.y - raycastLength));
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x - footCheckPointOffset, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x - footCheckPointOffset, theGroundCheckpoint.position.y - raycastLength));
        Gizmos.DrawLine(new Vector2(theGroundCheckpoint.position.x, theGroundCheckpoint.position.y), new Vector2(theGroundCheckpoint.position.x, theGroundCheckpoint.position.y - raycastLength));
    }
}
