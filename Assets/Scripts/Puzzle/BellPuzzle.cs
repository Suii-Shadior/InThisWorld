using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellPuzzle : LocalPuzzleBase
{
    /*
    1、涉及对象有：1）bellzones，用于在解密过程中展示可击打对象；2）indications，用于记录已经正确完成的次数；3）counterboard，用于记录本次游戏已经过去的时间
    2、解谜流程： 1）解谜激活后，counterboard开始计时，相关场景门关闭(事件)，除玩家死亡或计时完成或谜题完成后，场景门打开(事件)。
                  2）在bellzones中随机一个对象为本轮击打对象，并打开所有bellzones的门，玩家可以通过视觉确定本轮应击打哪个对象；
                  3）玩家若在可击打时间内击打对象，则所有bellzones门关闭。若玩家击打的正确对象，则正确击打数加一，同时indications同步。若未击打正确对象或超过击打时间。则不改变任何其他内容；
                  4）进行重开门倒计时，若倒计时后仍在counterboard计时内，则回到2）；
                  5）counterboard计时内，若正确击打数达到目标，则谜题完成。若计时结束后，则解谜重置。
     */
    [Header("——————————使用说明——————————\n1、拖入所有BellZone至bellZones\n2、设置所有Indication至theIndications\n3、设置攻击持续事件和重新开门时间")]
    [Header("Setting")]
    public BellZone[] bellZones;//
    public SpriteRenderer[] theIndications;
    public float attackableDuration;
    public float reopenDuration;
    [Header("Info")]
    public bool bellWinsClosing;
    public float attackableCounter;
    public float reopenCounter;
    [Header("Indications Related")]
    public Sprite rightAttackIndication;
    //public Sprite wrongIndication;
    public Sprite unattackedIndication;
    [Header("Condition Related")]
    public int totalTurn;
    public int needToComplete;



    protected override void Awake()
    {
        base.Awake();//空内容
        theEC.OnLocalEvent += OnLocalEventer;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        theEC.LoaclPuzzleRegisterPublish(this);
        SceneLoad_Itself();
    }
    protected override void Start()
    {
        base.Start();

    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }




    #region 抽象方法相关
    public override void SceneLoad_Itself()
    {
        if (!isSolved)
        {

        }
        else
        {

        }
    }

    public override void SceneLoad_Relative()
    {
        if (!isSolved)
        {
            foreach (BellZone bellzone in bellZones)//Step2.遍历所有涉及对象让其知道该谜题
            {
                bellzone.SetPuzzle(this);
            }
            needToComplete = totalTurn;
            foreach (SpriteRenderer indication in theIndications)
            {
                indication.sprite = unattackedIndication;
            }
        }
        else
        {

        }
    }

    public override void SceneLoad_Related()
    {
        throw new System.NotImplementedException();
    }


    public override void ThisPuzzleUpdate()
    {
        if (!isSolved)
        {
            if (isActive)
            {
                if (bellWinsClosing)
                {
                    if (reopenCounter > 0)
                    {
                        reopenCounter -= Time.deltaTime;
                    }
                    else
                    {
                        OpendWindows();
                    }
                }
                else
                {
                    if(attackableCounter > 0)
                    {
                        attackableCounter -= Time.deltaTime;
                    }
                    else
                    {
                        CloseWindows();
                    }
                }
                if (CheckComplete())
                {
                    FinishPuzzle();
                }
            }
        }
    }
    public override bool CheckComplete()
    {
        if (needToComplete == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void FinishPuzzle()
    {
        isSolved = true;
        theEC.LocalEventPublish(localPublisherChannel_FinishPuzzle);
        UnregisterLocalPuzzle(GetComponent<LocalPuzzleBase>());

    }



    public override void ActivatePuzzle()
    {
        isActive = true;

        OpendWindows();
    }
    public override void UnactivatePuzzle()
    {
        isActive = false;
        CloseWindows();

    }

    #endregion


    #region 小方法和外部调用
    public void RightAttack()
    {
        //播放正确音效
        CloseWindows();
        theIndications[totalTurn-needToComplete].sprite = rightAttackIndication;
        needToComplete--;

    }
    public void WrongAttack()
    {
        //播放错误音效
        CloseWindows();

    }

    public void CloseWindows()
    {
        bellWinsClosing = true;
        foreach (BellZone bellzone in bellZones)
        {
            bellzone.CloseWindow();
        }
        if (isActive)
        {
            reopenCounter = reopenDuration;
        }
    }

    public void OpendWindows()
    {
        bellWinsClosing = false;
        int isNeedToAttack = (int)Random.Range(0, 5);
        for(int i =0;i<=bellZones.Length;i++)
        {
            if (i == isNeedToAttack)
            {
                bellZones[i].isNeedToAttack = true;
            }
            else
            {
                bellZones[i].isNeedToAttack = false;

            }
            bellZones[i].OpenWindow();
        }
        if (isActive)
        {
            attackableCounter = attackableDuration;
        }
    }

    public override void OnLocalEventer(string _localEventerChannel)
    {
        if (canBeActivated)
        {
            if(_localEventerChannel == localSubscriberChannel_ActivatePuzzle)
            {
                if (!isActive)
                {
                    ActivatePuzzle();
                }
            }
            else if(_localEventerChannel == localSubscriberChannel_UnactivatePuzzle)
            {
                if (isActive)
                {
                    UnactivatePuzzle();
                }
            }
        }

    }
    #endregion
}
