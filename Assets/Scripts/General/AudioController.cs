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
        //NormalBGM();
        //BGMPlayer.Play();
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
