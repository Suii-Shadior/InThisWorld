
using UnityEngine;

[CreateAssetMenu(fileName = "DataConfig", menuName = "Configs/Data Config")]

public class DataConfig : ScriptableObject
{
    [Header("Const Related")]
    [HideInInspector] public string FILEPATHSTR = "LastGamePlayInfo_FilePath";
    [HideInInspector] public string FILEPATH1STR = "SaveData1";
    [HideInInspector] public string FILEPATH2STR = "SaveData2";
    [HideInInspector] public string FILEPATH3STR = "SaveData3";
    [HideInInspector] public string OVERVIEW1STR = "SaveData1Overview";
    [HideInInspector] public string OVERVIEW2STR = "SaveData2Overview";
    [HideInInspector] public string OVERVIEW3STR = "SaveData3Overview";

    [HideInInspector] public string SAVEFOLDERPATHSTR = "C:\\Users\\Administrator\\AppData\\LocalLow\\SeiDoge\\InThisWorld\\SaveData";//Ä¬ÈÏÎªAssets/SaveData/
    [HideInInspector] public string SAVEFOLDERPATHLOCALSTR = "Assets\\SaveData";

    [HideInInspector] public string HADSTARTGAMESTR = "HadPlayedGame";

}
