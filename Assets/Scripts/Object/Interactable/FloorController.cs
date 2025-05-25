using InteractiveAndInteractableEnums;
using System.Collections;
using System.Collections.Generic;
using InteractableInterface;
using UnityEngine;
using FloorFactoryRelated;


//[RequireComponent(typeof(Rigidbody2D),typeof(CompositeCollider2D))]
public class FloorController : MonoBehaviour
{
    [Header("��������ʹ��˵����������\n1��ѡ�����õĵذ����ͣ���thisFloorType")]

    #region ���
    private Animator thisAnim;
    private Collider2D thisCol;
    //private Rigidbody2D thisRB;

    #endregion


    [Header("Floor Setting")]
    public floorType thisFloorType;

    [Header("Floor Info")]
    public NewPlayerController thePlayer;
    public IFloor currentFloor;


    [Header("Piper Related")]
     public float passThroughCounter;
     public float passThroughDuration;
    public bool isPipeTransmiting;
    public Transform thePipeLocation;
    public float passDuration;
    [Header("Breaker Related")]
     public bool hasDetroyed;
    public int numToDestroy;
    public int needToDestroy;
    public float breakThreshold;
    public float breakDuration1;
    public float breakDuration2;
    private void Awake()
    {
        thisCol = GetComponent<CompositeCollider2D>();
        thisAnim = GetComponent<Animator>();

        switch (thisFloorType)
        {
            case floorType.passable_piper:
                currentFloor = new PiperFactory().CreateFloor(this);
                break;
            case floorType.destroyable_breaker:
                currentFloor = new BreakerFactory().CreateFloor(this);
                break;
        }
        //��ȡ�浵
    }





    private void Start()
    {
        currentFloor?.SceneLoad_Start();
    }



    public void SetPlayer(NewPlayerController _thePlayer)
    {
        thePlayer = _thePlayer;
    }

    public NewPlayerController GetPlayer()
    {
        return thePlayer;
    }

    public void ClearPlayer()
    {
        thePlayer = null;
    }


    #region С�������ⲿ����

    public void ColOn()
    {
        thisCol.enabled = true;
    }
    public void ColOff()
    {
        thisCol.enabled = false;
    }

    public void ColIsTrigger()
    {
        thisCol.isTrigger = true;
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            thisCol.isTrigger = false;
        }
    }
    #endregion
}
