using AttackInterfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class NailerController : AttackableBase
{

    [Header("Nailer Related")]
    public OnewayPlatform theOnewayPlatform;

    protected override void Awake()
    {
        base.Awake();
        thisAnim = GetComponentInChildren<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();

    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }



    public override void BePhysicalAttacked()
    {
        Debug.Log("³É¹¦");
        needToTriggered--;
        thisAnim.SetTrigger(ATTACKINGSTR);
        if (needToTriggered == 0)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(HASATTACKEDSTR, true);
            theOnewayPlatform.TriggerThisPlatform();
        }
        else
        {

            beAttackedCounter = beAttackedDuration;
            theOnewayPlatform.TriggerThisPlatform();
        }
    }
}
