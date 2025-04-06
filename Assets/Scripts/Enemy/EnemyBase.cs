using AttackInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IPhysicalAttack
{

    #region 组件
    protected Animator thisAnim;
    protected Collider2D thisCol;
    #endregion
    [SerializeField] protected Transform theDetectPoint;
    protected NewPlayerController thePlayer;
    protected virtual void Awake()
    {
        thisAnim = GetComponentInChildren<Animator>();
        thisCol = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        //读取缓存，没有缓存则直接置有

    }

    protected virtual void Update()
    {

    }


    public abstract void BePhysicalAttacked();

    public abstract void ResetThisEnemy();
    public void SetPlayer(NewPlayerController _theplayer)
    {
        thePlayer = _theplayer;
    }

    public NewPlayerController GetPlayer()
    {
        return thePlayer;
    }
    public void ClearPlayer()
    {
        thePlayer = null;
    }
    public bool isPlaying(string _chipName)
    {
        return thisAnim.GetCurrentAnimatorStateInfo(0).IsName(_chipName);
    }

    public float PlayingPercent()
    {
        return thisAnim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
    }


    public Vector3 thisEnemyPos()
    {
        return theDetectPoint.position;
    }
}
