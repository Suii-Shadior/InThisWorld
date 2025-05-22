using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventController;

public abstract class LocalPuzzleBase : PuzzleBase,ISave<LocalPuzzleSaveData>
{

    /*
     局部谜题构筑思路：
    1、再OnEnable中注册ISaveable,从缓存或者内存存档中读取可存储内容。根据存档内容注册谜题，初始设置SceneLoad
    2、在

     */
    public LocalPuzzleSaveData thisLocalPuzzleSaveData;
    [Header("Local Related")]
    public string puzzleID;
    public bool isSolved;
    [Header("Setting")]
    public bool canBeActivated;
    public bool isActive;
    public string localSubscriberChannel_ActivatePuzzle;
    public string localSubscriberChannel_UnactivatePuzzle;
    public string localPublisherChannel_FinishPuzzle;


    protected override void Awake()
    {
        base.Awake();

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        RegisterSaveable(GetComponent<ISaveable>());
        puzzleID = thisLocalPuzzleSaveData.SaveableID;
        isSolved = thisLocalPuzzleSaveData.isSolved;
        if (isSolved)
        {

        }
        else
        {
            RegisterLocalPuzzle(this);
            if (canBeActivated)
            {
                theEC.OnLocalEvent += OnLocalEventer;
            }
            else
            {

            }
        }

    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (isSolved)
        {

        }
        else
        {
            UnregisterLocalPuzzle(this);
        }
    }
    public abstract void SceneLoad_Itself();
    public abstract void SceneLoad_Relative();
    public abstract void SceneLoad_Related();
    public abstract void ThisPuzzleUpdate();

    public abstract bool CheckComplete();
    public abstract void FinishPuzzle();
    public abstract void ActivatePuzzle();
    public abstract void UnactivatePuzzle();
    public abstract void OnLocalEventer(string _localEventerChannel);
    public void RegisterLocalPuzzle(LocalPuzzleBase _localPuzzle)
    {
        theEC.LoaclPuzzleRegisterPublish(_localPuzzle);
    }
    public void UnregisterLocalPuzzle(LocalPuzzleBase _localPuzzle)
    {
        theEC.LocalPuzzleUnregisterPublish(_localPuzzle);
    }
    #region ISave接口相关

    public LocalPuzzleSaveData GetSpecificDataByISaveable(SaveData _passedData)
    {
        LocalPuzzleSaveData thedata = JsonUtility.FromJson<LocalPuzzleSaveData>(_passedData.value);
        return thedata;
    }

    public string GetISaveID()
    {
        return puzzleID;
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
        thisLocalPuzzleSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
    }

    public SaveData SaveDatalizeSpecificData()
    {
        SaveData _savedata = new()
        { 
            key = thisLocalPuzzleSaveData.SaveableID,
            type = TypeRegistry.GetDataTypeFromReverseDict(typeof(LocalPuzzleSaveData)),
            value = JsonUtility.ToJson(thisLocalPuzzleSaveData)
            
        };
        return _savedata;
    }

    public void SaveDataSync()
    {
        thisLocalPuzzleSaveData.SaveableID = puzzleID;
        thisLocalPuzzleSaveData.isSolved = isSolved;
    }
    #endregion
}
