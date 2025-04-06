using AttackInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoterBulletController : MonoBehaviour,IPhysicalAttack
{
    //private Animator thisAnim;
    private Collider2D thisCol;
    [Header("Setting")]
    public float moveSpeed;

    [Header("Info")]
    public int movingDir;
    public bool isDestroyed;
    private ShoterController thisShoter;
    [Header("Animator Related")]
    private const string MOVESTR = "Move";
    private const string DESTROYEDSTR = "Destroyed";


    private void Start()
    {
        
    }
    private void Update()
    {
        if (!isDestroyed)
        {
            transform.position += new Vector3(movingDir * moveSpeed * Time.deltaTime, 0f,0f);
        }
        else
        {
            //if(thisAnim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= .9f)
            //{
            //    isDestroyed = false;
            //    //thisAnim.SetBool(DESTROYEDSTR, false);
            //    //thisAnim.SetBool(MOVESTR, true);
            //    this.gameObject.SetActive(false);
            //}
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<AttackArea>())
        {
            BePhysicalAttacked();
        }
        else if(other.CompareTag("Ground"))
        {
            BePhysicalAttacked();
        }
    }

    public void BePhysicalAttacked()
    {
        isDestroyed = true;
        //thisAnim.SetBool(DESTROYEDSTR, true);
        //thisAnim.SetBool(MOVESTR, false);
        this.gameObject.SetActive(false);
    }


    public void ResetThis()
    {
        isDestroyed = false;
        this.gameObject.SetActive(true);

    }
}
