using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubInteractiveEnum;
public class OnewayPlatform : MonoBehaviour
{
    #region 组件
    private Animator thisAnim;
    private SpriteRenderer thisSR;
    private BoxCollider2D thisBoxCol;
    #endregion

    #region 变量
    [Header("OnewayPlatform Setting")]
    public OnewayPlatformType thisOnewayPlatformType;

    [Header("OnewayPlatform Info")]
    public float theTriggeringLength;


    [Header("Triggerable Related")]
    public bool isTriggering;
    public bool isLefttward;
    public bool hasTriggered;
    public float theOriginLength;
    public float theTriggeredLength;
    public float triggeringRatio;

    [Header("Static Related")]
    public bool RightUpBracket;
    public bool RightDownBracket;
    public bool LeftUpBracket;
    public bool LeftDownBracket;

    [Header("Animator Related")]
    private const string HASCHANGEDSTR = "hasTriggered";
    private const string UNCHANGEDSTR = "isUntriggered";
    private const string CHANGINGSTR = "isTriggering";
    #endregion


    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        thisSR = GetComponent<SpriteRenderer>();
        thisBoxCol = GetComponent<BoxCollider2D>();
    }
    void Start()
    {
        SceneLoadSetting_Related();

    }

    #region 初始化相关
    private void SceneLoadSetting_Related()
    {
        switch (thisOnewayPlatformType)
        {
            case OnewayPlatformType.static_platform:
                Static_Start();
                break;
            case OnewayPlatformType.triggerable_platform:
                Triggerable_Start();
                break;

        }
    }
    private void Static_Start()
    {
        thisAnim.SetBool(UNCHANGEDSTR, true);
        thisBoxCol.size = thisSR.size;
        thisBoxCol.offset = new Vector2(thisSR.size.x/2, 0);
    }
    private void Triggerable_Start()
    {
        if (hasTriggered)
        {
            //thisAnim.SetBool(HASCHANGEDSTR, true);
            theTriggeringLength = isLefttward?-theTriggeredLength: theTriggeredLength;
            ChangeLength();
        }
        else
        {
            thisAnim.SetBool(UNCHANGEDSTR, true);
            theOriginLength = thisSR.size.x;
            theTriggeringLength = thisSR.size.x;
            ChangeLength();
        }
    }


    #endregion
    private void Update()
    {
        switch (thisOnewayPlatformType)
        {
            case OnewayPlatformType.triggerable_platform:
                Triggerable_Update();
                break;

        }
    }


    #region Update相关
    private void Triggerable_Update()
    {
        if (isTriggering)
        {
            if (isLefttward)
            {

                theTriggeringLength = Mathf.Lerp(theTriggeringLength, -theTriggeredLength, triggeringRatio);
                if (Mathf.Abs(theTriggeringLength - theTriggeredLength) < 0.1f)
                {
                    theTriggeringLength = -theTriggeredLength;
                    isTriggering = false;
                    hasTriggered = true;
                }
            }
            else
            {
                theTriggeringLength = Mathf.Lerp(theTriggeringLength, theTriggeredLength, triggeringRatio);
                if (Mathf.Abs(theTriggeringLength - theTriggeredLength) < 0.1f)
                {
                    theTriggeringLength = theTriggeredLength;
                    isTriggering = false;
                    hasTriggered = true;
                }
            }
            ChangeLength();
        }



    }
    #endregion


    #region 小方法
    public void TriggerThisPlatform()
    {
        isTriggering = true;
    }

    private void ChangeLength()
    {

        thisSR.size = new Vector2(theTriggeringLength, thisSR.size.y);
        thisBoxCol.size = new Vector2 (Mathf.Abs(thisSR.size.x), thisSR.size.y);
        thisBoxCol.offset = new Vector2(theTriggeringLength/2, 0);
    }
    #endregion
}
