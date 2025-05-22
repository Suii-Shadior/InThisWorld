using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour
{

    [HideInInspector]public enum spikeDir { Upward,Leftward,Rightward,Downward}
    private SpriteRenderer thisSR;
    private BoxCollider2D thisBixCol;

    public spikeDir thisSpikeType;


    private void Awake()
    {
        thisSR = GetComponent<SpriteRenderer>();
        thisBixCol = GetComponent<BoxCollider2D>();
        //switch (thisSpikeType)
        //{
        //    case spikeDir.Upward:
        //        transform.rotation = Quaternion.Euler(0, 0, 0);
        //        break;
        //    case spikeDir.Downward:
        //        transform.rotation = Quaternion.Euler(0, 0, 180);
        //        break;
        //    case spikeDir.Leftward:
        //        transform.rotation = Quaternion.Euler(0, 0, 90);
        //        break;
        //    case spikeDir.Rightward:
        //        transform.rotation = Quaternion.Euler(0, 0, 270);
        //        break;
        //}

    }
    void Start()
    {
        thisBixCol.size = thisSR.size;
        thisBixCol.offset = new Vector2(0, thisSR.size.y / 2);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
            NewPlayerController thePlayer = other.GetComponent<NewPlayerController>();

            switch (thisSpikeType)
            {
                case spikeDir.Upward:
                    if (thePlayer.thisRB.velocity.y < 0)
                    {
                        Debug.Log("À¿Õˆ");
                    }
                    break;
                case spikeDir.Downward:
                    if (thePlayer.thisRB.velocity.y > 0)
                    {
                        Debug.Log("À¿Õˆ");
                    }
                    break;
                case spikeDir.Leftward:
                    if (thePlayer.thisRB.velocity.x > 0)
                    {
                        Debug.Log("À¿Õˆ");
                    }
                    break;
                case spikeDir.Rightward:
                    if (thePlayer.thisRB.velocity.x < 0)
                    {
                        Debug.Log("À¿Õˆ");
                    }
                    break;
            }
        }
    }
}
