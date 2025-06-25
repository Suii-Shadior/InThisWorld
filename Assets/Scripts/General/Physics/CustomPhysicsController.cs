// CustomPhysicsController.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CustomPhysicsController : MonoBehaviour
{
    // 物理参数
    [Header("Physics Parameters")]
    public Vector2 gravityScale;
    public bool useMass;
    public float mass = 1f;

    // 射线检测参数
    [Header("Raycast Settings\n" +
        "TIP：RayCount必须大于2")]
    public LayerMask collisionMask;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    public int horizontalStaticRayCount;
    public int verticalStaticRayCount;
    public float skinWidth = 0.015f;

    public bool isLeftwardsStatic;
    public bool isRightwardsStatic;
    public bool isUpwardsStatic;
    public bool isDownwardsStatic;



    [Header("PhysicsDetect Related")]
    [SerializeField] private bool wasGrounded;
    [SerializeField] private bool wasTouchingCeiling;
    [SerializeField] private bool wasTouchingLeftWall;
    [SerializeField] private bool wasTouchingRightWall;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isTouchingCeiling;
    [SerializeField] private bool isTouchingLeftWall;
    [SerializeField] private bool isTouchingRightWall;

    // 运行时状态
    [Header("Runtime State (Read Only)")]
    [SerializeField] private Vector2 _velocity;
    [SerializeField] private Vector2 _acceleration;
    [SerializeField] private Vector2 _force;

    // 内部变量
    private BoxCollider2D _collider;
    private float _leftwardsRaySpacing;
    private float _rightwardsRaySpacing;
    private float _upwardsRaySpacing;
    private float _downwardsRaySpacing;

    private RaycastOrigins _raycastOrigins;
  
    private List<RaycastHit2D> _currentHits = new List<RaycastHit2D>();

    public Vector2 Velocity => _velocity;
    public bool IsGrounded => isGrounded;
    public bool IsTouchingCeiling => isTouchingCeiling;
    public bool IsTouchingLeftWall => isTouchingLeftWall;
    public bool IsTouchingRightWall => isTouchingRightWall;

    private struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();

    }

    private void FixedUpdate()
    {
        // 1. 根据当前力更新加速度,计算可能位移
        UpdatePhysicsState();
        Vector2 displacement = _velocity * Time.fixedDeltaTime;

        // 2. 处理碰撞，调整可能位移
        HandleCollisions(ref displacement);

        // 4. 应用位移
        transform.Translate(displacement);

        // 5. 重置临时变量
        _force = Vector2.zero;

    }

    private void UpdatePhysicsState()
    {
        // 应用重力
        if (useMass)
        {
            _acceleration = (_force / mass) + gravityScale;
        }
        else
        {
            _acceleration = _force + gravityScale;
        }
        

        // 更新速度
        _velocity += _acceleration * Time.fixedDeltaTime;
    }

    private void HandleCollisions(ref Vector2 displacement)
    {

        // 重置碰撞状态
        ResetCollisionStates();
        // 更新射线起点
        UpdateRaycastOrigins();
        CalculateRaySpacing(displacement);
        // 水平碰撞检测
        CheckRightForwardsCollisions(ref displacement);
        CheckLeftwardsCollisions(ref displacement);
        // 垂直碰撞检测
        CheckUpForwardsCollisions(ref displacement);
        CheckDownForwardsCollisions(ref displacement);
    }

    private void ResetCollisionStates()
    {
        wasGrounded = isGrounded;
        wasTouchingCeiling = isTouchingCeiling;
        wasTouchingLeftWall = isTouchingLeftWall;
        wasTouchingRightWall = isTouchingRightWall;
        isGrounded = false;
        isTouchingCeiling = false;
        isTouchingLeftWall = false;
        isTouchingRightWall = false;
    }


    private void UpdateRaycastOrigins()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2f);

        _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    private void CalculateRaySpacing(Vector2 displacement)
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2f);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);
        horizontalStaticRayCount = Mathf.Clamp(horizontalStaticRayCount, 2, int.MaxValue);
        verticalStaticRayCount = Mathf.Clamp(verticalStaticRayCount, 2, int.MaxValue);

        if (displacement.x > 0)
        {
            if (wasTouchingRightWall)
            {
                isRightwardsStatic = true;
                _rightwardsRaySpacing = bounds.size.y / (horizontalStaticRayCount - 1);
            }
            else
            {
                isRightwardsStatic = false;
                _rightwardsRaySpacing = bounds.size.y / (horizontalRayCount - 1);

            }
        }
        else if (displacement.x < 0)
        {
            if (wasTouchingLeftWall)
            {
                isLeftwardsStatic = true;
                _leftwardsRaySpacing= bounds.size.y / (horizontalStaticRayCount - 1);
            }
            else
            {
                isLeftwardsStatic = false;
                _leftwardsRaySpacing=bounds.size.y / (horizontalRayCount - 1);
            }
        }
        else
        {
            _leftwardsRaySpacing= bounds.size.y / (horizontalStaticRayCount - 1);
            _rightwardsRaySpacing = bounds.size.y / (horizontalStaticRayCount - 1);
            isRightwardsStatic = true;
            isLeftwardsStatic = true;
        }

        if (displacement.y > 0)
        {
            if (wasTouchingCeiling)
            {
                isUpwardsStatic = true;
                _upwardsRaySpacing = bounds.size.x / (verticalStaticRayCount - 1);
            }
            else
            {
                isUpwardsStatic = false;
                _upwardsRaySpacing = bounds.size.x / (verticalRayCount - 1);
            }
        }
        else if (displacement.y < 0)
        {
            if (wasGrounded)
            {
                isDownwardsStatic = true;
                _downwardsRaySpacing= bounds.size.x / (verticalStaticRayCount - 1);
            }
            else
            {
                isDownwardsStatic = false;
                _downwardsRaySpacing= bounds.size.x / (verticalRayCount - 1);
            }
        }
        else
        {
            _upwardsRaySpacing = bounds.size.x / (verticalStaticRayCount - 1);
            _downwardsRaySpacing= bounds.size.x / (verticalStaticRayCount - 1);
            isUpwardsStatic = true;
            isDownwardsStatic = true;
        }


    }





    //分开检测有利于对于非移动方向的检测可以采用更加节约资源的方式
    private void CheckRightForwardsCollisions(ref Vector2 displacement)
    {
        float rayLength;
        int rayCount;
        if (Mathf.Abs(displacement.x) > skinWidth && !wasTouchingRightWall)
        {
            rayLength = Mathf.Abs(displacement.x) + skinWidth;
            rayCount = horizontalRayCount;
        }
        else
        {
            rayCount = horizontalStaticRayCount;
            rayLength = 2 * skinWidth;
        }
        float minHitDist = 0;
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = _raycastOrigins.bottomRight+ Vector2.up * (_rightwardsRaySpacing * i);
            Vector2 rayDirection = Vector2.right;


            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                rayDirection,
                rayLength,
                collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * rayDirection * rayLength, Color.red);

            if (hit)
            {
                if (displacement.x > 0)
                {
                    minHitDist = hit.distance - skinWidth;
                    rayLength = hit.distance;
                    
                }
                else
                {
                    //在被推
                }
                


                // 碰撞后水平速度归零
                
            }
        }
        if (minHitDist != 0)
        {
            displacement.x = minHitDist;
            _velocity.x = 0;
            isTouchingRightWall = true;
        }
    }

    private void CheckLeftwardsCollisions(ref Vector2 displacement)
    {
        float rayLength;
        int rayCount;
        if (Mathf.Abs(displacement.x) > skinWidth && !wasTouchingLeftWall)
        {
            rayLength = Mathf.Abs(displacement.x) + skinWidth;
            rayCount = horizontalRayCount;
        }
        else
        {
            rayCount = horizontalStaticRayCount;
            rayLength = 2 * skinWidth;
        }
        float minHitDist = 0;
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = _raycastOrigins.bottomLeft+ Vector2.up * (_leftwardsRaySpacing * i);
            Vector2 rayDirection = Vector2.left;

            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                rayDirection,
                rayLength,
                collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * rayDirection * rayLength, Color.red);

            if (hit)
            {
                if (displacement.x < 0)
                {
                    minHitDist = -(hit.distance - skinWidth);
                    rayLength = hit.distance;
                    
                }
                else
                {
                    //在被推
                }


            }
        }
        if (minHitDist!=0)
        {
            displacement.x = minHitDist;
            _velocity.x = 0;
            isTouchingLeftWall = true;
        }

    }

    private void CheckUpForwardsCollisions(ref Vector2 displacement)
    {

        float rayLength;
        int rayCount;
        if (Mathf.Abs(displacement.y) > skinWidth && !wasTouchingCeiling)
        {
            rayLength = Mathf.Abs(displacement.y) + skinWidth;
            rayCount = verticalRayCount;
        }
        else
        {
            rayCount = verticalStaticRayCount;
            rayLength = 2 * skinWidth;
        }
        float minHitDist = 0;
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = _raycastOrigins.topLeft+Vector2.right*_upwardsRaySpacing * i;
            Vector2 rayDirection = Vector2.up ;
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                rayDirection,
                rayLength,
                collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * rayDirection * rayLength, Color.red);

            if (hit)
            {
                if (displacement.y > 0)
                {
                    minHitDist = hit.distance - skinWidth;
                    rayLength = hit.distance;
                    
                }
                else
                {
                    //在被抬起
                }
                

            }
        }
        if (minHitDist != 0)
        {
            displacement.y = minHitDist;
            isTouchingCeiling = true;
            _velocity.y = 0;
        }
    }

    private void CheckDownForwardsCollisions(ref Vector2 displacement)
    {
        float rayLength;
        int rayCount;
        if (Mathf.Abs(displacement.y) > skinWidth && !wasGrounded)
        {
            rayLength = Mathf.Abs(displacement.y) + skinWidth;
            rayCount = verticalRayCount;
        }
        else
        {
            rayCount = verticalStaticRayCount;
            rayLength = 2 * skinWidth;
        }
        float minHitDist=0;
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = _raycastOrigins.bottomLeft +Vector2.right* _downwardsRaySpacing * i;
            Vector2 rayDirection = Vector2.down;
            RaycastHit2D hit = Physics2D.Raycast(
                rayOrigin,
                rayDirection,
                rayLength,
                collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * rayDirection * rayLength, Color.red);

            if (hit)
            {
                if (displacement.y < 0)
                {
                    minHitDist = -(hit.distance - skinWidth);
                    
                    rayLength = hit.distance;
                    
                }
                else
                {
                    //在被下压
                }
            }
        }
        if (minHitDist != 0)
        {
            displacement.y = minHitDist;
            isGrounded = true;
            _velocity.y = 0;
        }
    }


    #region 外部调用方法

    public void AddForce(Vector2 force)
    {
        _force += force;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _velocity = velocity;
    }



    #endregion
}