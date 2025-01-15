using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRelated : MonoBehaviour
{
    private Rigidbody2D thisRB;
    private Collider2D thisCol;

    [Header("PhysicsCheck")]
    private bool isGround;
    public bool wasGround;
    public Transform theGroundCheckpoint;
    public bool isWall;
    public bool wasWall;
    public Transform theWallCheckpoint;
    public bool isForward;
    public Transform theForwardCheckpoint;
    public bool isBackWall;
    public Transform theBackWallCheckpoint;
    public LayerMask theGround;
    public float theGroundCheckRadius;
    public float theWallCheckRadius;
    public float theForwardCheckRadius;
    public float theBackWallCheckRadius;

    [Header("PhysicsChange-")]
    public PhysicsMaterial2D normalMat;
    public PhysicsMaterial2D slipperMat;
    public bool isPeak;//new
    public bool isHolding;
    public bool gravityLock;
    public bool needGroundChange;
    public bool needWallChange;
    public bool needAirChange;
    [Header("PhysicsChange Detail")]
    public float normalGravity = 2;
    public float peakGravity = 3f;
    public float riseGravity = 4;
    public float fallGravity = 4.5f;
    public float normalDrag = 0;
    public float airDrag = 1;

    [Header("Other")]
    //public bool hasDisGround;
    //public float hasDisGroundCounter;
    //public float hasDisGroundLength;
    public bool hasGround;
    public float hasGroundCounter;
    public float hasGroundLength = .1f;
    //public bool hasDisWall;
    //public float hasDisWallCounter;
    //public float hasDisWallLength;
    public bool hasWall;
    public float hasWallCounter;
    public float hasWallLength = .1f;
    private bool hasForward;
    public float hasForwardCounter;
    public float hasForwardLength = .1f;



    private void Awake()
    {
        thisRB = GetComponent<Rigidbody2D>();
        thisCol = GetComponent<Collider2D>();
    }

    public void PRUpdate()//为了解决不同脚本的UPdate先后问题，将本方法用于PC调用进行Update
    {
        GroundCheck();
        WallCheck();
        ForwardCheck();
        BackWallCheck();
        MaterialAndGravityUpdate();
    }

    private void GroundCheck()//用于实现地面判定和触发，但是由于上墙离地都是在当前状态中Update持续更新，且不是唯一的条件，所以ifNeed判定在次作用不大，
    {
        WhetherHadGround();//设定离地时间，用来来调节离地瞬间的单纯图层检测的误判
        isGround = hasGround && Physics2D.OverlapCircle(theGroundCheckpoint.position, theGroundCheckRadius, theGround);//通过图层检测及前述离地时间判断本帧的触地情况


    }
    private void WallCheck()//用于实现墙面判定和触发，但是由于上墙离地都是在当前状态中Update持续更新，且不是唯一的条件，所以ifNeed判定在次作用不大，
    {
        WhetherHadWall();
        isWall = hasWall && Physics2D.OverlapCircle(theWallCheckpoint.position, theWallCheckRadius, theGround);

    }

    private void ForwardCheck()
    {
        //WhetherHadWall();
        isForward = Physics2D.OverlapCircle(theForwardCheckpoint.position, theForwardCheckRadius, theGround);
    }

    private void BackWallCheck()
    {
        isBackWall = Physics2D.OverlapCircle(theBackWallCheckpoint.position, theBackWallCheckRadius, theGround);
    }

    private void MaterialAndGravityUpdate()
    {
        if (!isGround)
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
        else
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
                if (isPeak) thisRB.gravityScale = peakGravity;
                else
                {
                    if (thisRB.velocity.y > 0) thisRB.gravityScale = riseGravity;
                    else thisRB.gravityScale = fallGravity;
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

    private void WhetherHadGround()
    {
        if (!hasGround)
        {
            if (hasGroundCounter > 0f) hasGroundCounter -= Time.deltaTime;
            else
            {
                hasGround = true;
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

    #endregion



    #region 外部调用

    public bool IsOnGround()
    {
        return isGround;
    }


    public void LeaveGround()
    {
        hasGround = false;
        hasGroundCounter = hasGroundLength;
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

    #endregion



    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(theGroundCheckpoint.position, theGroundCheckRadius);
        Gizmos.DrawWireSphere(theWallCheckpoint.position, theWallCheckRadius);
        Gizmos.DrawWireSphere(theForwardCheckpoint.position, theForwardCheckRadius);
        Gizmos.DrawWireSphere(theBackWallCheckpoint.position, theBackWallCheckRadius);
    }
}
