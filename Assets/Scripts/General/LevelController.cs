using CMRelatedEnum;
using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour, ISave<LevelSaveData>
{
    #region 组件及其他
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

    #region 变量
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

    #region 常量
    private const string LEVELSAVEDATAIDSTR = "LevelSaveData";
    private const string PERSISTENTGAMEPLAYOBJECTSSTR = "PersistentGameplayObjects";
    private const string ORIGINALSCENESTR = "SideA_P0-R1";
    public const string SAVEDATASCENESTR = "SaveData_LastSaveScaneName";
    #endregion



    private void Awake()
    {
        /* 
         * Work1.组件获取
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
         * Work1.组件配置
         * Work2.注册活跃可存储表，从内存存档中获取关卡存档内容
         * Work3.脚本内容同步存档内容，加载对应关卡场景
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
    #region 初始化相关
    private void SceneLoadFromLevelData()
    {
        /* 本方法用于
         * SceneLoad_CorresponceWithSaveData
         * Step1.将脚本内数据与SaveData同步
         * Step2.进行关卡场景加载
         * 
         * 
         */
        SceneLoad_CorresponceWithSaveData();
        //Debug.Log(currentSceneName);
        StartCoroutine(GameStartSceneLoadingCo(currentSceneName));
        //Debug.Log("加载咯");
    }
   private void SceneLoad_CorresponceWithSaveData()
    {
        lastSaveScene = thisLevelSaveData.lastSaveScene;
        currentSceneName = thisLevelSaveData.lastSaveScene;
    }
    #endregion


    #region 小方法和外部调用



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

    #region 分块加载相关
    public void SceneLoadTriggered(string _newCurSceneName)
    {
        //本方法用于外部调用关卡场景分块加载，适用于
        StartCoroutine(GamePlaySceneLoadingCo(_newCurSceneName));
    }
    private IEnumerator GamePlaySceneLoadingCo(string _newCurSceneName)
    {
        /* 本携程用于游戏游玩时切换场景时按照分块加载实现关卡场景的加载和卸载
         * 
         * Step1.获取当前场景和即将加载场景的各自相邻场景，方便后续进行对比
         * Step2.计算需要被卸载和需要被加载的场景
         * Step2-a.需要被卸载的场景=旧的相邻场景-新的相邻场景-即将加载的场景
         * Step2-b.需要加载的场景=新的相邻场景+即将加载场景-已加载场景
         * Step3.加载和卸载Step2的结果
         * 
         *
         */
        isLevelLoading = true;
        //
        List<string> oldNeighbors = connectionManager.GetNeighbors(currentSceneName);
        List<string> newNeighbors = connectionManager.GetNeighbors(_newCurSceneName);
        //Step2.创建要另外加载的场景和需要被卸载的场景
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

        /* 本携程用于游戏启动时开始加载场景时按照分块加载实现关卡场景的加载
         * 
         * Step1.获取存档所在场景的相邻场景
         * Step2.加载所有所有相邻场景和所在场景
         * Step3.调用UI加载效果
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
        /* 本携程用于在ganmepause时回到主页面
         * 
         * Step1.黑幕淡入
         * Step2.以最后保存场景作为目标场景进行分块加载
         * Step3.清空缓存，重新读取活跃可存储对象表内存存档
         * Step4.根据重新设置玩家初始位置和状态
         * Step5.UI对象更新
         * 镜头控制
         * Step5.黑牡淡出
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
    #region ResetPoint相关
    public void SetResetPos(Transform _thisResetPos)
    {
        //Debug.Log("更新重生点");
        resetPos = _thisResetPos;
    }


    #endregion
    #region UI相关
    public void GamePlay_PauseTime()
    {
        //本方法用于在游玩时暂停运行时间暂停，适用于UI调用
        tempTimeSpeed = Time.timeScale;
        Time.timeScale = 0;

    }
    public void GamePause_ResumeTime()
    {
        //本方法用于在暂停时重新运行
        Time.timeScale = tempTimeSpeed;
    }




    public void BackToMainMenu()
    {
        StartCoroutine(GameReloadSceneLoadingCo(lastSaveScene));
    }
    #endregion

    #region 重生、重置相关
    public void ResetPlayer_Respawn()
    {
        //本方法用于重置玩家位置于最后存储点，适用于死亡 
        StartCoroutine(ResetPlayer_RespawnCo());
    }

    private IEnumerator ResetPlayer_RespawnCo()
    {

        /* 本携程用于玩家重生过程中重置玩家位置
         * 
         * Step1.播放死亡动画、声效
         * Step2.UI淡出
         * Step3.加载最后的保存场景
         * Step4.将玩家放在最后保存位置，同时重设玩家具体状态，血量
         * Step5.重置所有已经加载的可被重置对象
         * Step6.UI淡出
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
        //本方法用于重置玩家位置于最后临时存储点，适用于受伤但是不能简单击退
        StartCoroutine(ResetPlayer_UnKnockableCo());
    }

    private IEnumerator ResetPlayer_UnKnockableCo()
    {
        /*  本携程用于玩家特定情况下重置玩家位置
         * 
         * Step1.UI淡出
         * Step2.将玩家放在最后保存位置，状态
         * Step3.重置所有已经加载的可被重置对象
         * Step3.UI淡出
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

    #region 接口实现


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

    public void LoadSpecificData(SaveData _passingData)//用于在本ISaveable注册时从DataController调用，从缓存或内存存档中获得SaveData
    {
        thisLevelSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
    }
    public void SaveDataSync()
    {
        thisLevelSaveData.lastSaveScene = currentSceneName;
        //Debug.Log("LevelSaveData保存");
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
