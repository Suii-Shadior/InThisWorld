using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private ControllerManager theCM;
    //private PlayerController thePlayer;
    private NewPlayerController thePlayer;
    private UIController theUI;
    private DialogeController theDC;
    private AudioController theBGMPlayer;
    private EventController theEvents;
    public bool isLevelLoading;
    public float levelLoadingDuration;
    public bool isPausing;
    private float tempTimeSpeed;
    public int activeResetIndex;
    public Transform resetPos;
    public bool resetFaceRight;
    [Header("Scene")]
    public string currentSceneName;




    private void Awake()
    {
        theCM = GetComponentInParent<ControllerManager>();
        thePlayer = theCM.thePlayer;
        theUI = theCM.theUI;
        theBGMPlayer = theCM.theBGMPlayer;
        theEvents = theCM.theEvent;
        theDC = theCM.theDC;
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        isLevelLoading = true;
        LevelLoadingFalse();
        if (currentSceneName == "LevelSelect")
        {
            if (!PlayerPrefs.HasKey("DialogueHasTriggered_0"))
            {
                theDC.SetUpNewDialogue(theDC.textFiles[0]);
                PlayerPrefs.SetInt("DialogueHasTriggered_0", 1);
            }
            else if (!PlayerPrefs.HasKey("DialogueHasTriggered_5"))
            {
                theDC.SetUpNewDialogue(theDC.textFiles[0]);
                PlayerPrefs.SetInt("DialogueHasTriggered_5", 1);
            }
        }
    }



    public void Respawn()
    {
        StartCoroutine(ResetPlayer());
    }

    private IEnumerator ResetPlayer()
    {
        yield return new WaitForSeconds(2f);
        theUI.FadeOut();
        yield return new WaitForSeconds(1 / theUI.fadeOutRatio);//等待实际淡出的时间
        theEvents.PlayerResetPublish();
        thePlayer.gameObject.transform.position = resetPos.position;
        thePlayer.faceRight = resetFaceRight;
        thePlayer.ChangeToIdleState();



        yield return new WaitForSeconds(1f);
        theUI.FadeIn();
        yield return new WaitForSeconds(1 / theUI.fadeInRatio);//等待实际淡入的时间
    }


    public void ResetPos(Transform _thisResetPos, bool _thisResetFaceRight)
    {
        //Debug.Log("更新重生点");
        resetFaceRight = _thisResetFaceRight;
        resetPos = _thisResetPos;
    }

    public void LevelLoadingFinish()
    {
        isLevelLoading = false;

    }
    public void LevelLoadingStart()
    {
        isLevelLoading = true;

    }

    public void GamePlayPause()
    {
        tempTimeSpeed = Time.timeScale;
        Time.timeScale = 0;

    }
    public void GamePlayResume()
    {
        Time.timeScale = tempTimeSpeed;
    }



    private IEnumerator BackToLevelSelectCo()
    {
        theUI.FadeOut();
        yield return new WaitForSeconds(1 / theUI.fadeInRatio);
        SceneManager.LoadScene("LevelSelect");
    }

    private IEnumerator GoToLevelSelectedCo(string _levelSelected)
    {
        theUI.FadeOut();
        yield return new WaitForSeconds(1 / theUI.fadeInRatio);
        SceneManager.LoadScene(_levelSelected);
    }


    public void BackToLevelSelect()
    {
        StartCoroutine(BackToLevelSelectCo());
    }

    public void GoToLevelSelected(string _levelSelected)
    {
        StartCoroutine(GoToLevelSelectedCo(_levelSelected));
        switch (_levelSelected)
        {
            case "Level1":
                PlayerPrefs.SetInt("CurrentlyPlayedLevel", 1);
                break;
            case "Level2":
                PlayerPrefs.SetInt("CurrentlyPlayedLevel", 2);
                break;
            case "Level3":
                PlayerPrefs.SetInt("CurrentlyPlayedLevel", 3);
                break;
            case "Level4":
                PlayerPrefs.SetInt("CurrentlyPlayedLevel", 4);
                break;
            case "Level5":
                PlayerPrefs.SetInt("CurrentlyPlayedLevel", 5);
                break;
            case "Level6":
                PlayerPrefs.SetInt("CurrentlyPlayedLevel", 6);
                break;
            case "Level7":
                PlayerPrefs.SetInt("CurrentlyPlayedLevel", 7);
                break;
            case "Level8":
                PlayerPrefs.SetInt("CurrentlyPlayedLevel", 8);
                break;
            case "Level9":
                PlayerPrefs.SetInt("CurrentlyPlayedLevel", 9);
                break;
            default:
                Debug.Log("不是关卡");
                break;

        }

    }

    public void BackToMainMenu()
    {
        StartCoroutine(BackToMainMenuCo());
    }

    private IEnumerator BackToMainMenuCo()
    {
        Time.timeScale = 1;
        theUI.FadeOut();
        yield return new WaitForSeconds(1 / theUI.fadeInRatio);
        SceneManager.LoadScene("MainMenu");
    }

    private void LevelLoadingFalse()
    {
        StartCoroutine(LevelLoadingFalseCo());
    }
    private IEnumerator LevelLoadingFalseCo()
    {
        yield return new WaitForSeconds(levelLoadingDuration);
        isLevelLoading = false;
    }
}
