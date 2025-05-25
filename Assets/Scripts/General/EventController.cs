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
    #region SceneLoad���
    public delegate void RegisterSaveablesEventer(ISaveable _saveable);//���ձ�Ĺ㲥������ʽ�����ȶ���ί�б���
    public event RegisterSaveablesEventer OnSaveableRegister;//Ȼ�����ί�ж����Ӧ�¼�

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
    #region Interact���
    public delegate void LocalEventer(string _delegateChannel);
    public event LocalEventer OnLocalEvent;
    public event Action<string> OnGlobalEvent;//
    #endregion
    #region Reset���


    //EventHandler�����ǲ������κ����ݵ��¼�
    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region �ⲿ����


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
