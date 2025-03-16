using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPhysicsRelated : MonoBehaviour
{
    private Rigidbody2D thisRB;
    public CapsuleCollider2D bodyCol;
    public BoxCollider2D footCol;

    [Header("PhysicsCheck")]
    public bool isGround;
    private bool wasGround;
    public Transform theGroundCheckpoint;
    public bool isHead;
    private bool wasHead;
    public Transform theHeadCheckpoint;
    public bool isWall;
    public Transform theWallCheckpoint;
    public LayerMask theGround;
    public float groundDetectionRayLength = 0.02f;
    public float headDetectionRayLength = 0.02f;
    [Range(0f, 1f)] public float HeadWidth = 0.75f;
    //public float theGroundCheckRadius;
    //public float theHeadCheckRadius;

    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _isGrounded;
    private bool _bumpedHead;
    //[Header("PhysicsChange")]

    //[Header("PhysicsChange Detail")]

    //[Header("Other")]
  





    private void Awake()
    {
        thisRB = GetComponent<Rigidbody2D>();
        bodyCol = GetComponentInChildren<CapsuleCollider2D>();
        footCol = GetComponentInChildren<BoxCollider2D>();
    }

    public void PRUpdate()//为了解决不同脚本的UPdate先后问题，将本方法用于PC调用进行Update
    {
        GroundCheck();
    }

    
    private void GroundCheck()
    {
        Vector2 boxCastOrigin = new Vector2(footCol.bounds.center.x, footCol.bounds.min.y);
        Vector2 boxCastSize = new Vector2(footCol.bounds.size.x, groundDetectionRayLength);
        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0, Vector2.down, groundDetectionRayLength, theGround);
        if (_groundHit.collider != null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }

    }

    #region 外部调用
    public bool IsOnGround()
    {
        return isGround;
    }

    public bool IsOnWall()
    {
        return isWall;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(new Vector2(footCol.bounds.center.x - footCol.bounds.size.x / 2, footCol.bounds.min.y), Vector2.down * groundDetectionRayLength);
        Gizmos.DrawRay(new Vector2(footCol.bounds.center.x + footCol.bounds.size.x / 2, footCol.bounds.min.y), Vector2.down * groundDetectionRayLength);
        Gizmos.DrawRay(new Vector2(footCol.bounds.center.x - footCol.bounds.size.x / 2, footCol.bounds.min.y- groundDetectionRayLength), Vector2.right * footCol.bounds.size.x);
    }
}
