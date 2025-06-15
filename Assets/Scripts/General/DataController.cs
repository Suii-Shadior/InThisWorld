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
    #region ���
    private ControllerManager thisCM;
    private EventController theEC;
    private UIController theUI;
    private LevelController theLevel;
    private NewPlayerController thePlayer;
    public DataConfig dataConfig;
    #endregion

    /* �浵ϵͳ��νṹ��
     * �ⲿ�ĵ� ==��saveableObjectsSaveData/�ڴ�浵 ==��saveableObjectsCacheData/���� ==��activitySaveableObjects
     * 
     * ǰ��������
     * 1.���ű�ά����νṹ�г��ⲿ�ĵ�������������ݣ��������������кܶ���߿ռ䣬��ʵ��ʵ��Ŀ��Ϊ����
     * 2.���ű�����Ϸ����ʱ����Ϸ�ر�ʱ��һֱ������Ψһ
     * 3.��Ϸ�����а����ɴ洢���ݵĶ��󶼹�����ISaveable�ӿ�
     * 
     * ʵ��˼·��
     * 1.�ڽű�����֮�������ⲿ�ĵ������д浵����д��saveableObjectsSaveData
     * 2.�ڳ�������ʱ�����س��������е�ISaveable�ӿڶ���ע���activitySaveableObjects��ͬʱ��ѯ�Ƿ����ڻ����д��ڣ����ǣ���ȡ��ֵ�����ո�ֵ��ʼ�������񣬶�ȡ�ڴ�浵ֵ�����������������
     * 3.�ڳ���ж��ʱ����ж�س����ڵ�����ISaveable�ӿڶ����activitySaveableObjectsע����ͬʱ����Ӧ�ɴ洢����д��saveableObjectsCacheData
     * 4.��ʵ�ֱ���ʱ���Ƚ���ǰ�������пɴ洢����д�뻺�棬�ٽ��������пɴ洢����д���ڴ�浵��Ȼ���ڴ�浵д���ⲿ�ĵ���ͬʱ��������գ����½���ɴ洢���ݶ����Ӧ�ɴ洢���ݴ��ڴ�浵��ȡ��д�뻺��
     * 
     */


    #region ����
    [Header("Cache Related")]
    [Tooltip("ʹ��List<ISaveable>��ԭ��\n" +
        "1.��С�Ǹ����ģ������޷�ʵ��\n" +
        "2.�����л����ܹ���Inspectorֱ�۵�չʾ��ǰ�����ݣ��ֵ��޷�ʵ��\n" +
        "3.��д���ֵ��ʱ����Ҫ����ֱ��ͨ���б�Ԫ�ػ�ȡ��浵���ݵķ�����ISaveableID�޷�ʵ��")]
    public List<ISaveable> activitySaveableObjects;
    [Tooltip("ʹ��Dictionary<string, string>��ԭ��\n" +
        "1.�ֵ����ʱ�临�Ӷ�O��1����List�޷�ʵ��\n" +
        "2.�ڳ���ж�������ʱ����ʹ��ͬһ�������ISaveable��Ӧ��InstanceIDҲ��һ���������޷�ʹ��ISaveable��Ϊ��\n" +
        "3.���浵������Ϣ��װ����SaveData�У���ֻʹ�þ���ɴ洢���ݵ�Jsonstring��������һ����Ч�ʣ������������")]
    private Dictionary<string, SaveData> saveableObjectsCacheData;
    [Header("SaveFile Related")]
    private string currentSaveFliePath;
    private string currentSaveDataOverview;
    public bool RelativeAddressMode;
    [Tooltip("ʹ��Dictionary<string, string>��ԭ��\n" +
        "1.�ֵ����ʱ�临�Ӷ�O��1����List�޷�ʵ��\n" +
        "2.�ò㲻���ٻ�ȡ�浵���ݣ���ISaveableID���Ը��ӷ����ʵ��ȫ���ڶ������,����ȫ������ļ��\n" +
        "3.���浵������Ϣ��װ����SaveData�У���ֻʹ�þ���ɴ洢���ݵ�Jsonstring��������һ����Ч�ʣ������������")]
    private Dictionary<string, SaveData> saveableObjectsSaveData;
    private DictionaryWrapper theSaveDataPackage;
    #endregion
    #region ����



    //private const string ORIGINALSCENENAMESTR = "SideA_P0-R1";
    #endregion


    private void Awake()
    {
        /*
         * 
         * Work1.��ȡ���������ע��EC��Ӧ�¼�
         * Work2.��PlayerPrefs�л�ȡ���ƫ�ã��Լ��ϴα���Ĵ浵��ţ����ޣ�������û�д浵���򴴽���������浵��
         * Work3.��������ƫ���������ݴ��ݸ����Controller
         * Work4.��ȡ�浵�ļ���·�������������ڻ��
         * Work5.��ʼ����Ծ�ɴ洢���ݶ�������ӻ����浵�ж�ȡ�������
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



    #region С�������ⲿ����
    private void GetSaveDataIndexFromPlayerPrefs()
    {
        //����ѡ��浵λ���ڶ��ʺϴ���PlayerPrefs�е�����,��������Ϸ�������ʱ
        if (thisCM.isTestMode)
        {
            currentSaveFliePath= dataConfig.TESTSCENESTR;
        }
        else
        {

            if (PlayerPrefs.HasKey(dataConfig.FILEPATHSTR))
            {
                currentSaveFliePath = PlayerPrefs.GetString(dataConfig.FILEPATHSTR);
            }
            else
            {
                currentSaveFliePath = dataConfig.FILEPATH1STR;
            }
        }

    }
    public void SaveDataCheck()
    {
        if (thisCM.isTestMode)
        {

        }
        else
        {
            if (File.Exists(Path.Combine(dataConfig.SAVEFOLDERPATHSTR, dataConfig.FILEPATH1STR + ".json")))
            {
                string saveData1Overview = PlayerPrefs.GetString(dataConfig.OVERVIEW1STR);
                //Debug.Log(saveData1Overview);
                SaveDataOverview overview1 = JsonUtility.FromJson<SaveDataOverview>(saveData1Overview);
                theUI.saveDataOverviews.Add(overview1);
                //theUI.saveDataOverviews.Add(JsonUtility.FromJson<SaveDataOverview>(saveData1Overview));

            }
            else
            {
                theUI.saveDataOverviews.Add(new SaveDataOverview());
            }
            if (File.Exists(Path.Combine(dataConfig.SAVEFOLDERPATHSTR, dataConfig.FILEPATH2STR + ".json")))
            {
                string saveData2Overview = PlayerPrefs.GetString(dataConfig.OVERVIEW2STR);
                theUI.saveDataOverviews.Add(JsonUtility.FromJson<SaveDataOverview>(saveData2Overview));
            }
            else
            {
                theUI.saveDataOverviews.Add(new SaveDataOverview());
            }
            if (File.Exists(Path.Combine(dataConfig.SAVEFOLDERPATHSTR, dataConfig.FILEPATH3STR + ".json")))
            {
                string saveData2Overview = PlayerPrefs.GetString(dataConfig.OVERVIEW3STR);
                theUI.saveDataOverviews.Add(JsonUtility.FromJson<SaveDataOverview>(saveData2Overview));
            }
            else
            {
                theUI.saveDataOverviews.Add(new SaveDataOverview());
            }

        }
    }
    private void SendPlayerPrefsToUI()
    {
        /* ���������ڽ���Controller��Ҫ�����ƫ�������ڴ˴�ͳһ���ã���������Ϸ����ʱ
         * 
         * ���ڸ���Controller�ű������м�����Ҫ���ǵ����߹���ʹ���˳����ܴ���Ǳ�ڵ�����
         * 
         */
        theUI.GetPlayerPrefsFromData(PlayerPrefs.GetInt(dataConfig.HADSTARTGAMESTR) == 1);
    }
    private void LoadDictionaryFromJson()
    {
        /* ����������ڴ����浵����д���ڴ�浵����������Ϸ����֮��
         * 
         * Step1.�ж�·���Ƿ�����ļ��������ڶ�ȡJson�ļ������رմ��ļ�
         * Step2.��Json�ļ���ȡȡΪDictionaryWrapper��������ṹд���ڴ�浵��
         * Step3.����ȡ������Ӧ�ļ���������
         * 
         * 
         * 
         */
        //
        if (File.Exists(Path.Combine(dataConfig.SAVEFOLDERPATHSTR, currentSaveFliePath + ".json")))
        {
            Debug.Log("�浵�����ҿ�ʼ��ȡ");
            StreamReader theSR = new(Path.Combine(dataConfig.SAVEFOLDERPATHSTR, currentSaveFliePath + ".json"));
            string json = theSR.ReadToEnd();
            theSR.Close();

            theSaveDataPackage = JsonUtility.FromJson<DictionaryWrapper>(json);
            foreach(SaveData _saavedata in theSaveDataPackage.items)
            {
                saveableObjectsSaveData.Add(_saavedata.key, _saavedata);
                //Debug.Log(_saavedata.key + "�����ѷ����ڴ�浵");
            }

            theSaveDataPackage = null;
        }
        else
        {
            //saveableObjectsSaveData = oringinaldata;
            Debug.Log(currentSaveFliePath+"�浵������");
        }
    }
    private void ISaveableRegister(ISaveable _saveable)
    {
        /* ����������ڽ��ɴ洢���ݼ����Ծ�ɴ洢������У�ͬʱ�ӻ�������ڴ�浵�ж�ȡ�ö���ɴ洢���ݣ���ͨ�����ڼ��س���ʱ
         * 
         * Step1.���㲥��ISaveableע�����ɴ洢�������
         * Step2.���ÿɴ洢���ݴӻ����л�浵�ж�ȡ
         * 
         */
        //Debug.Log(_saveable.GetISaveID() + "ע����");
        activitySaveableObjects.Add(_saveable);
        string saveableID = _saveable.GetISaveID();

        if (saveableObjectsCacheData.ContainsKey(saveableID))
        {
            //Debug.Log("�ӻ����ж�ȡ" + saveableID);
            _saveable.LoadSpecificData(saveableObjectsCacheData[saveableID]);
        }
        else
        {
            //Debug.Log("���ڴ�浵�ж�ȡ" + _saveable.GetISaveID());
            _saveable.LoadSpecificData(saveableObjectsSaveData[saveableID]);
            saveableObjectsCacheData.Add(saveableID, saveableObjectsSaveData[saveableID]);
        }
    }
    private void ISaveableUnregister(ISaveable _saveable)
    {
        /* ����������ڽ��ɴ洢���ݶ���ӻ�ɴ洢�������ע������ʱӦ�ý���Ҫע���Ķ���ɴ洢���ݷ��뻺���У���ͨ�����ڳ���ж��ʱ
         * 
         * Step1.���ô洢����ͬ��������
         * Step2.�ӻ�ɴ洢�������ע���ù㲥��ISaveable
         * 
         */
        string saveableID = _saveable.GetISaveID();
        if (saveableObjectsCacheData.ContainsKey(saveableID))
        {
            saveableObjectsCacheData[saveableID] = _saveable.SaveDatalizeSpecificData();
        }
        activitySaveableObjects.Remove(_saveable);
        //Debug.Log(_saveable.GetISaveID()+"ע����");
    }
    private void SaveToCacheByJson()
    {
        //�����治Ϊ��ʱ���޸ĵ�ǰ�ɴ洢���������棬����Ϊ�գ����ڴ�浵�ж�ȡ��������
        //�������Ӧ��ĿΪ�գ�����ӣ��������޸�
        foreach (ISaveable _saveableObject in activitySaveableObjects)
        {
            _saveableObject.SaveDataSync();
            Debug.Log(_saveableObject.GetISaveID() + "�ѱ���");
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
         * Step1.���ƫ�������ϴ�����Ĵ浵�ļ���
         * Step2.���ݵ�ǰ��Ϸ���汣��浵�����ṹ��
         * Step3.���ݵ�ǰ�浵·��ȷ����Ҫ�����PlayPrefs��λ��Key
         * Step4.����Ӧλ�ô���浵������Json�ļ�
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
        /* ����������ڽ���ǰ�ɴ洢����д�����浵����ʱ��Ҫ��������գ�������ǰ�����ڻ����ɴ洢����д�뻺�棩�������ڵ��ô洢����ʱ
         * 
         * Step1.����ǰ��ɴ洢���ݶ���Ŀɴ洢���ݷ��뻺����
         * Step2.�����ڻ������������ݾ�д���ڴ�浵��
         * Step3.��SaveDataת��DictionaryWrapper
         * Step4.��DictionaryWrapper�ļ�ת��ΪJson�ļ���д�����浵
         * Step5.��������ղ����½����������л�ɴ洢���ݶ������´��ڴ�浵��д��
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

        File.WriteAllText(Path.Combine(dataConfig.SAVEFOLDERPATHSTR, currentSaveFliePath + ".json"), JsonUtility.ToJson(theDictWrapper));
        //Debug.Log(Path.Combine(currentSaveFolderPath, currentSaveFliePath + ".json")+"����ɹ�");

        CacheDataOriginSet();
        SaveToCacheByJson();

    }//TODO:�ᱻ����һ������������


    public void CacheDataOriginSet()
    {
        saveableObjectsCacheData.Clear();
        SaveToCacheByJson();
    }

    #endregion

}
