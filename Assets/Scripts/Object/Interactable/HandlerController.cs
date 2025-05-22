using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using InteractiveAndInteractableEnums;
using PlayerInterfaces;
using InteractableFactoryRelated;

public class HandlerController : MonoBehaviour,IInteract
{

    #region 变量
    [Header("Hnadler Setting")]
    public handlerType thisHandleType;
    private HandlerFactory thisFactory;
    public bool isMirrorInput;
    [Header("Hnader Info")]
    public NewPlayerController thePlayer;
    private IHandle currentHandler;
    [Header("MoveablePlatform Related")]
    public PlatformController[] thePlatforms;
    #endregion



    // Start is called before the first frame update
    void Awake()
    {
        switch (thisHandleType)
        {
            case handlerType.moveableplatform_handler:
                thisFactory = new PlatformHandlerFactory();
                break;
            default:
                break;
        }
        currentHandler = thisFactory?.CreateHandler(this);
    }

    // Update is called once per frame


    #region 接口相关
    public void Interact()
    {
        thePlayer.theHandle = currentHandler;
        thePlayer.ChangeToHandleState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController _thePlayer))
        {
            if (_thePlayer.theInteractable == null)
            {
                //Debug.Log("加入交互对象");
                _thePlayer.theInteractable = this;
                thePlayer = _thePlayer;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController _thePlayer))
        {
            if (_thePlayer.theInteractable == this.GetComponent<IInteract>())
            {
                _thePlayer.theInteractable = null;
                thePlayer = null;
                //Debug.Log("移除交互对象");
            }
        }
    }
    #endregion
}
