using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleBase : MonoBehaviour
{
    #region ×é¼þ
    protected PuzzleController thePC;
    protected EventController theEC;
    #endregion




    protected virtual void Awake()
    {
    }
    protected virtual void OnEnable()
    {
        thePC = ControllerManager.instance.thePC;
        theEC = ControllerManager.instance.theEvent;

    }
    protected virtual void Start()
    {

    }
    protected virtual void OnDisable()
    {

    }






}

