using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaterPuzzle : LocalPuzzleBase
{

    /*
        1、涉及对象有：1）rotaters，用于在解密过程中展示可操作对象；
        2、解谜流程：1）使用leveler操作对应的rotater旋转，不同leveler对应不同rotater；
                     2）当rotater同时满足对应角度要求时，谜题完成。
     */
    [Header("——————————使用说明——————————\n1、设置所有Rotaer至rotaters\n2、设置所有对应的RightAngles至theCorrespondAngles")]
    [Header("Setting")]
    public PlatformController[] rotaters;
    [Header("Condition Related")]
    public float[] theCorrespondAngles;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        SceneLoad_Itself();
        SceneLoad_Relative();
    }
    protected override void Start()
    {
        base.Start();
        SceneLoad_Related();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    #region 抽象方法相关
    public override void SceneLoad_Itself()
    {

    }
    public override void SceneLoad_Relative()
    {

        if (!isSolved)
        {
            foreach (PlatformController rotater in rotaters)
            {
                rotater.ResetThisPlatform();
            }
        }
        else
        {
            for (int i = 0; i < rotaters.Length; i++)
            {
                Debug.Log("转了");
                rotaters[i].transform.RotateAround(rotaters[i].theRotatePovit_ElevatorPoint.position, Vector3.forward, theCorrespondAngles[i]);
                rotaters[i].nowAngle=theCorrespondAngles[i];
                rotaters[i].isHidden = true;
            }
        }
    }
    public override void SceneLoad_Related()
    {

    }

    public override void ThisPuzzleUpdate()
    {
        if (!isSolved)
        {
            if (CheckComplete())
            {
                FinishPuzzle();
            }

        }
    }

    public override bool CheckComplete()
    {

            bool locallPuzzleCompleted = true;
            for(int i = 0; i< rotaters.Length; i++)
            {
                if (rotaters[i].nowAngle != theCorrespondAngles[i])
                {
                    locallPuzzleCompleted = false;
                }
            }
            return locallPuzzleCompleted;
    }

    public override void FinishPuzzle()
    {
        isSolved = true;
        theEC.LocalEventPublish(localPublisherChannel_FinishPuzzle);
        foreach(PlatformController _rotater in rotaters)
        {
            _rotater.currentPlatform.Interact1();
        }

        //UnregisterLocalPuzzle(GetComponent<LocalPuzzleBase>());
    }


    public override void ActivatePuzzle()
    {
        //Debug.Log("无特定触发");
    }

    public override void UnactivatePuzzle()
    {
        //Debug.Log("无特定触发");
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
