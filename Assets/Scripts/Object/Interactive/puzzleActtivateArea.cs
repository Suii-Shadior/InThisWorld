using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class puzzleActtivateArea : MonoBehaviour
{


    public bool isActive;
    public string localPublisherChannel_ActivatePuzzle;

    public EventController theEC;

    // Start is called before the first frame update
    private void Start()
    {
        theEC = ControllerManager.instance.theEvent;    
    }
    private void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>())
        {
           
        }
    }
}
