using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubInteractiveEnum;
using System;
using StructForSaveData;
using static UnityEngine.EventSystems.EventTrigger;


public class DoorController : MonoBehaviour, ISave<DoorSaveData>
{
    #region 组件和其他
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;
    private EventController theEC;

    public DoorSaveData thisDoorSaveData;
    #endregion


    [Header("————使用说明————\n1、选择适用的门类型，thisDoorType")]
    [Space(3)]
    [Header("——————————Door Setting——————————")]
    public DoorInteractType thisDoorType;
    public bool isOpened;
    [Header("——————————Door Info——————————")]
    [Header("Opener Related")]//通过物品交互打开，其中button指可分开交互保存的开启器，debris指可分开交互但必须一齐保存的开启器，key指可在不同场景中分开交互保存的开启器
    [Header("2、除Key外，添加相应的开门对象\n3、手动添加开门需要数量，即needToOpen")]
    public OpenerController[] Openers;
    public int needToOpen;

    [Header("Eventer Related")]//通过事件打开，其中local指场景内的事件，马上触发相应内容，record指存档内的内容，让相应内容在加载时改变
    public string localSubscriberChannel;

    [Header("Animator Related")]
    private const string OPENEDSTR = "isOpened";
    private const string CLOSEDSTR = "isClosed";
    private const string OPENNINGSTR = "isOpenning";
    private const string CLOSINGSTR = "isClosing";

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        theEC = ControllerManager.instance.theEvent;


