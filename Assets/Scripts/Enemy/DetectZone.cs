using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectZone : MonoBehaviour
{
    private EnemyBase thisEnemy;
    private float theDistance;


    private void Awake()
    {
        thisEnemy=GetComponentInParent<EnemyBase>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            //Debug.Log("?");
            thisEnemy.SetPlayer(other.GetComponent<NewPlayerController>());
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            thisEnemy.ClearPlayer();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            if (other.GetComponent<NewPlayerController>()== thisEnemy.GetPlayer())
            {
                theDistance = Vector2.Distance(thisEnemy.thisEnemyPos(), other.GetComponent<NewPlayerController>().thisDP.position);
            }
        }
    }
}
