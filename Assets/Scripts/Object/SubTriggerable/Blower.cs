using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blower : MonoBehaviour
{

    public bool isOperating;
    private Animator thisAnim;
    private WindArea thisWindZone;
    // Start is called before the first frame update

    private void Awake()
    {
        thisAnim = GetComponentInChildren<Animator>();
        thisWindZone = GetComponentInChildren<WindArea>();
        //∂¡»°¥Êµµ
    }
    void Start()
    {
        if (isOperating)
        {
            //thisAnim.SetTrigger();
            thisWindZone.WindZoneOn();
        }
        else
        {
            //thisAnim.SetBool();
            thisWindZone.WindZoneOff();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Operate()
    {

    }


}