        SceneLoadSetting_Itself();//读取缓存或者存档，获取对应SaveData内容并对本身进行初始化
        SceneLoadSetting_Relative();//根据SaveData的内容对本身涉及的其他非包含可存储内容的对象进行初始化


    }
    private void Start()
    {
        SceneLoadSetting_Related();//根据本身设计的其他包含可存储内容的对象进行初始化
    }
    // Update is called once per frame
    #region 初始化相关


    private void SceneLoadSetting_Itself()
    {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                Collector_Button_Itself();//读取存档，更新计数
                break;
            case DoorInteractType.opener_debris://读取存档
                Collector_Debris_Itself();
                break;
            case DoorInteractType.opener_key://读取存档，读取计数
                Collector_Key_Itself();
                break;
            case DoorInteractType.eventer_local:
                Eventer_Local_Itself();
                break;
            case DoorInteractType.eventer_global:
                Eventer_Global_Itself();
                break;

        }
    }


    #region 初始化相关
    #region Collector_Button
    private void Collector_Button_Itself()
    {

        RegisterSaveable(GetComponent<ISaveable>());
        isOpened = thisDoorSaveData.isOpened;
        needToOpen = thisDoorSaveData.needToOpen;
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);

        }

    }
    private void Collector_Button_Relative()
    {


    }
    private void Collector_Button_Related()
    {
        if (isOpened)
        {
            //Debug.Log("");
        }
        else
        {

        }
    }


    #endregion


    #region Collector_Debris
    private void Collector_Debris_Itself()
    {
        //读取存档
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = true;
            }
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);
            //计数部分
            needToOpen = Openers.Length;
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = false;
            }
        }

    }

    private void Collector_Debris_Relative()
    {
        //LoadThisISave();

    }
    private void Opener_Debris_Related()
    {

    }
    #endregion


    #region Collector_Key
    private void Collector_Key_Itself()
    {

        RegisterSaveable(GetComponent<ISaveable>());
        isOpened = thisDoorSaveData.isOpened;
        needToOpen = thisDoorSaveData.needToOpen;
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);

        }

    }

    private void Collector_Key_Relative()
    {
       // LoadThisISave();

    }
    private void Collector_Key_Related()
    {
        if (isOpened)
        {
            //Debug.Log("");
        }
        else
        {

        }
    }
    #endregion

    #region Eventer_Local
    private void Eventer_Local_Itself()//可以自行设定是否需要事件关门，默认在本地事件开门
    {
        RegisterSaveable(GetComponent<ISaveable>());
        isOpened = thisDoorSaveData.isOpened;
        needToOpen = thisDoorSaveData.needToOpen;
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);

        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);
        }
    }
    private void Eventer_Local_Relative()
    {

        if (!isOpened)
        {
            theEC.OnLocalEvent += OnLoaclEventer;

        }
    }
    private void Eventer_Local_Related()
    {

    }
    #endregion
    #region Eventer_Global
    private void Eventer_Global_Itself()//默认仅根据缓存或存档开门；
    {
        //读取存档
        thisBoxCol.enabled = true;
        thisAnim.SetBool(CLOSEDSTR, true);
    }
    private void Eventer_Global_Relative()
    {
        theEC.SaveableRegisterPublish(this.GetComponent<ISaveable>());
    }
    private void Eventer_Global_SLRelatedLater()
    {

    }

    #endregion
    #endregion
    private void SceneLoadSetting_Relative()
    {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                Collector_Button_Relative();//空内容
                break;
            case DoorInteractType.opener_debris://读取存档
                Collector_Debris_Relative();
                break;
            case DoorInteractType.opener_key://读取存档，读取计数
                Collector_Key_Relative();
                break;
            case DoorInteractType.eventer_local:
                Eventer_Local_Relative();//读取存档
                break;
            case DoorInteractType.eventer_global:
                Eventer_Global_Relative();//读取存档
                break;

        }
    }






     private void SceneLoadSetting_Related()
     {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                Collector_Button_Related();
                break;
            case DoorInteractType.opener_debris:
                Collector_Debris_Relative();
                break;
            case DoorInteractType.opener_key:
                Collector_Key_Related();
                break;
            case DoorInteractType.eventer_local:
                Eventer_Local_Related();
                break;
            case DoorInteractType.eventer_global:
                Eventer_Global_SLRelatedLater();
                break;
        }
    }



    #endregion

    void Update()//也可以用代码更新
    {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                break;
            case DoorInteractType.opener_debris://读取存档
                break;
            case DoorInteractType.opener_key://读取存档，读取计数
                break;
            case DoorInteractType.eventer_local:
                Eventer_LocalUpdate();
                break;
            case DoorInteractType.eventer_global:
                Eventer_Global_Itself();
                break;

        }
    }


    private void Eventer_LocalUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            theEC.LocalEventPublish(localSubscriberChannel);
        }
    }
    private void Eventer_GlobalUpdate() 
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            theEC.LocalEventPublish(localSubscriberChannel);
        }
    }

    private void OnDisable()
    {
        SaveDataSync();
        theEC.SaveableUnregisterPublish(GetComponent<ISaveable>());
    }








    #region 重置相关
    public void ResetThisDoor()//这个要监听触发,目前只有debris有这个需求
    {

        switch (thisDoorType)
        {
            case DoorInteractType.opener_debris:
                Opener_DebtisReset();
                break;
            case DoorInteractType.eventer_local:
                Eventer_LocalReset();
                break;
        }
    }

    private void Opener_DebtisReset()
    {
        needToOpen = Openers.Length;
        foreach (OpenerController _opener in Openers)
        {
            _opener.Opener_DebrisReset();
        }
    }
    private void Eventer_LocalReset()
    {
        thisBoxCol.enabled = false;
        thisAnim.SetBool(OPENEDSTR, true);
    }



    #endregion


    #region 小方法和外部调用

    private void OnLoaclEventer(string _localEventerChannel)
    {
        if (!isOpened)
        {
            if(_localEventerChannel == localSubscriberChannel)
            {
                Debug.Log("本地事件门调用");
                isOpened = true;
                thisBoxCol.enabled = false;
                thisAnim.SetTrigger(OPENNINGSTR);
                //theEC.OnLocalEvent -= OnLoaclEventer;
            }
            else
            {
                Debug.Log("并非本地事件门");

            }
        }
        else
        {

        }

    }


    public void OpenTheDoor()
    {

        if (needToOpen > 0)
        {
            needToOpen--;
            if (needToOpen == 0)
            {
            isOpened = true;
            thisBoxCol.enabled = false;
            thisAnim.SetTrigger(OPENNINGSTR);
            }
        }

    }



     public void CloseTheDoor()
    {
        isOpened = false;
        thisAnim.SetTrigger(CLOSEDSTR);
        thisBoxCol.enabled = false;
    }






    #region ISave接口实现




    public string GetISaveID()
    {
        return thisDoorSaveData.SaveableID;
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
       thisDoorSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
        //Debug.Log(thisDoorSaveData.isOpened);

    }

    public DoorSaveData GetSpecificDataByISaveable(SaveData _passedData)
    {
        DoorSaveData thedata = JsonUtility.FromJson<DoorSaveData>(_passedData.value);
        //Debug.Log(thedata.isOpened);
        return thedata;
    }


    public SaveData SaveDatalizeSpecificData()
    {
        SaveData _savedata = new()
        {
            key = thisDoorSaveData.SaveableID,
            type = TypeRegistry.GetDataTypeFromReverseDict(typeof(DoorSaveData)),
            value = JsonUtility.ToJson(thisDoorSaveData)
        };
        return _savedata;
    }

    public void SaveDataSync()
    {
        thisDoorSaveData.isOpened = isOpened;
        thisDoorSaveData.needToOpen = needToOpen;
    }




    #endregion
    #endregion
}
