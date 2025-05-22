using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchTrigger : MonoBehaviour
{
    private ControllerManager thisCM;
    private LevelController theLevel;
    public string theNewSceneName;


    private void Awake()
    {
        
    }
    private void Start()
    {
        thisCM = ControllerManager.instance;
        theLevel = thisCM.theLevel;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<NewPlayerController>()&&theLevel.GetCurrentSceneName() != theNewSceneName)
        {
            theLevel.SceneLoadTriggered(theNewSceneName);
            theLevel.SetCurrentSceneName(theNewSceneName);
        }
    }



}
