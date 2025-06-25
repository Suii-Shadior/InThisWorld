using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rope : MonoBehaviour
{
    #region ���
    private LineRenderer lineRenderer;
    [SerializeField] private Transform headerPoint;
    [SerializeField] private Transform tailerPoint;

    #endregion

    [Header("Rope Related")]
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    public GameObject ropeSegment_TestPrefab;


    public float ropeSegLen;//���ӽ��֮��̶�����
    public int segmentLength;//�����ܳ���
    public float lineWidth;//���ӿ��
    [Header("Constrain Related")]
    public Vector2 forceGravity;//��������
    [SerializeField] private int constrainApplyTime;//��֮��Լ����

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
        // ���������ڸ���ropeSegments���ݻ�����������������ͨ֡
        Vector3[] ropePositions = new Vector3[segmentLength];
        for(int i =0; i < segmentLength; i++)
        {
            ropePositions[i] = ropeSegments[i].posNow;
        }
        
        lineRenderer.SetPositions(ropePositions);
    }

    private void Simulate()
    {
        /* ����������ģ���������������ԣ�����������֡����
         * 
         * Step1.Ӧ�������͹���
         * Step2.����Ӧ�õ�֮���Լ��
         * Step3.����������ײ����
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
        /* �������������õ�֮���Լ����������Simulate�ڶ�ε����ﵽ����ģ��Ч��
         * 
         * Step1.����FixedStraint
         * Step2.����SoftStraint
         * Step2-a.�ɶԱ����ڵ�,�ȼ���ǰ��ڵ�֮����룬����������ķ���Զ�����ƣ�,��Ҫ�����ľ���
         * Step2-b.��ǰ��ڵ�ͬʱ���е���
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
                //��ζ��Ҫ��Զ���ƶ�����Ϊ�м�former��later
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
        /* ���������ڴ���ڵ�򵥵���ײ��ϵ
         * 
         * Step1.�������еĽڵ㣬�ȵõ�����Ӧ�õ�֮��Լ��ʹ�õ�velocity
         * Step2.��ȡ������ײ��Χ�ڵ���ײ�壬���������㱻�����ķ���
         * Step3.���ݷ����ķ��򣬼��㷴�����λ�ã���֤��̾������ײ������һ�£��ͷ����ٶ�
         * Step4.����Ƴ�����ǰ��λ�á�ʵ�ֽڵ���������ݶ��Ѿ�������ײ�����£����ؽڵ�
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
