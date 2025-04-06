using AttackInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(Animator),typeof(BoxCollider2D))]
public abstract class AttackableBase : MonoBehaviour, IPhysicalAttack
{


    #region ×é¼þ
    protected Animator thisAnim;
    protected BoxCollider2D thisBoxCol;
    #endregion


    [Header("Setting")]
    public float beAttackedDuration;
    public int numToTriggered;
    [Header("Info")]
    public int needToTriggered;
    public bool hasAttacked;
    protected float beAttackedCounter;
    public Vector2 attackVec2;

    [Header("Animator Related")]
    protected const string HASATTACKEDSTR = "hasAttacked";
    protected const string UNATTACKEDSTR = "isUnattacked";
    protected const string ATTACKINGSTR = "isAttacking"; 
    
    protected virtual void Awake()
    {
        thisAnim = GetComponentInChildren<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
    }


    protected virtual void Start()
    {
        if (hasAttacked)
        {
            thisBoxCol.enabled = false;
            needToTriggered = 0;
            thisAnim.SetBool(HASATTACKEDSTR, true);
            //this.gameObject.SetActive(false);
        }
        else
        {

            needToTriggered = numToTriggered;
            thisAnim.SetBool(UNATTACKEDSTR, true);

        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (beAttackedCounter > 0)
        {
            beAttackedCounter -= Time.deltaTime;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AttackArea>())
        {
            if (beAttackedCounter <= 0)
            {
                attackVec2 = other.GetComponent<AttackArea>().AttackVec;
                BePhysicalAttacked();
            }
        }
    }

    public abstract void BePhysicalAttacked();

}
