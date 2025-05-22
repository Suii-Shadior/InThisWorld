using AttackableInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public NewPlayerController thePlayer;
    public Transform theAttackItself;
    public int AttackDir;
    public Vector2 AttackVec;


    private void OnEnable()
    {
        AttackDirection();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<TurtleController>())
        {
            TurtleController enemy = other.gameObject.GetComponentInParent<TurtleController>();
            NewPlayerController player = this.GetComponentInParent<NewPlayerController>();
            
        }


    }

    private void AttackDirection()
    {
        switch (thePlayer.attackSignal)
        {
            case 1:
                AttackVec = new Vector2(thePlayer.faceDir,0);
                break;
            case 2:
                AttackVec = new Vector2(thePlayer.faceDir, 0);
                break;
            case 3:
                AttackVec = new Vector2(0, -1);
                break;
            case 4:
                AttackVec = new Vector2(0, 1);
                break;

        }
        Debug.Log(AttackVec);
    }
}
