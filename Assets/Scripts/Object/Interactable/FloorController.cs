using InteractiveAndInteractableEnums;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//[RequireComponent(typeof(Rigidbody2D),typeof(CompositeCollider2D))]
public class FloorController : MonoBehaviour
{


    #region 组件
    private Animator thisAnim;
    private Collider2D thisCol;
    private Rigidbody2D thisRB;

    #endregion


    [Header("Floor Setting")]
    public floorType ThisFloorType;
    [Header("Floor Info")]
    private NewPlayerController thePlayer;



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
        switch (ThisFloorType)
        {
            case floorType.passable_piper:
                Floor_PiperAwake();
                break;
            case floorType.destroyable_breaker:
                Floor_BreakerAwake();
                break;
        }
        //读取存档
    }

    private void Floor_PiperAwake()
    {
        thisCol = GetComponent<CompositeCollider2D>();
        thisAnim = GetComponent<Animator>();
        thisRB = GetComponent<Rigidbody2D>();
    }
    private void Floor_BreakerAwake()
    {
        thisCol = GetComponent<CompositeCollider2D>();
        thisAnim = GetComponent<Animator>();
    }



    private void Start()
    {
        switch (ThisFloorType) 
        {
            case floorType.passable_piper:
                Floor_PiperStart();
                break;
            case floorType.destroyable_breaker:
                Floor_BreakerStart();
                break;
        }

    }


    private void Floor_PiperStart()
    {
        thisCol.enabled = true;
    }
    private void Floor_BreakerStart()
    {
        if (hasDetroyed)
        {
            thisCol.enabled = false;
            needToDestroy = 0;
        }
        else
        {
            thisCol.enabled = true;
            needToDestroy = numToDestroy;
        }
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


    public void Floor_Enter()
    {
        switch (ThisFloorType)
        {
            case floorType.passable_piper:
                Passable_PiperEnter();
                break;
            case floorType.destroyable_breaker:
                Destroyable_BreakerEnter();
                break;
        }
    }
    private void Passable_PiperEnter()
    {
        passThroughCounter = passThroughDuration;
    }

    private void Destroyable_BreakerEnter()
    {
        if (thePlayer.thisRB.velocity.y<-breakThreshold)
        {
            needToDestroy--;
            if (needToDestroy == 0)
            {

                StartCoroutine(DestoryThisFloor());
            }
        }
    }
    public void Floor_Stay()
    {
        switch (ThisFloorType)
        {
            case floorType.passable_piper:
                Passable_PiperStay();
                break;
            case floorType.destroyable_breaker:
                break;
        }
    }

    public void Passable_PiperStay()
    {
        if (thePlayer != null)
        {

            if (thePlayer.verticalInputVec < 0)
            {
                passThroughCounter -= Time.deltaTime;
                if (passThroughCounter < 0)
                {
                    StartCoroutine(Pipe());
                    thePlayer = null;
    
                }

            }
            else
            {
                passThroughCounter = passThroughDuration;

            }
        }
        else
        {

        }
    }



    private IEnumerator Pipe()
    {
        thePlayer.Unact(passDuration);
        thisCol.isTrigger = true;
        //thePlayer.transform.position = thePipeLocation.position;
        Debug.Log("进入");
        //while (isPipeTransmiting)
        //{
        //    thePlayer.transform.position += (Vector3)Vector2.down * Time.deltaTime;
        //    if (thePlayer.canAct)
        //    {
        //        isPipeTransmiting = false;
        //    }
        //    yield return new WaitForSeconds(Time.deltaTime);
        //}
        yield return new WaitForSeconds(passDuration);

        Debug.Log("退出");
    }

    private IEnumerator DestoryThisFloor()
        
    {
        //第一段动画
        Debug.Log("1");
        yield return new WaitForSeconds(breakDuration1);
        //第二段动画
        Debug.Log("2");
        yield return new WaitForSeconds(breakDuration2);
        //第三段动画
        Debug.Log("3");
        hasDetroyed = true;
        this.gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            thisCol.isTrigger = false;
        }
    }
}
