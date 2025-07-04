using System.Collections;
using System.Collections.Generic;
using HandlerFactoryRelated;
using UnityEngine;
using InteractiveAndInteractableEnums;
using PlayerInterfaces;
using InteractableInterface;
using InteractableFactoryRelated;

public class HandlerController : MonoBehaviour,IInteract
{

    #region 变量
    [Header("Hnadler Setting")]
    public handlerType thisHandleType;
    private HandlerFactory_Handler thisFactory;
    public bool isMirrorInput;
    [Header("Hnader Info")]
    public NewPlayerController thePlayer;
    public IHandle currentHandler;
    [Header("MoveablePlatform Related")]
    public PlatformController[] thePlatforms;
    #endregion




    void Awake()
    {
        switch (thisHandleType)
        {
            case handlerType.moveableplatform_handler:
                thisFactory = new HandlerFactory();
                break;
            default:
                break;
        }
        //currentInteract = thisFactory?.CreateHandler(this);
        currentHandler = thisFactory?.CreateHandler(this);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController _thePlayer))
        {
            SetPlayer(_thePlayer);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController _thePlayer))
        {
            ClearPlayer(_thePlayer);
        }
    }


    #region 接口相关
    public void Interact()
    {
        thePlayer.SetHandle(currentHandler);
        thePlayer.ChangeToHandleState();
    }
    public void SetPlayer(NewPlayerController _thePlayer)
    {
        if (_thePlayer.theInteractable == null)
        {
            //Debug.Log("加入交互对象");
            _thePlayer.theInteractable = this;
            thePlayer = _thePlayer;
        }
    }

    public void ClearPlayer(NewPlayerController _thePlayer)
    {
        if (_thePlayer.theInteractable == this.GetComponent<IInteract>())
        {
            _thePlayer.theInteractable = null;
            thePlayer = null;
            //Debug.Log("移除交互对象");
        }
    }

    #endregion


}
