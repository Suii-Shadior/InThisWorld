using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    private ControllerManager theCM;
    private LevelController theLevel;
    private AudioMixer thisAM;
    private AudioSource BGMPlayer;
    [SerializeField] private AudioClip currentChip;
    public AudioClip[] levelBGM;
    public AudioClip[] bossBGM;
    public float switchCounter;
    public float switchDuration;

    private void Awake()
    {
        theCM = GetComponentInParent<ControllerManager>();
        theLevel = theCM.theLevel;
        BGMPlayer = GetComponent<AudioSource>();
    }
    void Start()
    {
        NormalBGM();
        BGMPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BossBGM()
    {
        BGMPlayer.Stop();
        currentChip = levelBGM[2];
        BGMPlayer.clip = currentChip;
        BGMPlayer.Play();
    }

    private void NormalBGM()
    {

        if (theLevel.currentSceneName == "MainMenu")//
        {
            currentChip = levelBGM[0];
            BGMPlayer.clip = currentChip;

        }
        else 
        switch (theLevel.currentSceneName)
        {
            case "Level1":
                currentChip = levelBGM[2];
                BGMPlayer.clip = currentChip;
                break;
            case "Level2":
                currentChip = levelBGM[2];
                BGMPlayer.clip = currentChip;
                break;
            case "Level3":
                currentChip = levelBGM[2];
                BGMPlayer.clip = currentChip;
                break;
            case "Level4":
                currentChip = levelBGM[3];
                BGMPlayer.clip = currentChip;
                break;
            case "Level5":
                currentChip = levelBGM[3];
                BGMPlayer.clip = currentChip;
                break;
            case "Level6":
                currentChip = levelBGM[4];
                BGMPlayer.clip = currentChip;
                break;
            case "Level7":
                currentChip = levelBGM[4];
                BGMPlayer.clip = currentChip;
                break;
            case "Level8":
                currentChip = levelBGM[5];
                BGMPlayer.clip = currentChip;
                break;
            case "Level9":
                currentChip = levelBGM[6];
                BGMPlayer.clip = currentChip;
                break;
            default:
                break;
        }
    }
    public void NonBossBGM()
    {
        Debug.Log("πÿ“ÙœÏ£°");
        BGMPlayer.Stop();
        NormalBGM();
        BGMPlayer.Play();
    }

    private void BGMFadeOut()
    {

    }


}
