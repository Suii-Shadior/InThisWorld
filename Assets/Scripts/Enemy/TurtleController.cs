using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyEnums;
public class TurtleController : EnemyBase
{
    [Header("Setting")]
    public TurtleState thisTurtleState;
    public Transform ResetPoint;
    public int hpMax;
    [Header("Info")]
    public int hpCur;
    [Header("Physical Check")]
    public Transform theGroundCheckPoint;
    public Transform theForwardCheckPoint;
    public Transform theWallCheckPoint;
    public bool isGrounded;
    public bool isForwarded;
    public bool isWalled;
    public LayerMask theGround;
    public float theGroundCheckRadius;
    [Header("Move Related")]
    public float walkSpeed;
    public int isClockwise=1;
    public Vector2 moveDir;
 
    public Transform theWallRotatePoint;
    public Transform theForwardRotatePoint;

    [Header("Idle Related")]
    public float idleCounter;
    public float idleDuration;

    [Header("Animator Related")]
    private const string WALKSTR = "isWalking";
    private const string FALLSTR = "isFalling";

    private const string IDLESTR = "isDieing";



    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();//空内容
        if (isClockwise == -1) transform.localScale = new Vector3(-.5f, .5f, .5f);
    }
    protected override void Update()
    {
        base.Update();//空内容
        switch (thisTurtleState)
        {
            case TurtleState.fallstate:
                Fall();
                break;
            case TurtleState.walkstate:
                transform.position +=(Vector3)( isClockwise * moveDir * walkSpeed * Time.deltaTime);
                PhysicsCheck();
                MoveTowardChange();

                break;
            case TurtleState.idlestate:
                Idle();
                break;
        }
    }


    private void PhysicsCheck()
    {
        isGrounded= Physics2D.OverlapCircle(theGroundCheckPoint.position, theGroundCheckRadius, theGround);
        isForwarded = Physics2D.OverlapCircle(theForwardCheckPoint.position, theGroundCheckRadius, theGround);
        isWalled = Physics2D.OverlapCircle(theWallCheckPoint.position, theGroundCheckRadius, theGround);
    }
    public void MoveTowardChange()
    {
        if (!isGrounded)
        {
            transform.rotation = Quaternion.identity;
            if (isClockwise == 1)
            {
                moveDir = Vector2.right;
            }
            else if (isClockwise == -1)
            {
                moveDir = Vector2.left;
            }

            thisTurtleState = TurtleState.fallstate;
        }
        else
        if (isWalled)
        {
            if (isClockwise == 1)
            {
                transform.RotateAround(theWallRotatePoint.position, Vector3.forward, 90f * isClockwise);
                moveDir = AntiClockwiseRotate(moveDir);
            }
            else if (isClockwise == -1)
            {
                transform.RotateAround(theWallRotatePoint.position, Vector3.forward, 90f * isClockwise);
                moveDir = ClockwiseRotate(moveDir);

            }
            else
            {
                Debug.Log("有问题");
            }
        }
        else if (!isForwarded)
        {
            if (isClockwise == 1)
            {
                transform.RotateAround(theForwardRotatePoint.position, Vector3.forward, -90f * isClockwise);
                moveDir = ClockwiseRotate(moveDir);
            }
            else if (isClockwise == -1)
            {
                transform.RotateAround(theForwardRotatePoint.position, Vector3.forward, -90f * isClockwise);
                moveDir = AntiClockwiseRotate(moveDir);
            }
            else
            {
                Debug.Log("有问题");
            }
        }
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    transform.RotateAround(theWallRotatePoint.position, Vector3.forward, 90f * isClockwise);
        //    moveDir = AntiClockwiseRotate(moveDir);
        //}
        //else if ((Input.GetKeyDown(KeyCode.Q)))
        //{
        //    transform.RotateAround(theWallRotatePoint.position, Vector3.forward, -90f * isClockwise);
        //    moveDir = ClockwiseRotate(moveDir);
        //}
    }
    public void Fall()
    {
        Debug.Log("下落");
    }

    private void Idle()
    {
        if (!isGrounded)
        {
            transform.rotation = Quaternion.identity;
            if (isClockwise == 1)
            {
                moveDir = Vector2.right;
            }
            else if (isClockwise == -1)
            {
                moveDir = Vector2.left;
            }

            thisTurtleState = TurtleState.fallstate;
        }
        if (idleCounter >=0)
        {
            idleCounter -= Time.deltaTime;
        }
        else
        {
            thisAnim.SetBool(IDLESTR, false);
            thisAnim.SetBool(WALKSTR, true);
            thisTurtleState = TurtleState.walkstate;

        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<AttackArea>(out AttackArea attackArea))
        {
            BePhysicalAttacked(attackArea);
        }
    }
    public override void BePhysicalAttacked(AttackArea attackArea)
    {
        if (thisTurtleState == TurtleState.walkstate)
        {
            idleCounter = idleDuration;
            thisAnim.SetBool(WALKSTR, false);
            thisAnim.SetBool(IDLESTR, true);
            thisTurtleState = TurtleState.idlestate;
        }
    }









    public static Vector2 ClockwiseRotate(Vector2 _moveDir)
    {
        return _moveDir.x * Vector2.down + Vector2.right * _moveDir.y;
    }

    public static Vector2 AntiClockwiseRotate(Vector2 _moveDir)
    {
        return _moveDir.x * Vector2.up + Vector2.left * _moveDir.y;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(theGroundCheckPoint.position, theGroundCheckRadius);
        Gizmos.DrawWireSphere(theWallCheckPoint.position, theGroundCheckRadius);
        Gizmos.DrawWireSphere(theForwardCheckPoint.position, theGroundCheckRadius); 
    }
    public override void ResetThisEnemy()
    {
        hpCur = hpMax;
        thisCol.enabled = true;
        thisAnim.SetBool(FALLSTR, false);
        thisAnim.SetBool(IDLESTR, false);
        thisAnim.SetBool(WALKSTR, true);
        thisTurtleState = TurtleState.walkstate;
    }
}
