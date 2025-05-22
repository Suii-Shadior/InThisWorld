using AttackableInterfaces;
using InteractiveAndInteractableEnums;
using IPhysicalAttackableFactoryRelated;
using NailerFactoryRelated;
using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class NailerController : MonoBehaviour,ISave<NailerSaveData>
{
    #region 组件
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;
    private EventController theEC;
    #endregion
    #region 

    [Header("Setting")]
    public AttackableConfigSO attackConfigSO;
    public nailerType thisNailerType;
    public NaelerFactory thisFactory;
    [Header("Info")]
    public NailerSaveData thisNailerSaveData;
    private IPhysicalAttackable currentBlocker;
    public bool isTriggered;

    [Header("Nailer Related")]
    public OnewayPlatform theOnewayPlatform;
    #endregion

    private void Awake()
    {

        thisAnim = GetComponentInChildren<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
        switch (thisNailerType)
        {
            case nailerType.sword_attackeder:
                thisFactory = new SworderFactory();
                break;
            default:
                break;
        }
        currentBlocker = thisFactory.CreateNailer(this);
    }
    private void OnEnable()
    {
        theEC = ControllerManager.instance.theEvent;
        SceneLoadSetting_Itself();
        SceneLoadSetting_Relative();
    }

    private void Start()
    {
        SceneLoadSetting_Related();
    }

    private void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<AttackArea>(out AttackArea attackArea))
        {
            currentBlocker?.BePhysicalAttacked(attackArea);

        }
    }

    private void OnDisable()
    {
        ISaveable _saveable = GetComponent<ISaveable>();
        SaveDataSync();
        theEC.SaveableUnregisterPublish(_saveable);
    }
    #region 初始化相关
    public void SceneLoadSetting_Itself()
    {
        /* 
         * Step1.注册可存储对象，获取内存存档内容
         * Step2.初始化
         *       
         */

        RegisterSaveable(GetComponent<ISaveable>());
        isTriggered = thisNailerSaveData.isTriggered;
        if (isTriggered)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(attackConfigSO.Nailer_ISTRIGGEREDSTR, true);
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(attackConfigSO.Nailer_ISUNTRIGGEREDSTR, true);
        }
    }
    public void SceneLoadSetting_Relative()
    {
        //可能还是要用简易工厂模式
        theOnewayPlatform.hasTriggered = isTriggered;
    }
    public void SceneLoadSetting_Related()
    {

    }
    #endregion

    public void AnimTriger()
    {
        thisAnim.SetTrigger(attackConfigSO.Nailer_ATTACKINGSTR);
        thisAnim.SetBool(attackConfigSO.Nailer_ISTRIGGEREDSTR, true);
    }
    public void CollidersEnable()
    {
        thisBoxCol.enabled = false;
    }

    #region ISave接口相关
    public NailerSaveData GetSpecificDataByISaveable(SaveData _passedData)
    {
        NailerSaveData thedata = JsonUtility.FromJson<NailerSaveData>(_passedData.value);
        //Debug.Log(thedata.isOpened);
        return thedata;
    }

    public string GetISaveID()
    {
        return thisNailerSaveData.SaveableID;
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
        thisNailerSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
    }

    public SaveData SaveDatalizeSpecificData()
    {
        SaveData _savedata = new()
        {
            key = thisNailerSaveData.SaveableID,
            type = TypeRegistry.GetDataTypeFromReverseDict(typeof(NailerSaveData)),
            value = JsonUtility.ToJson(thisNailerSaveData)
        };
        return _savedata;
    }


    public void SaveDataSync()
    {
        thisNailerSaveData.isTriggered = isTriggered;

}
    #endregion
}
