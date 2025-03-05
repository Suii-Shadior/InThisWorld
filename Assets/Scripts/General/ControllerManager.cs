using UnityEngine;

public class ControllerManager : MonoBehaviour
{
    public static ControllerManager instance;
    //public PlayerController thePlayer;
    //public LSPlayerController theLSPlayer;
    public NewPlayerController thePlayer;
    public CameraController theCamera;
    public InputController theInput;
    public LevelController theLevel;
    public UIController theUI;
    public AudioController theBGMPlayer;
    public EventController theEvent;
    public DialogeController theDC;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {

    }
}
