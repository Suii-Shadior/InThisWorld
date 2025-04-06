using AttackInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using OtherEnum;


public class Obstacle : MonoBehaviour, IPhysicalAttack
{

    private SpriteRenderer thisSR;
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;
    private EdgeCollider2D thisEdgeCol;
    private float beAttackedCounter;
    public float beAttackedDuration;
    public float multiple;
    [HideInInspector]public enum ObstacleTpye { unrepeatable_blocker,unrepeatable_nail}



    public bool hasAttacked;
    public Vector2 attackVec2;
    public ObstacleTpye thisObstacleType;



    [Header("Blocker Related")]
    public AnimatorController theBlockerAnimator;
    public int needToDestroy;
    public BeAttackedDir canAttackDir;

    [Header("Nailer Related")]
    public AnimatorController theNailerAnimator;
    public OnewayPlatform theOnewayPlatform;

    [Header("Animator Related")]
    private const string HASATTACKEDSTR = "hasAttacked";
    private const string UNATTACKEDSTR = "isUnattacked";
    private const string ATTACKINGSTR = "isAttacking";
    private const string DESTROYINGSTR = "isDestroying";



    private void Awake()
    {
        thisAnim = GetComponentInChildren<Animator>();
        thisSR = GetComponentInChildren<SpriteRenderer>();
        thisEdgeCol = GetComponent<EdgeCollider2D>();
        thisBoxCol = GetComponent<BoxCollider2D>();
        SceneLoadRelates();
    }

    private void SceneLoadRelates()
    {

        switch (thisObstacleType)
        {
            case ObstacleTpye.unrepeatable_nail:
                Unrepeatable_NailAwake();
                break;
        }
    }

    private void Unrepeatable_NailAwake()
    {
        theOnewayPlatform.hasTriggered = hasAttacked;
    }



    void Start()
    {
        SceneLoadItself();
    }

    private void SceneLoadItself()
    {

        switch (thisObstacleType)
        {
            case ObstacleTpye.unrepeatable_blocker:
                Unrepeatable_BlockStart();
                break;
            case ObstacleTpye.unrepeatable_nail:
                Unrepeatable_NailStart();
                break;
        }
    }
    private void Unrepeatable_BlockStart()
    {
        //theBlockerAnimator
        thisAnim.runtimeAnimatorController = theBlockerAnimator;
        if (!hasAttacked)
        {
            needToDestroy = 3;
            thisBoxCol.enabled = true;
            thisBoxCol.size = thisSR.size* multiple;
            thisEdgeCol.enabled = true;
            thisEdgeCol.isTrigger = true;
            thisAnim.SetBool(UNATTACKEDSTR, true);
            List<Vector2> theLocation = new List<Vector2>();
            switch (canAttackDir)
            {
                case BeAttackedDir.leftwardDir:
                    thisEdgeCol.offset = new Vector2(-thisBoxCol.size.x / 2, 0);
                    theLocation.Add(new Vector2(0f, thisBoxCol.size.y / 2));
                    theLocation.Add(new Vector2(0f, -thisBoxCol.size.y / 2));
                    break;
                case BeAttackedDir.rightwardDir:
                    thisEdgeCol.offset = new Vector2(thisBoxCol.size.x / 2, 0);
                    theLocation.Add(new Vector2(0f, thisBoxCol.size.y / 2));
                    theLocation.Add(new Vector2(0f, -thisBoxCol.size.y / 2));

                    break;
                case BeAttackedDir.upwardDir:
                    thisEdgeCol.offset = new Vector2(0, thisBoxCol.size.y / 2);
                    theLocation.Add(new Vector2(-thisBoxCol.size.x / 2, 0f));
                    theLocation.Add(new Vector2(thisBoxCol.size.x / 2, 0f));


                    break;
                case BeAttackedDir.downwardDir:
                    thisEdgeCol.offset = new Vector2(0, -thisBoxCol.size.y / 2);
                    theLocation.Add(new Vector2(-thisBoxCol.size.x / 2, 0f));
                    theLocation.Add(new Vector2(thisBoxCol.size.x / 2, 0f));

                    break;
            }
            thisEdgeCol.points = theLocation.ToArray();

        }
        else
        {
            thisBoxCol.enabled = false;
            thisEdgeCol.enabled = false;
            thisAnim.SetBool(HASATTACKEDSTR, true);
            needToDestroy = 0;
        }
    }

    private void Unrepeatable_NailStart()
    {
        thisAnim.runtimeAnimatorController = theNailerAnimator;
        if (hasAttacked)
        {
            thisBoxCol.enabled = false;
            thisEdgeCol.enabled = false;
            thisAnim.SetBool(HASATTACKEDSTR, true);
        }
        else
        {

            thisEdgeCol.enabled = false;
            thisAnim.SetBool(UNATTACKEDSTR, true);

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (beAttackedCounter > 0)
        {
            beAttackedCounter -= Time.deltaTime;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AttackArea>())
        {
            attackVec2 = other.GetComponent<AttackArea>().AttackVec;
            BePhysicalAttacked();
        }
    }


    public void BePhysicalAttacked()
    {
        switch (thisObstacleType)
        {
            case ObstacleTpye.unrepeatable_blocker:
                Unrepeatable_BlockAttacked();
                break;
            case ObstacleTpye.unrepeatable_nail:
                Unrepeatable_NailAttacked();
                break;
        }
    }


    private void Unrepeatable_BlockAttacked()
    {
        if (beAttackedCounter <= 0)
        {
            switch (canAttackDir)
            {
                case BeAttackedDir.rightwardDir:
                    if(attackVec2==new Vector2(-1, 0))
                    {
                        HitThisObstacle();
                    }
                    break;
                case BeAttackedDir.leftwardDir:
                    if (attackVec2 == new Vector2(1, 0))
                    {
                        HitThisObstacle();
                    }
                    break;
                case BeAttackedDir.upwardDir:
                    if (attackVec2 == new Vector2(0, -1))
                    {
                        HitThisObstacle();
                    }
                    break;
                case BeAttackedDir.downwardDir:
                    if (attackVec2 == new Vector2(0, 1))
                    {
                        HitThisObstacle();
                    }
                    break;
            }

        }
    }
    private void Unrepeatable_NailAttacked()
    {
        Debug.Log("攻击计数");
        thisAnim.SetTrigger(DESTROYINGSTR);
        thisBoxCol.enabled = false;
        thisEdgeCol.enabled = false;
        theOnewayPlatform.TriggerThisPlatform();
    }

    private void HitThisObstacle()
    {
        Debug.Log("攻击计数");
        needToDestroy--;
        if (needToDestroy == 0)
        {
            thisBoxCol.enabled = false;
            thisEdgeCol.enabled = false;
            thisAnim.SetTrigger(DESTROYINGSTR);
        }
        else
        {
            thisAnim.SetTrigger(ATTACKINGSTR);
        }
        beAttackedCounter = beAttackedDuration;
    }
}
