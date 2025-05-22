using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static UnityEditor.LightingExplorerTableColumn;
using System;

namespace StructForSaveData
{
    [Flags]
    public enum DataTypes
    {
        Empty = 0,
        PlayerData = 1 << 0,
        LevelSaveData = 1 << 1,
        DoorSaveData = 1 << 2,
        OpenerSaveData = 1 << 3,
        BlockerSaveData = 1 << 4,
        NailerSaveData = 1 << 5,
        LocalPuzzleSaveData = 1 << 6,
        GlobalPuzzleSaveData = 1 << 7,
    }

    #region 结构体
    [System.Serializable]
    public struct PlayerSaveData
    {
        public string playerSaveDataID;
        public bool attackAblity;
        public bool umbrellaAbility;
        public bool candleAbility;
        public float lastSavePointX;
        public float lastSavePointY;
        public void CopyData(PlayerSaveData _playerSaveData)
        {
            this = _playerSaveData;
            //Debug.Log("复制了");
        }
        public Vector2 GetLastSavePoint()
        {
            return new Vector2(lastSavePointX, lastSavePointY);
        }

    }

    [System.Serializable]
    public struct DoorSaveData
    {
        public string SaveableID;
        public bool isOpened;
        public int needToOpen;

        public void CopyData(DoorSaveData _doorSaveData)
        {
            this = _doorSaveData;
            //Debug.Log("复制了");
        }

    }


    [System.Serializable]
    public struct OpenerSaveData
    {
        public string SaveableID;
        public bool isPressed;
        public void CopyData(OpenerSaveData _openSavedata)
        {
            this = _openSavedata;
        }
    }
    [System.Serializable]
    public struct LevelSaveData
    {
        public string levelSaveDataID;
        public string lastSaveScene;

        public void CopyData(LevelSaveData _levelSaveData)
        {
            this = _levelSaveData;
            //Debug.Log("复制了");
        }
        public string GetLastSaveScene()
        {
            return lastSaveScene;
        }

    }
    [System.Serializable]
    public struct BlockerSaveData
    {
        public string SaveableID;
        public bool isTriggered;
        public int needToTriggered;

        public void CopyData(BlockerSaveData _blockerSaveData)
        {
            this = _blockerSaveData;
            //Debug.Log("复制了");
        }
    }

    [System.Serializable]
    public struct NailerSaveData
    {
        public string SaveableID;
        public bool isTriggered;
        public void CopyData(NailerSaveData _nailerSaveData)
        {
            this = _nailerSaveData;
            //Debug.Log("复制了");
        }
    }
    [System.Serializable]
    public struct LocalPuzzleSaveData
    {
        public string SaveableID;
        public bool isSolved;
        public void CopyData(LocalPuzzleSaveData _localPuzzleSaveData)
        {
            this = _localPuzzleSaveData;
        }
    }
    [System.Serializable]
    public struct GlobalPuzzleSaveData
    {
        public string SaveableID;
        public bool isSolved;
        public void CopyData(GlobalPuzzleSaveData _globalPuzzleSaveData)
        {
            this = _globalPuzzleSaveData;
        }
    }
    #endregion

    public static class TypeRegistry
    {
        private static readonly Dictionary<DataTypes, System.Type> typeMap = new Dictionary<DataTypes, System.Type>
        {
            { DataTypes.PlayerData, typeof(PlayerSaveData) },
            { DataTypes.LevelSaveData,typeof(LevelSaveData) },
            { DataTypes.DoorSaveData, typeof(DoorSaveData) },
            { DataTypes.OpenerSaveData, typeof(OpenerSaveData) },
            { DataTypes.BlockerSaveData,typeof(BlockerSaveData) },
            { DataTypes.NailerSaveData,typeof(NailerSaveData) },
            { DataTypes.LocalPuzzleSaveData,typeof(LocalPuzzleSaveData) },
            { DataTypes.GlobalPuzzleSaveData,typeof(GlobalPuzzleSaveData) },
        };

        private static readonly Dictionary<System.Type, DataTypes> reverseTypeMap;

        // 静态构造函数初始化反向字典
        static TypeRegistry()
        {
            reverseTypeMap = new Dictionary<System.Type, DataTypes>();
            foreach (var pair in typeMap)
            {
                if (!reverseTypeMap.ContainsKey(pair.Value))
                {
                    reverseTypeMap.Add(pair.Value, pair.Key);
                }
                else
                {
                    Debug.LogError($"重复类型映射: {pair.Value}");
                }
            }
        }

        public static System.Type GetTypeFromDict(DataTypes _dataType)
        {
            return typeMap.TryGetValue(_dataType, out var _type) ? _type : null;
        }

        public static DataTypes GetDataTypeFromReverseDict(System.Type _type)
        {
            return reverseTypeMap.TryGetValue(_type, out var _dataType) ? _dataType : DataTypes.Empty;
        }

    }
    [System.Serializable]
    public class DictionaryWrapper
    {
        public List<SaveData> items = new List<SaveData>();
    }
    [System.Serializable]
    public struct SaveData
    {
        public string key;
        public StructForSaveData.DataTypes type;
        public string value;
    }










    public struct SaveDataOverview 
    {
        public bool isValid;
        public int playDuration;
        public string saveSceneName;
        
        public bool attackAbility;
        public bool umbrellaAbility;
        public bool candleAbility;

        public bool gameHasFinished;
        public float gameCompleteDegree;
    }
    [System.Serializable]
    public struct SaveDataSlotEntry
    {
        public TextMeshProUGUI playDurationText;
        public TextMeshProUGUI gameCompleteDegreeText;
        public Image[] skillIcons;
        public GameObject noSaveDataImage;
    }


}