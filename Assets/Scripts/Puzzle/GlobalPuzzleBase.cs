using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GlobalPuzzleBase : PuzzleBase,ISave<GlobalPuzzleSaveData>
{
    public GlobalPuzzleSaveData thisGlobalPuzzleSaveData;
    [Header("Global Related")]
    public string puzzleID;
    public bool isSolved;
    public string[] globalPuzzleStrings;

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        ISaveable saveable = GetComponent<ISaveable>();
        RegisterSaveable(saveable);
        puzzleID = thisGlobalPuzzleSaveData.SaveableID;
        isSolved = thisGlobalPuzzleSaveData.isSolved;

    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
    public abstract void SceneLoad();
    public abstract void ThisPuzzleUpdate();
    public abstract void ResetPuzzle();

    public abstract void FinishsetPuzzle();

    public abstract bool CheckComplete();

    public abstract void ActivatePuzzle();
    public abstract void UnactivatePuzzle();



    #region ISave接口相关
    public GlobalPuzzleSaveData GetSpecificDataByISaveable(SaveData _passedData)
    {
        GlobalPuzzleSaveData thedata = JsonUtility.FromJson<GlobalPuzzleSaveData>(_passedData.value);
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
        thisGlobalPuzzleSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
    }

    public SaveData SaveDatalizeSpecificData()
    {
        SaveData savedata = new()
        {
            key = thisGlobalPuzzleSaveData.SaveableID,
            type = TypeRegistry.GetDataTypeFromReverseDict(typeof(GlobalPuzzleSaveData)),
            value = JsonUtility.ToJson(thisGlobalPuzzleSaveData)
        };
        return savedata;

    }

    public void SaveDataSync()
    {
        thisGlobalPuzzleSaveData.SaveableID = puzzleID;
        thisGlobalPuzzleSaveData.isSolved = isSolved;
    }
    #endregion
}
