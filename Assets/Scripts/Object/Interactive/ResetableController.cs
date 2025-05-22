using InteractiveAndInteractableEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResetableController : MonoBehaviour
{

    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;
    [Header("AutoResetable Setting")]
    public resetableType thisRsetableType;
    [Header("AutoResetable Info")]
    public bool isPressed;

    [Header("Hider Related")]
    //要用单独的动画
    //public AnimatorController theHiderAnimator;
    public PlatformController theHidePlatform;
    public float ReappearCounter;
    public float ReappearDuration;

    [Header("Animator Related")]
    private const string PRESSEDSTR = "isPressed";
    private const string UNPRESSEDSTR = "isUnpressed";
    private const string PRESSINGSTR = "isPressing";
    private const string UNPRESSINGSTR = "isUnpressing";
    private void Awake()
    {
        thisAnim = GetComponentInChildren<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
    }
    private void OnEnable()
    {
        
    }
    private void Start()
    {
        SceneLoadSetting_Related();
    }
    // Update is called once per frame
    void Update()
    {
        
        switch (thisRsetableType)
        {
            case resetableType.autoresetable_hider:
                AutoResetable_Hider_Itself();
                break;
        }
    }

    private void SceneLoadSetting_Related()
    {
        switch (thisRsetableType)
        {
            case resetableType.autoresetable_hider:
                AutoResetable_Hider_Itself();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (thisRsetableType)
        {
            case resetableType.autoresetable_hider:
                AutoResetable_HiderPress();
                break;
        }
    }
    #region Opener_Hider
    private void AutoResetable_Hider_Itself()
    {


    }
    private void AutoResetable_Hider_Relative()
    {

    }
    private void AutoResetable_Hider_Related()
    {
        //thisAnim.runtimeAnimatorController = theHiderAnimator;
        thisBoxCol.enabled = true;
        thisAnim.SetBool(UNPRESSEDSTR, true);
    }

    private void AutoResetable_HiderPress()
    {
        if (!isPressed)
        {
            isPressed = true;
            thisAnim.SetTrigger(PRESSINGSTR);
            //theHidePlatform.HideThisPlatform();
            ReappearCounter = ReappearDuration;
        }
    }
    private void AutoResetable_Updata()
    {
        if (ReappearCounter > 0)
        {
            ReappearCounter -= Time.deltaTime;
        }
        else
        {
            isPressed = false;
            thisAnim.SetTrigger(UNPRESSINGSTR);
            //theHidePlatform.ReappearThisPlatform();
        }
    }
    #endregion
}
