using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using InteractiveAndInteractableEnums;
using StructForSaveData;
using static UnityEngine.RuleTile.TilingRuleOutput;
using InteractiveInterface;
using OpenerFactoryRelated;

public class OpenerController : MonoBehaviour, ISave<OpenerSaveData>
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
    public InteractiveConfigSO theInteractiveConfig;
    public openerType thisOpenerType;
    public DoorController theDoor;
    [Header("Opener Info")]
    public IOpener currentOpener;
    public bool isPressed;









    private void Awake()
    {
        thisAnim = GetComponentInChildren<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
        switch (thisOpenerType)
        {
            case openerType.trigerable_button:
                currentOpener = new ButtonFactory().CreateOpener(this);
                break;
            case openerType.triggerable_debris:
                currentOpener = new DebrisFactory().CreateOpener(this);
                break;
            case openerType.triggerable_key:
                currentOpener = new KeyFactory().CreateOpener(this);
                break;

        }
        currentOpener?.SceneLoad_Awake();
    }
    private void OnEnable()
    {
        theEC = ControllerManager.instance.theEvent;
        currentOpener?.SceneLoad_Enable();
    }










    private void Start()
    {

        currentOpener?.SceneLoad_Start();
    }




 




    private void Update()
    {

    }


    private void OnDisable()
    {

        SaveDataSync();
        
        ISaveable saveable = GetComponent<ISaveable>();
        theEC.SaveableUnregisterPublish(saveable);
    }

    #region Press相关
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController thePlayer))
        {

            currentOpener?.BePressed();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController thePlayer))
        {

            currentOpener?.BePressing();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        currentOpener?.Unpressed();
    }

    #endregion

    #region 调用方法
    public void CreateFollowKey()
    {
        Instantiate(theInteractiveConfig.theKeyPrefab, transform.position, Quaternion.identity);
    }
   

    public void ColOn()
    {
        thisBoxCol.enabled = true;
    }
    public void ColOff()
    {
        thisBoxCol.enabled = false;
    }
    public void Opener_DebrisReset()
    {
        isPressed = false;
        SetAnimUnpressed();
    }

    public void SetAnimCont(AnimatorController theAnimCont)
    {
        thisAnim.runtimeAnimatorController = theAnimCont;
    }

    public void SetAnimUnpressed()
    {
        thisAnim.SetBool(theInteractiveConfig.Opener_UNPRESSEDSTR, true);
    }

    public void SetAnimPressed()
    {
        thisAnim.SetBool(theInteractiveConfig.Opener_PRESSEDSTR, true);
    }

    public void SetAnimPressing()
    {
        thisAnim.SetTrigger(theInteractiveConfig.Opener_PRESSINGSTR);
    }
    public void SetAnimUnpressing()
    {

    }
    #endregion

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
