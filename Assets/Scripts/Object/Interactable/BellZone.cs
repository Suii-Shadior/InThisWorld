using AttackableInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellZone : MonoBehaviour,IPhysicalAttackable
{
    #region 组件
    private Animator thisAnim;
    private Collider2D thisBellCol;

    #endregion
    [Header("Setting")]
    public bool isNeedToAttack;
    [Header("Info")]
    public BellPuzzle thePuzzle;

    private SpriteRenderer needToAttackIcon;
    private SpriteRenderer normalIcon;

    [Header("Animator Related")]
    private const string ISNEEDTOATTACK = "isNeedToAttack";
    //private const string OPENEDSTR = "isOpened";
    //private const string CLOSEDSTR = "isClosed";
    private const string OPENNINGSTR = "isOpenning";
    private const string CLOSINGSTR = "isClosing";


    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        thisBellCol = GetComponent<Collider2D>();
    }

    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<AttackArea>(out AttackArea attackArea))
        {
            BePhysicalAttacked(attackArea);
        }
    }




    #region 小方法和外部调用

    public void SetPuzzle(BellPuzzle _puzzle)
    {
        thePuzzle = _puzzle;
    }

    public void CloseWindow()
    {
        thisAnim.SetTrigger(CLOSINGSTR);
        thisBellCol.enabled = false;
    }
    public void OpenWindow()
    {
        thisAnim.SetBool(ISNEEDTOATTACK, isNeedToAttack);
        thisAnim.SetTrigger(OPENNINGSTR);
        thisBellCol.enabled = true;
    }

    public void BePhysicalAttacked(AttackArea attackArea)
    {
        if (isNeedToAttack)
        {
            thePuzzle.RightAttack();
        }
        else
        {
            thePuzzle.WrongAttack();
        }
    }
    #endregion
}
