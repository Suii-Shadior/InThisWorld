using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyEnums;
using AttackableInterfaces;

public class SpiderController : EnemyBase
{


    [Header("Setting")]
    public SpiderState thisSpiderState;
    public Transform ResetPoint;
    public int hpMax;
    [Header("Info")]
    public int hpCur;


    [Header("Hang")]
    public Transform theNowPoint;
    public Transform theDestinationPoint;
    public Transform theStartPoint;
    public Transform theEndPoint;
    public float hungSpeed;

    [Header("Animator Related")]
    private const string HANGSTR = "isHanging";
    private const string DIESTR = "isDieing";


    protected override void Awake()
    {
        base.Awake();//空内容
    }
    protected override void Start()
    {
        base.Start();//空内容
        theNowPoint.parent = null;
        theDestinationPoint.parent = null;
        theEndPoint.parent = null;
        theStartPoint.parent = null;

        ResetThisEnemy();

    }

    // Update is called once per frame
    protected override void Update()
    {

        switch (thisSpiderState)
        {
            case SpiderState.hangstate:
                Hang();
                break;
            case SpiderState.deadstate:
                Die();
                break;
        }
    }



    private void Hang()
    {
        Vector3 offset = Vector3.MoveTowards(theNowPoint.position, theDestinationPoint.position, hungSpeed * Time.deltaTime) - theNowPoint.position;
        theNowPoint.position += offset;
        transform.position += offset;
        if (Vector3.Distance(theNowPoint.position, theDestinationPoint.position) < 0.1f)
        {
            if(Vector3.Distance(theStartPoint.position, theDestinationPoint.position) < 0.1f)
            {
                theDestinationPoint.position = theEndPoint.position;
            }
            else if(Vector3.Distance(theEndPoint.position, theDestinationPoint.position) < 0.1f)
            {
                theDestinationPoint.position = theStartPoint.position;

            }
        }
    }

    private void Die()
    {
        if (PlayingPercent() >= .99f)
        {
            thisAnim.speed = 0f;
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
        if (--hpCur == 0)
        {
            thisSpiderState = SpiderState.deadstate;
            thisCol.enabled = false;
            thisAnim.SetBool(HANGSTR, false);
            thisAnim.SetBool(DIESTR, true);
        }
    }


    #region 小方法和外部调用
    public  override void ResetThisEnemy()
    {
        hpCur = hpMax;
        thisCol.enabled = true;
        thisAnim.SetBool(DIESTR, false);
        thisAnim.SetBool(HANGSTR, true);
        thisAnim.speed = 1f;
        thisSpiderState = SpiderState.hangstate;
    }
    #endregion
}
