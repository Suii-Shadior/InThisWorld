using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public NewPlayerController thePlayer;
    public Transform theAttackItself;
    public int AttackDir;
    public Vector2 AttackVec;


    private void Start()
    {
        AttackDirection();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>())
        {
            Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
            PlayerController player = this.GetComponentInParent<PlayerController>();
            
        }


    }

    private void AttackDirection()
    {
        if (thePlayer.transform.position.x > transform.position.x)
        {
            AttackDir = -1;
        }
        else
        {
            AttackDir = 1;
        }

    }
}
