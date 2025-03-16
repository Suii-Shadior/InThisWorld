using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{

    public Vector2 AttackDir;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Enemy>())
        {
            Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
            PlayerController player = this.GetComponentInParent<PlayerController>();
            
        }

    }
}
