using CMRelatedEnum;
using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour, ISave<LevelSaveData>
{
    #region ���������
    private ControllerManager thisCM;
    private NewPlayerController thePlayer;
    private UIController theUI;
    //private DialogeController theDC;
    private AudioController theBGMPlayer;
    private EventController theEC;
    private DataController theData;
    [SerializeField]private LevelSaveData thisLevelSaveData;
    public SceneConnectionManager connectionManager;
    #endregion

    #region ����
    
    [Header("WorkState")]
    private bool isLevelLoading;
    public bool isPausing;
    private float tempTimeSpeed;

    [Header("Respawn Related")]
    public Transform resetPos;
    [Header("Overview Related")]
    private string currentSceneName;
    public List<string> loadedScenes = new List<string>();
    public string lastSaveScene;
    public bool gameHasFinished;
    public float gameCompleteDegree;
    #endregion

    #region ����
    private const string LEVELSAVEDATAIDSTR = "LevelSaveData";
    private const string PERSISTENTGAMEPLAYOBJECTSSTR = "PersistentGameplayObjects";
    private const string ORIGINALSCENESTR = "SideA_P0-R1";
    
    public const string SAVEDATASCENESTR = "SaveData_LastSaveScaneName";
    #endregion



    private void Awake()
    {
        /* 
         * Work1.�����ȡ
         * 
         * 
         * 
         */
        thisCM = GetComponentInParent<ControllerManager>();
        thePlayer = thisCM.thePlayer;
        theUI = thisCM.theUI;
        theBGMPlayer = thisCM.theBGMPlayer;
        theEC = thisCM.theEvent;
        theData = thisCM.theData;
        //theDC = thisCM.theDC;

    }


    private void OnEnable()
    {
        /*
         * 
         * Work1.�������
         * Work2.ע���Ծ�ɴ洢�����ڴ�浵�л�ȡ�ؿ��浵����
         * Work3.�ű�����ͬ���浵���ݣ����ض�Ӧ�ؿ�����
         * 
         */

        //thisLevelSaveData.CopyData(GetSpecificDataByISaveable(theData.GetSaveData(LEVELSAVEDATAIDSTR)));
        //ISaveable saveable = GetComponent<ISaveable>();
        //RegisterSaveable(saveable);
        RegisterSaveable(GetComponent<ISaveable>());
        
        SceneLoadFromLevelData();


    }



    private void Start()
    {

    }
    private void Update()
    {


    }
    #region ��ʼ�����
    private void SceneLoadFromLevelData()
    {
        /* ����������
         * SceneLoad_CorresponceWithSaveData
         * Step1.���ű���������SaveDataͬ��
         * Step2.���йؿ���������
         * 
         * 
         */
        SceneLoad_CorresponceWithSaveData();
        //Debug.Log(currentSceneName);
        StartCoroutine(GameStartSceneLoadingCo(currentSceneName));
        //Debug.Log("���ؿ�");
    }
   private void SceneLoad_CorresponceWithSaveData()
    {
        lastSaveScene = thisLevelSaveData.lastSaveScene;
        currentSceneName = thisLevelSaveData.lastSaveScene;
    }
    #endregion


    #region С�������ⲿ����



    public bool GetLevelLoading()
    {
        return isLevelLoading;
    }
    public void SetCurrentSceneName(string _movingToSceneName)
    {
        currentSceneName = _movingToSceneName;
    }
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }

    #region �ֿ�������
    public void SceneLoadTriggered(string _newCurSceneName)
    {
        //�����������ⲿ���ùؿ������ֿ���أ�������
        StartCoroutine(GamePlaySceneLoadingCo(_newCurSceneName));
    }
    private IEnumerator GamePlaySceneLoadingCo(string _newCurSceneName)
    {
        /* ��Я��������Ϸ����ʱ�л�����ʱ���շֿ����ʵ�ֹؿ������ļ��غ�ж��
         * 
         * Step1.��ȡ��ǰ�����ͼ������س����ĸ������ڳ���������������жԱ�
         * Step2.������Ҫ��ж�غ���Ҫ�����صĳ���
         * Step2-a.��Ҫ��ж�صĳ���=�ɵ����ڳ���-�µ����ڳ���-�������صĳ���
         * Step2-b.��Ҫ���صĳ���=�µ����ڳ���+�������س���-�Ѽ��س���
         * Step3.���غ�ж��Step2�Ľ��
         * 
         *
         */
        isLevelLoading = true;
        //
        List<string> oldNeighbors = connectionManager.GetNeighbors(currentSceneName);
        List<string> newNeighbors = connectionManager.GetNeighbors(_newCurSceneName);
        //Step2.����Ҫ������صĳ�������Ҫ��ж�صĳ���
        List<string> scenesToUnload = new();
        foreach (string _scene in loadedScenes)
        {
            if (!newNeighbors.Contains(_scene) && _scene != _newCurSceneName)
            {
                scenesToUnload.Add(_scene);
            }
        }
        List<string> scenesToLoad = new(newNeighbors);

        scenesToLoad.RemoveAll(s => loadedScenes.Contains(s));
        scenesToLoad.Add(_newCurSceneName);

        foreach (var scene in scenesToUnload)
        {
            if (SceneManager.GetSceneByName(scene).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
                loadedScenes.Remove(scene);
            }
        }
        foreach (string sceneName in scenesToLoad)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                loadedScenes.Add(sceneName);
            }
        }
        isLevelLoading = false;
    }
    private IEnumerator GameStartSceneLoadingCo(string _originalSceneName)
    {

        /* ��Я��������Ϸ����ʱ��ʼ���س���ʱ���շֿ����ʵ�ֹؿ������ļ���
         * 
         * Step1.��ȡ�浵���ڳ��������ڳ���
         * Step2.���������������ڳ��������ڳ���
         * Step3.����UI����Ч��
         */
        isLevelLoading = true;
        List<string> scenesToLoad = new();
        List<string> neighbors = connectionManager.GetNeighbors(_originalSceneName);

        scenesToLoad.Add(_originalSceneName);
        foreach (string neighbor in neighbors)
        {
            scenesToLoad.Add(neighbor);
        }
        foreach (string sceneName in scenesToLoad)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                loadedScenes.Add(sceneName);
            }
        }

        theUI.StartCoroutine(theUI.BlackScreen_FadeOutCo());
        isLevelLoading = false;

    }


    private IEnumerator GameReloadSceneLoadingCo(string _reloadSceneName)
    {
        /* ��Я��������ganmepauseʱ�ص���ҳ��
         * 
         * Step1.��Ļ����
         * Step2.����󱣴泡����ΪĿ�곡�����зֿ����
         * Step3.��ջ��棬���¶�ȡ��Ծ�ɴ洢������ڴ�浵
         * Step4.��������������ҳ�ʼλ�ú�״̬
         * Step5.UI�������
         * ��ͷ����
         * Step5.��ĵ����
         * 
         */
        isLevelLoading = true;
        
        yield return StartCoroutine(theUI.BlackScreen_FadeInCo());
        //Time.timeScale = 0f;

        List<string> oldNeighbors = connectionManager.GetNeighbors(currentSceneName);
        List<string> newNeighbors = connectionManager.GetNeighbors(_reloadSceneName);
        SetCurrentSceneName(_reloadSceneName);
        List<string> scenesToUnload = new();
        foreach (string _scene in loadedScenes)
        {
            if (!newNeighbors.Contains(_scene) && _scene != _reloadSceneName)
            {
                scenesToUnload.Add(_scene);
            }
        }
        List<string> scenesToLoad = new(newNeighbors);

        scenesToLoad.RemoveAll(s => loadedScenes.Contains(s));
        scenesToLoad.Add(_reloadSceneName);

        foreach (var scene in scenesToUnload)
        {
            if (SceneManager.GetSceneByName(scene).isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
                loadedScenes.Remove(scene);
            }
        }
        foreach (string sceneName in scenesToLoad)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                loadedScenes.Add(sceneName);
            }
        }
        isLevelLoading = false;

        theData.CacheDataOriginSet();


        thePlayer.PlayerOriginalSetting();
        
        Time.timeScale = 1f;
        theUI.UIObjectOriginalSetting();
        yield return null;
        yield return StartCoroutine(theUI.BlackScreen_FadeOutCo());
    }




    #endregion
    #region ResetPoint���
    public void SetResetPos(Transform _thisResetPos)
    {
        //Debug.Log("����������");
        resetPos = _thisResetPos;
    }


    #endregion
    #region UI���
    public void GamePlay_PauseTime()
    {
        //����������������ʱ��ͣ����ʱ����ͣ��������UI����
        tempTimeSpeed = Time.timeScale;
        Time.timeScale = 0;

    }
    public void GamePause_ResumeTime()
    {
        //��������������ͣʱ��������
        Time.timeScale = tempTimeSpeed;
    }




    public void BackToMainMenu()
    {
        StartCoroutine(GameReloadSceneLoadingCo(lastSaveScene));
    }
    #endregion

    #region �������������
    public void ResetPlayer_Respawn()
    {
        //�����������������λ�������洢�㣬���������� 
        StartCoroutine(ResetPlayer_RespawnCo());
    }

    private IEnumerator ResetPlayer_RespawnCo()
    {

        /* ��Я��������������������������λ��
         * 
         * Step1.����������������Ч
         * Step2.UI����
         * Step3.�������ı��泡��
         * Step4.����ҷ�����󱣴�λ�ã�ͬʱ������Ҿ���״̬��Ѫ��
         * Step5.���������Ѿ����صĿɱ����ö���
         * Step6.UI����
         * 
         * 
         */
        yield return new WaitForSeconds(2f);
        //theUI.BlackScreen_FadeOut();
        yield return new WaitForSeconds(theUI.UIConfig.blackScreenFadeOutDuration);


        StartCoroutine(GamePlaySceneLoadingCo(lastSaveScene));
        //


        thePlayer.InteractRelated_SaveItem(thePlayer.lastSavePos);




        yield return new WaitForSeconds(1f);
        //theUI.BlackScreen_FadeIn();
        yield return new WaitForSeconds(theUI.UIConfig.blackScreenFadeInDuration);
    }

    public void ResetPlayer_UnKnockable()
    {
        //�����������������λ���������ʱ�洢�㣬���������˵��ǲ��ܼ򵥻���
        StartCoroutine(ResetPlayer_UnKnockableCo());
    }

    private IEnumerator ResetPlayer_UnKnockableCo()
    {
        /*  ��Я����������ض�������������λ��
         * 
         * Step1.UI����
         * Step2.����ҷ�����󱣴�λ�ã�״̬
         * Step3.���������Ѿ����صĿɱ����ö���
         * Step3.UI����
         */
        yield return new WaitForSeconds(2f);
        //theUI.BlackScreen_FadeOut();
        yield return new WaitForSeconds(1 / theUI.fadeOutRatio);
 
        thePlayer.ResetRelated_UnKnockable(resetPos.position);




        yield return new WaitForSeconds(1f);
        //theUI.BlackScreen_FadeIn();
        yield return new WaitForSeconds(1 / theUI.fadeInRatio);
    }
    #endregion

    #region �ӿ�ʵ��


    public string GetISaveID()
    {
        return thisLevelSaveData.levelSaveDataID;
    }

    public void RegisterSaveable(ISaveable _saveable)
    {
        theEC.SaveableRegisterPublish(_saveable);
    }

    public void UnregisterSaveable(ISaveable _saveable)
    {
        theEC.SaveableUnregisterPublish(_saveable);
    }

    public void LoadSpecificData(SaveData _passingData)//�����ڱ�ISaveableע��ʱ��DataController���ã��ӻ�����ڴ�浵�л��SaveData
    {
        thisLevelSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
    }
    public void SaveDataSync()
    {
        thisLevelSaveData.lastSaveScene = currentSceneName;
        //Debug.Log("LevelSaveData����");
    }

    public SaveData SaveDatalizeSpecificData()
    {
        SaveData _savedata = new()
        {
            key = thisLevelSaveData.levelSaveDataID,
            type = TypeRegistry.GetDataTypeFromReverseDict(typeof(LevelSaveData)),
            value = JsonUtility.ToJson(thisLevelSaveData)
        };
        return _savedata;
    }
    public LevelSaveData GetSpecificDataByISaveable(SaveData _passedData)
    {
        LevelSaveData thedata = JsonUtility.FromJson<LevelSaveData>(_passedData.value);
        //Debug.Log(thedata.isOpened);
        return thedata;
    }

    #endregion
    #endregion
}
