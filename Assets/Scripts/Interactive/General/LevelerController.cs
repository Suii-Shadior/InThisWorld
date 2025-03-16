using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelerController : MonoBehaviour
{
    public enum levelerType { attackable_rotater,attackable_elevator,}
    public levelerType thisLevelerType;


    [Header("Switch Info")]
    public bool isInteracted;
    public bool canInteracted;
    private float canInteractedCounter;
    public float canInteractedDuration;
    [Header("Rotater Related")]
    public PlatformController[] rotatePlatforms;
    public int theAttackFrom;



    [Header("Elevator Related")]
    public bool isUpwardOriention;

    [Header("Combine Related")]
    public PlatformController[] triggeredPlatforms;



    [Header("Animator Related")]
    private const string UNINTERACTSTR = "isUnacted";
    private const string INTERACT1STR = "isTriggered";
    private const string INTERACT2STR = "isTriggering";
    private const string UNTRIGGERINGSTR = "isUntriggering";
    private void Start()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AttackArea>())
        {
            Debug.Log("检测到了");
            AttackArea theAttack = other.GetComponent<AttackArea>();
            switch (thisLevelerType) {
                case levelerType.attackable_rotater:

                    if (theAttack.thePlayer.transform.position.x > transform.position.x)
                    //if (theAttack.AttackDir == 1)
                    {
                        Debug.Log("左手一个慢动作");
                        //ClockwiseRotate();
                    }
                    else
                    {
                        Debug.Log("右手慢动作重播");
                        //AntiClockwiseRotate();
                    }
                    break;
            }

        }
    }


    private void ClockwiseRotate()
    {

    }
    private void AntiClockwiseRotate()
    {

    }


}
