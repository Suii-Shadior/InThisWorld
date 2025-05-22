using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using InteractiveAndInteractableEnums;
using StructForSaveData;

public class OpenerController:MonoBehaviour,ISave<OpenerSaveData>
{
    #region 组件
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;
    private EventController theEC;
    #endregion
    public OpenerSaveData thisOpenerSaveData;
    [Header("————使用说明————\n1、选择本脚本适用的开门器类型，thisOpenerType\n2、添加相应的门对象\n3、根据种类同进行设定，详情看对应Related")]
    [Space(3)]

    [Header("Opener Setting")]
    public openerType thisOpenerType;
    public DoorController theDoor;
    [Header("Opener Info")]

    public bool isPressed;



    [Header("Button Related")]
    public AnimatorController theButtonAnimator;
    [Header("Debris Related")]
    public AnimatorController theDebrisAnimator;
    [Header("Key Related")]
    public AnimatorController theKeyAnimator;
    public GameObject theKeyPrefab;


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
        theEC = ControllerManager.instance.theEvent;
        SceneLoadSetting_Itself();
        SceneLoadSetting_Relative();
    }

    private void SceneLoadSetting_Itself()
    {
        switch (thisOpenerType)
        {
            case openerType.trigerable_button:
                Opener_Button_Itself();
                break;
            case openerType.triggerable_debris:
                Opener_Debris_Itself();
                break;
            case openerType.triggerable_key:
                Opener_Key_Itself();
                break;

        }
    }


    private void SceneLoadSetting_Relative()
    {
        switch (thisOpenerType)
        {
            case openerType.trigerable_button:
                Opener_Button_Relative();
                break;
            case openerType.triggerable_debris:
                Opener_Debris_Relative();
                break;
            case openerType.triggerable_key:
                Opener_Key_Relative();
                break;

        }
    }





    private void Start()
    {
        SceneLoadSetting_Related();
    }


    private void SceneLoadSetting_Related()
    {
        switch (thisOpenerType)
        {
            case openerType.trigerable_button:
                Opener_Button_Related();
                break;
            case openerType.triggerable_debris:
                Opener_Debris_Related();
                break;
            case openerType.triggerable_key:
                Opener_Key_Related();
                break;

        }
    }

    #region 初始化相关

    #region Opener_Button

    private void Opener_Button_Itself()
    {
        thisAnim.runtimeAnimatorController = theButtonAnimator;
        RegisterSaveable(GetComponent<ISaveable>());
        isPressed = thisOpenerSaveData.isPressed;

        if (isPressed)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(PRESSEDSTR, true);
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(UNPRESSEDSTR, true);
        }
    }
    private void Opener_Button_Relative()
    {

    }
    private void Opener_Button_Related()
    {

    }
    #endregion

    #region Opener_Debris
    private void Opener_Debris_Itself()
    {
        Debug.Log("运行了吗？");
        thisAnim.runtimeAnimatorController = theDebrisAnimator;
        thisBoxCol.enabled = true;
        thisAnim.SetBool(UNPRESSEDSTR, true);
    }
    private void Opener_Debris_Relative()
    {

    }
    private void Opener_Debris_Related()
    {

    }



    #endregion

    #region Opener_Key
    private void Opener_Key_Itself()
    {
        thisAnim.runtimeAnimatorController = theKeyAnimator;
        RegisterSaveable(GetComponent<ISaveable>());
        isPressed = thisOpenerSaveData.isPressed;
        if (isPressed)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(PRESSEDSTR, true);
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(UNPRESSEDSTR, true);
        }
    }
    private void Opener_Key_Relative()
    {

    }

    private void Opener_Key_Related()
    {

    }

    #endregion


    #endregion

    private void Update()
    {
        switch (thisOpenerType)
        {
            case openerType.trigerable_button:
                break;
            case openerType.triggerable_debris:
                break;
            case openerType.triggerable_key:

                break;

        }
    }


    private void OnDisable()
    {

        SaveDataSync();
        theEC.SaveableUnregisterPublish(GetComponent<ISaveable>());
    }

    #region Press相关
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            switch (thisOpenerType)
            {
                case openerType.trigerable_button:
                    Opener_ButtonPress();
                    break;
                case openerType.triggerable_debris:
                    Opener_DebrisPress();
                    break;
                case openerType.triggerable_key:
                    Opener_KeyPress();
                    break;

            }
        }
    }

    private void Opener_DebrisPress()
    {
        if (!isPressed)
        {
            isPressed = true;
            thisAnim.SetTrigger(PRESSINGSTR);
            theDoor.OpenTheDoor();
        }
    }

    private void Opener_ButtonPress()
    {
        if (!isPressed)
        {
            isPressed = true;
            thisAnim.SetTrigger(PRESSINGSTR);
            theDoor.OpenTheDoor();
        } 

    }

    private void Opener_KeyPress()
    {
        if (!isPressed)
        {
            isPressed = true;
            thisAnim.SetTrigger(PRESSINGSTR);
            Instantiate(theKeyPrefab, transform.position, Quaternion.identity);
        }
    }



    #endregion



    public void Opener_DebrisReset()
    {
        isPressed = false;
        thisAnim.SetTrigger(UNPRESSINGSTR);
    }





    #region ISave接口相关
    public string GetISaveID()
    {
        return thisOpenerSaveData.SaveableID;
    }
    public void RegisterSaveable(ISaveable _saveable)
    {
        theEC.SaveableRegisterPublish(_saveable);
    }

    public void UnregisterSaveable(ISaveable _saveable)
    {
        theEC.SaveableUnregisterPublish(_saveable);   
    }

    public void LoadSpecificData(SaveData _passingData)
    {
        thisOpenerSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
    }


    public SaveData SaveDatalizeSpecificData()
    {
        SaveData _savedata = new()
        {
            key = thisOpenerSaveData.SaveableID,
            type = TypeRegistry.GetDataTypeFromReverseDict(typeof(OpenerSaveData)),
            value = JsonUtility.ToJson(thisOpenerSaveData)
        };
        return _savedata;
    }
    public void SaveDataSync()
    {
        thisOpenerSaveData.isPressed = isPressed;
    }
    public OpenerSaveData GetSpecificDataByISaveable(SaveData _passedData)
    {
        OpenerSaveData thedata = JsonUtility.FromJson<OpenerSaveData>(_passedData.value);
        //Debug.Log(thedata.isOpened);
        return thedata;
    }





    #endregion
}
