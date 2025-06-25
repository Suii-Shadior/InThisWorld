using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rope : MonoBehaviour
{
    #region 组件
    private LineRenderer lineRenderer;
    [SerializeField] private Transform headerPoint;
    [SerializeField] private Transform tailerPoint;

    #endregion

    [Header("Rope Related")]
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    public GameObject ropeSegment_TestPrefab;


    public float ropeSegLen;//绳子结点之间固定距离
    public int segmentLength;//绳子总长度
    public float lineWidth;//绳子宽度
    [Header("Constrain Related")]
    public Vector2 forceGravity;//重力方向
    [SerializeField] private int constrainApplyTime;//点之间约束的

    public int startConstrainApplyTime;
    public int updateConstraintApplyTime;

    [Header("Collision Related")]
    public LayerMask collisionMask;
    public float collisionRadius;
    public float bounceFactor;
    public float dumpingFactor;
    private float correctionClampAmount;
    public int collisionSegmentInterval;


    public struct RopeSegment
    {
        
        public Vector2 posNow;
        public Vector2 posOld;
        
        public RopeSegment(Vector2 pos)
        {
            this.posNow = pos;
            this.posOld = pos;
        }

        public void NextPos()
        {
            Vector2 velocity = posNow - posOld;
            posOld = posNow;
            posNow += velocity;
        }
        public void ApplyGrivaty(Vector2 grivatyScale)
        {
            posNow += grivatyScale * Time.deltaTime;
        }
    }


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = segmentLength;
        for (int i = 0; i < segmentLength; i++)
        {
            Vector2 originalPos = headerPoint.position;
            ropeSegments.Add(new RopeSegment(originalPos));
            originalPos+= forceGravity.normalized*ropeSegLen;
        }
    }

    private void OnEnable()
    {
        constrainApplyTime = startConstrainApplyTime;
        Simulate();
    }

    private void Start()
    {
        constrainApplyTime = updateConstraintApplyTime;
    }

    private void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        Simulate();
    }






    private void DrawRope()
    {
        // 本方法用于根据ropeSegments内容绘制线条，适用于普通帧
        Vector3[] ropePositions = new Vector3[segmentLength];
        for(int i =0; i < segmentLength; i++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }
        
        lineRenderer.SetPositions(ropePositions);
    }

    private void Simulate()
    {
        /* 本方法用于模仿绳索的物理特性，适用于物理帧调用
         * 
         * Step1.应用重力和惯性
         * Step2.迭代应用点之间的约束
         * Step3.跳个迭代碰撞处理
         */

        for (int i = 0; i < segmentLength; i++)
        {

            RopeSegment theSegment = ropeSegments[i];
            theSegment.NextPos();
            theSegment.ApplyGrivaty(forceGravity);
            ropeSegments[i] = theSegment;
  
        }
        for (int i = 0; i < constrainApplyTime; i++)
        {
            ApplyConstraint();
            if(i%collisionSegmentInterval == 0)
            {
                HandleCollions();
            }
            
        }


    }



    #region segmentConstraint
    
    private void ApplyConstraint()
    {
        /* 本方法用于引用点之间的约束，适用于Simulate内多次调整达到更好模拟效果
         * 
         * Step1.处理FixedStraint
         * Step2.处理SoftStraint
         * Step2-a.成对遍历节点,先计算前后节点之间距离，计算出调整的方向（远拉近推）,需要调整的距离
         * Step2-b.对前后节点同时进行调整
         * 
         */


        //MouseHeadConstraint();
        FixedropeConstraint();
        //BridgeConstraint();
        for (int i = 0; i < segmentLength - 1; i++)
        {
            RopeSegment formerSeg = ropeSegments[i];
            RopeSegment laterSeg = ropeSegments[i + 1];
            float dist = (formerSeg.posNow - laterSeg.posNow).magnitude;
            float distDif = Mathf.Abs(dist - ropeSegLen);

            Vector2 changeDir = Vector2.zero;
            if (dist > ropeSegLen)
            {
                changeDir = (formerSeg.posNow - laterSeg.posNow).normalized;
            }
            else if (dist < ropeSegLen)
            {
                //意味着要推远，移动方向为中间former向later
                changeDir = (laterSeg.posNow - formerSeg.posNow).normalized;
            }
            Vector2 changeAmount = changeDir * distDif;

            if (i != 0)
            {
                formerSeg.posNow -= changeAmount * .5f;
                laterSeg.posNow += changeAmount * .5f;
                ropeSegments[i] = formerSeg;
                ropeSegments[i + 1] = laterSeg;

            }
            else
            {
                laterSeg.posNow += changeAmount;

                ropeSegments[i + 1] = laterSeg;
            }
        }
    }


    private void FixedropeConstraint()
    {
        RopeSegment headSegment = ropeSegments[0];
        headSegment.posNow = headerPoint.position;
        ropeSegments[0] = headSegment;
    }

    private void BridgeConstraint()
    {
        RopeSegment headSegment = ropeSegments[0];
        headSegment.posNow = headerPoint.position;
        ropeSegments[0] = headSegment;
        RopeSegment tailerSegment = ropeSegments[segmentLength-1];
        tailerSegment.posNow = tailerPoint.position;
        ropeSegments[segmentLength - 1] = tailerSegment;
    }

    private void MouseHeadConstraint()
    {
        RopeSegment firstSegment = ropeSegments[0];
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z; ;
        firstSegment.posNow = Camera.main.ScreenToWorldPoint(mousePos);
        //Debug.Log(firstSegment.posNow);
        ropeSegments[0] = firstSegment;
    }
    private void HandleCollions()
    {
        /* 本方法用于处理节点简单的碰撞关系
         * 
         * Step1.遍历所有的节点，先得到本轮应用点之间约束使用的velocity
         * Step2.获取所有碰撞范围内的碰撞体，遍历，计算被反弹的方向，
         * Step3.根据反弹的方向，计算反弹后的位置（保证最短距离和碰撞检测距离一致）和反弹速度
         * Step4.最后反推出反弹前的位置。实现节点的所有内容都已经根据碰撞检测更新，返回节点
         */
        for(int i = 1; i < segmentLength; i++)
        {
            RopeSegment segment = ropeSegments[i];
            Vector2 velocity = segment.posNow - segment.posOld;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(segment.posNow, collisionRadius, collisionMask);
            foreach(Collider2D collider in colliders)
            {
                Vector2 closestPoint = collider.ClosestPoint(segment.posNow);
                float dist = Vector2.Distance(segment.posNow, closestPoint);
                if (dist < collisionRadius)
                {
                    Vector2 normal = (segment.posNow - closestPoint).normalized;
                    if(normal == Vector2.zero)
                    {
                        normal = (segment.posNow - (Vector2)collider.transform.position).normalized;
                    }
                    float depth = collisionRadius - dist;
                    segment.posNow += normal * depth;
                    velocity = Vector2.Reflect(velocity, normal) * bounceFactor;
                }
            }
            segment.posOld = segment.posNow - velocity;
            ropeSegments[i] = segment;
        }
    }
    #endregion




}
