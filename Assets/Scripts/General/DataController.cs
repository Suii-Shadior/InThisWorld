using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;
using StructForSaveData;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEditor.Progress;

public class DataController : MonoBehaviour
{
    #region 组件
    private ControllerManager thisCM;
    private EventController theEC;
    private UIController theUI;
    private LevelController theLevel;
    private NewPlayerController thePlayer;
    public DataConfig dataConfig;
    #endregion

    /* 存档系统层次结构：
     * 外部文档 ==》saveableObjectsSaveData/内存存档 ==》saveableObjectsCacheData/缓存 ==》activitySaveableObjects
     * 
     * 前置条件：
     * 1.本脚本维护层次结构中除外部文档以外的所有内容（可能性能上仍有很多提高空间，本实现实验目的为主）
     * 2.本脚本从游戏运行时至游戏关闭时都一直存在且唯一
     * 3.游戏内所有包含可存储内容的对象都挂载了ISaveable接口
     * 
     * 实现思路：
     * 1.在脚本运行之初，从外部文档将所有存档内容写入saveableObjectsSaveData
     * 2.在场景加载时，加载场景内所有的ISaveable接口都会注册进activitySaveableObjects，同时查询是否已在缓存中存在，若是，读取其值并按照该值初始化，若否，读取内存存档值并将内容添加至缓存
     * 3.在场景卸载时，被卸载场景内的所有ISaveable接口都会从activitySaveableObjects注销，同时将对应可存储内容写入saveableObjectsCacheData
     * 4.在实现保存时，先将当前场景所有可存储内容写入缓存，再将缓存所有可存储内容写入内存存档，然后将内存存档写入外部文档。同时将缓存清空，重新将活动可存储内容对象对应可存储内容从内存存档获取并写入缓存
     * 
     */


    #region 变量
    [Header("Cache Related")]
    [Tooltip("使用List<ISaveable>的原因：\n" +
        "1.大小是浮动的，数组无法实现\n" +
        "2.可序列化，能够在Inspector直观的展示当前的内容，字典无法实现\n" +
        "3.在写入字典的时候需要可以直接通过列表元素获取其存档内容的方法，ISaveableID无法实现")]
    public List<ISaveable> activitySaveableObjects;
    [Tooltip("使用Dictionary<string, string>的原因：\n" +
        "1.字典查找时间复杂度O（1），List无法实现\n" +
        "2.在场景卸载与加载时，即使是同一个对象的ISaveable对应的InstanceID也不一样，所以无法使用ISaveable作为键\n" +
        "3.将存档类型信息封装进了SaveData中，不只使用具体可存储内容的Jsonstring，牺牲了一定的效率，保留了灵活性")]
    private Dictionary<string, SaveData> saveableObjectsCacheData;
    [Header("SaveFile Related")]
    private string currentSaveFolderPath;
    private string currentSaveFliePath;
    private string currentSaveDataOverview;
    public bool RelativeAddressMode;
    [Tooltip("使用Dictionary<string, string>的原因：\n" +
        "1.字典查找时间复杂度O（1），List无法实现\n" +
        "2.该层不用再获取存档内容，且ISaveableID可以更加方便的实现全局内定向查找,方便全局谜题的检测\n" +
        "3.将存档类型信息封装进了SaveData中，不只使用具体可存储内容的Jsonstring，牺牲了一定的效率，保留了灵活性")]
    private Dictionary<string, SaveData> saveableObjectsSaveData;
    private DictionaryWrapper theSaveDataPackage;
    #endregion
    #region 常量



    //private const string ORIGINALSCENENAMESTR = "SideA_P0-R1";
    #endregion


