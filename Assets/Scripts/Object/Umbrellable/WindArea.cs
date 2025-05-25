using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindArea : MonoBehaviour
{
    private BoxCollider2D thisBC;
    #region ����
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
                //��ɡ״̬�µ����

            }
        }
    }
    #region С�������ⲿ����
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
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
