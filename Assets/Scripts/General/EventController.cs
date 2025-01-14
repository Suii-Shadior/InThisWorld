using System;
using UnityEngine;

public class EventController : MonoBehaviour
{

    public event EventHandler OnPlayerReset;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void PlayerResetPublish()
    {
        OnPlayerReset?.Invoke(this, EventArgs.Empty);
    }
}