    private void Awake()
    {
        /*
         * 
         * Work1.获取各种组件，注册EC相应事件
         * Work2.从PlayerPrefs中获取玩家偏好，以及上次保存的存档序号（若无，表明还没有存档，则创建初次游玩存档）
         * Work3.将相关玩家偏好设置内容传递给相关Controller
         * Work4.获取存档文件夹路径————后期会变
         * Work5.初始化活跃可存储内容对象表，并从缓存或存档中读取相关内容
         * 
         */

        thisCM = GetComponentInParent<ControllerManager>();
        theUI = thisCM.theUI;
        theEC = thisCM.theEvent;
        theLevel = thisCM.theLevel;
        thePlayer = thisCM.thePlayer;
        theEC.OnSaveableRegister += ISaveableRegister;
        theEC.OnSaveableUnregister += ISaveableUnregister;
        GetSaveDataIndexFromPlayerPrefs();

        SendPlayerPrefsToUI();
        //SaveDataCheck();
        currentSaveFolderPath = RelativeAddressMode ? dataConfig.SAVEFOLDERPATHLOCALSTR : dataConfig.SAVEFOLDERPATHSTR;

        activitySaveableObjects = new();
        saveableObjectsCacheData = new();
        saveableObjectsSaveData = new();
        LoadDictionaryFromJson();

    }
    private void OnEnable()
    {
        
    }
    void Start()
    {
       
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (string savecache in saveableObjectsCacheData.Keys)
            {
                Debug.Log(savecache + ":" + saveableObjectsCacheData[savecache].key);
            }
        }
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    foreach (string savecache in saveableObjectsSaveData.Keys)
        //    {
        //        Debug.Log(savecache + ":" + saveableObjectsSaveData[savecache].key);
        //    }
        //}
    }



    #region 小方法和外部调用
    private void GetSaveDataIndexFromPlayerPrefs()
    {
        //用于选择存档位等众多适合存在PlayerPrefs中的内容,适用于游戏最初运行时
        if(!PlayerPrefs.HasKey(dataConfig.FILEPATHSTR))
        {
            currentSaveFliePath = PlayerPrefs.GetString(dataConfig.FILEPATHSTR);
        }
        else
        {
            currentSaveFliePath = dataConfig.FILEPATH1STR;
        }

    }
    public void SaveDataCheck()
    {
        if(File.Exists(Path.Combine(currentSaveFolderPath, dataConfig.FILEPATH1STR + ".json")))
        {
            string saveData1Overview = PlayerPrefs.GetString(dataConfig.OVERVIEW1STR);
            Debug.Log(saveData1Overview);
            SaveDataOverview overview1 = JsonUtility.FromJson<SaveDataOverview>(saveData1Overview);
            theUI.saveDataOverviews.Add(overview1);
            //theUI.saveDataOverviews.Add(JsonUtility.FromJson<SaveDataOverview>(saveData1Overview));

        }
        else
        {
            theUI.saveDataOverviews.Add(new SaveDataOverview());
        }
        if (File.Exists(Path.Combine(currentSaveFolderPath, dataConfig.FILEPATH2STR + ".json")))
        {
            string saveData2Overview = PlayerPrefs.GetString(dataConfig.OVERVIEW2STR);
            theUI.saveDataOverviews.Add(JsonUtility.FromJson<SaveDataOverview>(saveData2Overview));
        }
        else
        {
            theUI.saveDataOverviews.Add(new SaveDataOverview());
        }
        if (File.Exists(Path.Combine(currentSaveFolderPath, dataConfig.FILEPATH3STR + ".json")))
        {
            string saveData2Overview = PlayerPrefs.GetString(dataConfig.OVERVIEW3STR);
            theUI.saveDataOverviews.Add(JsonUtility.FromJson<SaveDataOverview>(saveData2Overview));
        }
        else
        {
            theUI.saveDataOverviews.Add(new SaveDataOverview());
        }
    }
    private void SendPlayerPrefsToUI()
    {
        /* 本方法用于将各Controller需要的玩家偏好内容在此处统一调用，适用于游戏加载时
         * 
         * 不在各自Controller脚本中自行加载主要考虑到总线管理和传递顺序可能存在潜在的需求
         * 
         */
        theUI.GetPlayerPrefsFromData(PlayerPrefs.GetInt(dataConfig.HADSTARTGAMESTR) == 1);
    }
    private void LoadDictionaryFromJson()
    {
        /* 这个方法用于从外村存档内容写入内存存档，适用于游戏运行之初
         * 
         * Step1.判断路径是否存在文件，若存在读取Json文件，并关闭此文件
         * Step2.将Json文件读取取为DictionaryWrapper，根据其结构写入内存存档。
         * Step3.将读取过程相应文件进行清理
         * 
         * 
         * 
         */
        //
        if (File.Exists(Path.Combine(currentSaveFolderPath, currentSaveFliePath + ".json")))
        {
            Debug.Log("存档存在且开始读取");
            StreamReader theSR = new(Path.Combine(currentSaveFolderPath, currentSaveFliePath + ".json"));
            string json = theSR.ReadToEnd();
            theSR.Close();

            theSaveDataPackage = JsonUtility.FromJson<DictionaryWrapper>(json);
            foreach(SaveData _saavedata in theSaveDataPackage.items)
            {
                saveableObjectsSaveData.Add(_saavedata.key, _saavedata);
                //Debug.Log(_saavedata.key + "对象已放入内存存档");
            }

            theSaveDataPackage = null;
        }
        else
        {
            //saveableObjectsSaveData = oringinaldata;
            Debug.Log("存档不存在");
        }
    }
    private void ISaveableRegister(ISaveable _saveable)
    {
        /* 这个方法用于将可存储内容加入活跃可存储对象表中（同时从缓存或者内存存档中读取该对象可存储内容），通常用于加载场景时
         * 
         * Step1.将广播的ISaveable注册进活动可存储对象表内
         * Step2.将该可存储内容从缓存中或存档中读取
         * 
         */
        //Debug.Log(_saveable.GetISaveID() + "注册了");
        activitySaveableObjects.Add(_saveable);
        string saveableID = _saveable.GetISaveID();

        if (saveableObjectsCacheData.ContainsKey(saveableID))
        {
            //Debug.Log("从缓存中读取" + saveableID);
            _saveable.LoadSpecificData(saveableObjectsCacheData[saveableID]);
        }
        else
        {
            //Debug.Log("从内存存档中读取" + _saveable.GetISaveID());
            _saveable.LoadSpecificData(saveableObjectsSaveData[saveableID]);
            saveableObjectsCacheData.Add(saveableID, saveableObjectsSaveData[saveableID]);
        }
    }
    private void ISaveableUnregister(ISaveable _saveable)
    {
        /* 这个方法用于将可存储内容对象从活动可存储对象表中注销（此时应该将需要注销的对象可存储内容放入缓存中），通常用于场景卸载时
         * 
         * Step1.将该存储内容同步到缓存
         * Step2.从活动可存储对象表内注销该广播的ISaveable
         * 
         */
        string saveableID = _saveable.GetISaveID();
        if (saveableObjectsCacheData.ContainsKey(saveableID))
        {
            saveableObjectsCacheData[saveableID] = _saveable.SaveDatalizeSpecificData();
        }
        activitySaveableObjects.Remove(_saveable);
        //Debug.Log(_saveable.GetISaveID()+"注销了");
    }
    private void SaveToCacheByJson()
    {
        //当缓存不为空时，修改当前可存储内容至缓存，缓存为空，从内存存档中读取至缓存中
        //若缓存对应条目为空，则添加，若否，则修改
        foreach (ISaveable _saveableObject in activitySaveableObjects)
        {
            _saveableObject.SaveDataSync();
            Debug.Log(_saveableObject.GetISaveID() + "已保存");
            saveableObjectsCacheData[_saveableObject.GetISaveID()] = _saveableObject.SaveDatalizeSpecificData();
        }
    }
    private SaveData GetSaveData(string _saveableID)
    {
        return saveableObjectsSaveData[_saveableID];
    }




    public void SetSaveDataRelationsByPlayerPrefs()
    {
        /* 
         * 
         * Step1.玩家偏好设置上次游玩的存档文件名
         * Step2.根据当前游戏保存保存存档总览结构体
         * Step3.根据当前存档路径确定需要存入的PlayPrefs的位置Key
         * Step4.在相应位置存入存档总览的Json文件
         * 
         */
        PlayerPrefs.SetString(dataConfig.FILEPATHSTR, currentSaveFliePath);
        SaveDataOverview saveDataOverview = new()
        {
            isValid = true,
            playDuration = (int) theUI.totalSeconds,
            saveSceneName = theLevel.lastSaveScene,
            gameHasFinished = theLevel.gameHasFinished,
            gameCompleteDegree = theLevel.gameCompleteDegree,
            attackAbility = thePlayer.attackAbility,
            umbrellaAbility = thePlayer.umbrellaAbility,
            candleAbility = thePlayer.candleAbility,


        };
        string currentSaveDataOverview;
        if (currentSaveFliePath == dataConfig.FILEPATH1STR)
        {
            currentSaveDataOverview = dataConfig.OVERVIEW1STR;
        }
        else if (currentSaveFliePath == dataConfig.FILEPATH2STR)
        {
            currentSaveDataOverview = dataConfig.OVERVIEW2STR;
        }
        else
        {
            currentSaveDataOverview = dataConfig.OVERVIEW3STR;
        }
        PlayerPrefs.SetString(currentSaveDataOverview, JsonUtility.ToJson(saveDataOverview));
    }
    public void SaveByJson()
    {
        /* 这个方法用于将当前可存储内容写入外村存档（此时需要将缓存清空，并将当前场景内活动对象可存储内容写入缓存），适用于调用存储交互时
         * 
         * Step1.将当前活动可存储内容对象的可存储内容放入缓存中
         * Step2.将用于缓存中所有内容均写入内存存档中
         * Step3.将SaveData转换DictionaryWrapper
         * Step4.将DictionaryWrapper文件转换为Json文件并写入外存存档
         * Step5.将缓存清空并重新将场景内所有活动可存储内容对象重新从内存存档中写入
         * 
         */
        SaveToCacheByJson();

        foreach (string _saveableObject in saveableObjectsCacheData.Keys)
        {
            saveableObjectsSaveData[_saveableObject] = saveableObjectsCacheData[_saveableObject];
        }

        List<SaveData> theitems = new();
        foreach (string _saveableObjectSaveData in saveableObjectsSaveData.Keys)
        {
            theitems.Add(saveableObjectsSaveData[_saveableObjectSaveData]);
        }
        DictionaryWrapper theDictWrapper = new()
        {
            items = theitems
        };

        File.WriteAllText(Path.Combine(currentSaveFolderPath, currentSaveFliePath + ".json"), JsonUtility.ToJson(theDictWrapper));
        //Debug.Log(Path.Combine(currentSaveFolderPath, currentSaveFliePath + ".json")+"保存成功");

        CacheDataOriginSet();
        SaveToCacheByJson();

    }//TODO:会被上面一个方法给包括


    public void CacheDataOriginSet()
    {
        saveableObjectsCacheData.Clear();
        SaveToCacheByJson();
    }

    #endregion

}
