using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    private BoxCollider2D thisBC;
    #region 变量
    //public Transform windOrigin;
    //public float windRange;
    public NewPlayerController thePlayer;
    public float windBlowSpeed;
    //[SerializeField] private GameObject theFX;

    #endregion


    private void Awake()
    {
        thisBC = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            thePlayer = other.GetComponent<NewPlayerController>();
            if (thePlayer.stateMachine.currentState == thePlayer.umbrellaState)
            {
                //撑伞状态下的玩家

            }
        }
    }
    #region 小方法和外部调用
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            thePlayer = null;
        }
    }

    public void WindZoneOn()
    {
        thisBC.enabled = true;
        //if (theFX != null) theFX.SetActive(true);
    }
    public void WindZoneOff()
    {
        thisBC.enabled = false;
        //if (theFX != null) theFX.SetActive(false);
    }

    #endregion
}
