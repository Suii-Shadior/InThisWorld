using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            PlayerController thePlayer = other.GetComponent<PlayerController>();
            Debug.Log("ÕÊº“À¿¡À");
            thePlayer.ChangeToDeadState();
        }
    }
}
