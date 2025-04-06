using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyEnums;

public class ShoterController : EnemyBase
{

    #region 组件
    #endregion

    [Header("Setting")]
    public ShooterState thisShooterState;
    //public float attackArrange;
    public bool faceLeft;
    public int hpMax;

    [Header("Info")]
    public int hpCur;

    [Header("Shot Related")]
    public float shotCooldownCounter;
    public float shotCooldownDuration;
    public GameObject[] theBulletPool;
    public int nextBullet;
    public Transform theShotPoint;
    public bool hasShoted;

    [Header("Animator Related")]
    private const string IDLESTR = "isIdling";
    private const string SHOTSTR = "isShotting";
    private const string DIESTR = "isDieing";



    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();//空内容
        transform.localScale = new Vector3(transform.localScale.x * (faceLeft ? -1 : 1), transform.localScale.y, transform.localScale.z);
        ResetThisEnemy();
    }


    protected override void Update()
    {
        base.Update();//空内容
        switch (thisShooterState)
        {
            case ShooterState.idlestate:
                PrepareShoot();
                break;
            case ShooterState.shootingstate:
                Shot();
                break;
            case ShooterState.deadstate:
                Die();
                break;
        }
    }



    private void PrepareShoot()
    {
        if (shotCooldownCounter >= 0)
        {
            shotCooldownCounter -= Time.deltaTime;
        }
        else if(thePlayer!=null)
        {
            thisAnim.SetBool(IDLESTR, false);
            thisAnim.SetBool(SHOTSTR, true);
            thisShooterState = ShooterState.shootingstate;
        }
    }


    private void Shot()
    {
        if (isPlaying("shoot"))//名字判断这里可能有问题
        {

            if(PlayingPercent() >= .99f)
            {
                thisAnim.SetBool(SHOTSTR, false);
                thisAnim.SetBool(IDLESTR, true);
                thisShooterState = ShooterState.idlestate;
            }
            else 
            if (PlayingPercent() >= .9f && !hasShoted)//这里要对上动画效果不是那么容易 是否弄成事件帧？
            {

                if (!theBulletPool[nextBullet].activeInHierarchy)
                {
                    ShoterBulletController _theBullet = theBulletPool[nextBullet++].GetComponent<ShoterBulletController>();
                    _theBullet.movingDir = faceLeft ? -1 : 1;
                    _theBullet.transform.position = theShotPoint.position;
                    _theBullet.ResetThis();
                    hasShoted = true;

                }
                if(nextBullet== theBulletPool.Length)
                {
                    nextBullet = 0;
                }

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
        if (other.GetComponent<AttackArea>())
        {
            BePhysicalAttacked();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {

    }

    public override void BePhysicalAttacked()
    {
        if (--hpCur == 0)
        {
            thisShooterState = ShooterState.deadstate;
            thisCol.enabled = false;
            thisAnim.SetBool(IDLESTR, false);
            thisAnim.SetBool(SHOTSTR, false);
            thisAnim.SetBool(DIESTR, true);
        }

    }
    public override void ResetThisEnemy()
    {
        hpCur = hpMax;
        thisCol.enabled = true;
        thisAnim.SetBool(IDLESTR, true);
        thisAnim.SetBool(SHOTSTR, false);
        thisAnim.SetBool(DIESTR, false);
        thisAnim.speed = 1f;
        thisShooterState = ShooterState.idlestate;

    }

}
