using OtherEnum;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(EdgeCollider2D))]
public class BlockerController : AttackableBase
{
    #region 组件
    private SpriteRenderer thisSR;
    private EdgeCollider2D thisEdgeCol;
    #endregion

    [Header("Blocker Related")]
    [Header("1、先将子对象的SR对象的大小调整至需要大小\n2、将父对象放到对应位置\n3、选择可攻击方向，即canAttackDir")]
    public BeAttackedDir canAttackDir;



    protected override void Awake()
    {
        base.Awake();
        thisEdgeCol = GetComponent<EdgeCollider2D>();
        thisSR = GetComponentInChildren<SpriteRenderer>();
    }
    protected override void Start()
    {
        base.Start();
        if (!hasAttacked)
        {
            thisBoxCol.size = thisSR.size;
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
            thisEdgeCol.enabled = false;
            //this.gameObject.SetActive(false);
        }
    }

    protected override void Update()
    {
        base.Update();
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }



    public override void BePhysicalAttacked()
    {
        switch (canAttackDir)
        {
            case BeAttackedDir.rightwardDir:
                if (attackVec2 == new Vector2(-1, 0))
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
    private void HitThisObstacle()
    {
        Debug.Log("成功");
        needToTriggered--;
        thisAnim.SetTrigger(ATTACKINGSTR);
        if (needToTriggered == 0)
        {
            thisBoxCol.enabled = false;
            thisEdgeCol.enabled = false;
            thisAnim.SetBool(HASATTACKEDSTR, true);

        }
        else
        {
            thisAnim.SetTrigger(ATTACKINGSTR);
            beAttackedCounter = beAttackedDuration;
        }

    }
}
