using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlatformController;

[RequireComponent(typeof(BoxCollider2D),typeof(SpriteRenderer),typeof(Animator))]
public class PlatformUnit : MonoBehaviour
{
    #region 组件
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;
    private SpriteRenderer thisSR;
    #endregion
    #region 变量

    [Header("Unit Info")]
    private bool isHidden;

    [Header("Platform Related")]
    private PlatformController parentController;

    [Header("Animator Related")]
    private const string HiddenStr = "isHidden";
    private const string AppearedStr = "isAppeared";
    private const string HidingStr = "isHiding";
    private const string AppearingStr = "isReappearing";

    [Header("Additions")]
    private List<PlatformUnit> adjacentUnits = new List<PlatformUnit>();

    #endregion
    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
        thisSR = GetComponent<SpriteRenderer>();
        parentController = GetComponentInParent<PlatformController>();
    }

    private void Start()
    {
        SceneLoadSetting_PlatformUnit();//根据Unit所属的Platform进行初始化
    }

    #region 初始化相关
    private void SceneLoadSetting_PlatformUnit()
    {
        isHidden = parentController.isHidden;
        if (isHidden)
        {
            thisAnim.SetBool(HiddenStr, true);
            thisAnim.SetBool(AppearedStr, false);
            ColOff();
        }
        else
        {
            thisAnim.SetBool(HiddenStr, false);
            thisAnim.SetBool(AppearedStr, true);
            ColOn();
        }
    }
    #endregion

    #region 外部调用

    public void Hide()//隐藏，玩家不能再踩上去
    {
        thisAnim.SetTrigger(HidingStr);

    }
    public void Appear()//显现，玩家可以再踩上去
    {
        thisAnim.SetTrigger(AppearingStr);
    }

    public void ColOn()
    {
        thisBoxCol.enabled = true;

    }
    public void ColOff()
    {
        thisBoxCol.enabled = false;

    }
    #endregion




}
