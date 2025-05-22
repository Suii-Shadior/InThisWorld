using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{

    private EventController theEC;

    [Header("Local Related")]

    private List<LocalPuzzleBase> activeLocalPuzzles = new();
    private List<GlobalPuzzleBase> activeGlobalPuzzles = new();
    [Header("Global Related")]
    private Dictionary<string, string[]> globalPuzaleRefer = new Dictionary<string, string[]>(); //这里string均是存档内的Saveab代表其实全局谜题不会重复
    private Dictionary<string, int> globalPuzzleCheck = new Dictionary<string, int>();
    private Dictionary<string, string> globalFinishRefer = new Dictionary<string, string>();
    [Header("Save Related")]
    private const string PUZZLESTR = "Puzzle"; 

    private void Awake()
    {
        theEC = GetComponentInParent<ControllerManager>().theEvent;
        theEC.OnLocalPuzzleRegister += RegisterLocalPuzzle;
        theEC.OnLocalPuzzleUnregister += UnregisterLocalPuzzle;
        theEC.OnGlobalPuzzleRegister += RegisterGlobalPuzzle;

    }


    private void Start()
    {
        
        theEC.OnGlobalEvent += FinishGlobalPuzzle;

    }
    void Update()
    {
        foreach (LocalPuzzleBase activeLocakPuzzle in activeLocalPuzzles)// Step1.对所有活动状态下的谜题进行更新检测
        {
            activeLocakPuzzle.ThisPuzzleUpdate();


            //    if (activePuzzle.isLocalPuzzle)//Step2-1.进行是否是本地谜题对象判断
            //    {
            //        activePuzzle.ThisPuzzleUpdate();
            //    }
            //    if (activePuzzle.isGlobalPuzzle)//Step2-2.进行是否是全局谜题对象判断
            //    {
            //        foreach(string globalPuzzleString in activePuzzle.globalPuzzleStrings)//Step3.根据所有的全局谜题索引进行全局判断
            //        {
            //            if (GlobalPuzzleCheck(globalPuzzleString))
            //            {
            //                FinishGlobalPuzzle(globalPuzzleString);
            //            }

            //        }
            //    }
        }
    }
    #region 注册活动谜题
    public void RegisterLocalPuzzle(LocalPuzzleBase puzzle)
    {
        activeLocalPuzzles.Add(puzzle);
    }

    public void UnregisterLocalPuzzle(LocalPuzzleBase puzzle)
    {
        activeLocalPuzzles.Remove(puzzle);

    }
    public void RegisterGlobalPuzzle(GlobalPuzzleBase puzzle)
    {
        activeGlobalPuzzles.Add(puzzle);
    }
    public void UnregisterGlobalPuzzle(GlobalPuzzleBase puzzle)
    {
        activeGlobalPuzzles.Remove(puzzle);
    }
    #endregion
    private bool GlobalPuzzleCheck(string puzzleName)
    {
        bool globalPuzzleCompleted = true;
        foreach(string _referPuzzleName in globalPuzaleRefer[puzzleName])//Step1.对该全局谜题索引涉及的所有对象逐个在缓存或存档进行判断
        {

            if (PlayerPrefs.GetInt(_referPuzzleName) != globalPuzzleCheck[_referPuzzleName])//Step2.若该对象的缓存或存档与该全局谜题内该对象的对应的要求不同，则返回否并跳出，若直到最后都相同，则返回是
            {
                globalPuzzleCompleted = false;
                return globalPuzzleCompleted;
            }
        }
        return globalPuzzleCompleted;
        //puzzle.EnableInteractions(false);
    }

    public void FinishGlobalPuzzle(string puzzleName)
    {
        //修改缓存或存档
    }

}
