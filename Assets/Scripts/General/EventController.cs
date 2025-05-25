using System;
using Unity.VisualScripting;
using UnityEngine;
using StructForSaveData;

public class EventController : MonoBehaviour
{
    //public event EventHandler<LocalEventerArgs> OnLocalEventer;

    //public class LocalEventerArgs : EventArgs
    //{
    //    public string levelName;
    //    public int localSubscriberChannel;
    //}
    #region SceneLoad相关
    public delegate void RegisterSaveablesEventer(ISaveable _saveable);//最普遍的广播监听方式，首先定义委托本身
    public event RegisterSaveablesEventer OnSaveableRegister;//然后基于委托定义对应事件

    public delegate void UnregisterSaveableEventer(ISaveable _saveable);
    public event UnregisterSaveableEventer OnSaveableUnregister;

    public delegate void RegisterLocalPuzzleEventer(LocalPuzzleBase _localpuzzle);
    public event RegisterLocalPuzzleEventer OnLocalPuzzleRegister;

    public delegate void UnregisterPuzzleEventer(LocalPuzzleBase _localpuzzle);
    public event UnregisterPuzzleEventer OnLocalPuzzleUnregister;

    public delegate void RegisterGlobalPuzzleEventer(GlobalPuzzleBase _globalpuzzle);
    public event RegisterGlobalPuzzleEventer OnGlobalPuzzleRegister;

    public delegate void UnregisterGlobalPuzzleEventer(GlobalPuzzleBase _globalpuzzle);
    public event UnregisterGlobalPuzzleEventer OnGlobalPuzzleUnregister;


    public Action pendingAction;



    #endregion
    #region Interact相关
    public delegate void LocalEventer(string _delegateChannel);
    public event LocalEventer OnLocalEvent;
    public event Action<string> OnGlobalEvent;//
    #endregion
    #region Reset相关


    //EventHandler就是是不传递任何内容的事件
    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region 外部调用


    public void RegisterConfirmation(Action _confirmAction)
    {
        pendingAction = _confirmAction;
    }

    public void CancleConfirmation()
    {
        pendingAction = null;
    }

    public void ExecutePendingAction()
    {
        pendingAction?.Invoke();
        pendingAction = null;
    }

    public void LocalEventPublish(string _eventPublisher)
    {
        OnLocalEvent?.Invoke(_eventPublisher);
    }

    public void GlobalEventPublish(string _globalEventRefer)
    {
        OnGlobalEvent?.Invoke(_globalEventRefer);
    }


    public void LoaclPuzzleRegisterPublish(LocalPuzzleBase _localpuzzle)
    {
        OnLocalPuzzleRegister?.Invoke(_localpuzzle);
    }
    public void LocalPuzzleUnregisterPublish(LocalPuzzleBase _localpuzzle)
    {
        OnLocalPuzzleUnregister?.Invoke(_localpuzzle);
    }

    public void GlobalPuzzleRegisterPublish(GlobalPuzzleBase _localpuzzle)
    {
        OnGlobalPuzzleRegister?.Invoke(_localpuzzle);
    }
    public void GlobalPuzzleUnregisterPublish(GlobalPuzzleBase _localpuzzle)
    {
        OnGlobalPuzzleUnregister?.Invoke(_localpuzzle);
    }
    public void SaveableRegisterPublish(ISaveable _saveable)
    {
        OnSaveableRegister?.Invoke(_saveable);
    }

    public void SaveableUnregisterPublish(ISaveable _saveable)
    {
        
        OnSaveableUnregister?.Invoke(_saveable);
    }
    #endregion
}
