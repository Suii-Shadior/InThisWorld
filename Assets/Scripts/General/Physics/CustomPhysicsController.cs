// CustomPhysicsController.cs
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CustomPhysicsController : MonoBehaviour
{
    // �������
    [Header("Physics Parameters")]
    public Vector2 gravityScale;
    public bool useMass;
    public float mass = 1f;

    // ���߼�����
    [Header("Raycast Settings\n" +
        "TIP��RayCount�������2")]
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

    // ����ʱ״̬
    [Header("Runtime State (Read Only)")]
    [SerializeField] private Vector2 _velocity;
    [SerializeField] private Vector2 _acceleration;
    [SerializeField] private Vector2 _force;

    // �ڲ�����
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
        // 1. ���ݵ�ǰ�����¼��ٶ�,�������λ��
        UpdatePhysicsState();
        Vector2 displacement = _velocity * Time.fixedDeltaTime;

        // 2. ������ײ����������λ��
        HandleCollisions(ref displacement);

        // 4. Ӧ��λ��
        transform.Translate(displacement);

        // 5. ������ʱ����
        _force = Vector2.zero;

    }

    private void UpdatePhysicsState()
    {
        // Ӧ������
        if (useMass)
        {
            _acceleration = (_force / mass) + gravityScale;
        }
        else
        {
            _acceleration = _force + gravityScale;
        }
        

        // �����ٶ�
        _velocity += _acceleration * Time.fixedDeltaTime;
    }

    private void HandleCollisions(ref Vector2 displacement)
    {

        // ������ײ״̬
        ResetCollisionStates();
        // �����������
        UpdateRaycastOrigins();
        CalculateRaySpacing(displacement);
        // ˮƽ��ײ���
        CheckRightForwardsCollisions(ref displacement);
        CheckLeftwardsCollisions(ref displacement);
        // ��ֱ��ײ���
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





    //�ֿ���������ڶ��ڷ��ƶ�����ļ����Բ��ø��ӽ�Լ��Դ�ķ�ʽ
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
                    //�ڱ���
                }
                


                // ��ײ��ˮƽ�ٶȹ���
                
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
                    //�ڱ���
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
                    //�ڱ�̧��
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
                    //�ڱ���ѹ
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


    #region �ⲿ���÷���

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