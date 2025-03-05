using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //public static UIController instance;
    private ControllerManager thisCM;
    //private PlayerController thePlayer;
    private NewPlayerController thePlayer;
    private LevelController theLevel;
    private InputController theInput;
    private DialogeController theDC;//用于触发对话

    [SerializeField] private GameObject theMainmenuUI;
    [SerializeField] private GameObject theGamePlayLevelUI;
    [SerializeField] private GameObject thePauseUI;
    [SerializeField] private GameObject theDialogeUI;

    [Header("Transition")]
    public Image theBlackScreen;
    public float fadeInRatio;
    public float fadeOutRatio;
    public bool isFadeIn;
    public bool isFadeOut;


    [Header("MainMenu Info")]
    //public GameObject theStartButton;
    //public GameObject theContinueButton;
    //public GameObject theQuitButton;
    //public Transform theFirstStartPos;
    //public Transform theFirstQuitPos;



    [Header("Skill Info")]
    //public Image thePullInfo;
    //public Image thePullGrayInfo;
    //public Image theBoomInfo;
    //public Image theElectricInfo;
    //public Image thePlantInfo;
    //public Image theBabbleInfo;
    //public TextMeshProUGUI thePullText;
    //public TextMeshProUGUI theBoomText;
    //public TextMeshProUGUI theElectricText;
    //public TextMeshProUGUI thePlantText;
    //public TextMeshProUGUI theBabbleText;
    //private Vector4 uncooldownColor = new Vector4(1, 1, 1, 0);
    //private Vector4 cooldownColor = new Vector4(0, 0, 0, 1);
    //public Sprite NoAbilitySprite;
    //public List<Image> needCooldownSkills = new List<Image>();
    [Header("Dialogue")]
    public bool isDialogue;
    private void Awake()
    {
    }
    private void Start()
    {
        thisCM = ControllerManager.instance;
        theLevel = thisCM.theLevel;
        thePlayer = thisCM.thePlayer;
        theInput = thisCM.theInput;
        theDC = thisCM.theDC;
        UICanvasSelect();
        UIObjectsUpdate();
        FadeIn();
    }


    private void Update()
    {
        FadeInOrFadeOut();//Step1.淡入淡出
        UIDisplayUpdate();//Step2.更新其他全局UI相关信息，例如事件，血量，分数
        //Step3.更新其他非全局UI信息，例如触发提示
    }

    #region 基本方法
    private void FadeInOrFadeOut()
    {
        if (isFadeIn)
        {
            //Debug.Log("淡入");
            theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, Mathf.MoveTowards(theBlackScreen.color.a, 0f, fadeInRatio * Time.deltaTime));
            if (theBlackScreen.color.a == 0f)
            {
                isFadeIn = false;

            }

        }
        if (isFadeOut)
        {
            //Debug.Log("淡出 ");
            theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, Mathf.MoveTowards(theBlackScreen.color.a, 1f, fadeOutRatio * Time.deltaTime));
            if (theBlackScreen.color.a == 1f)
            {
                isFadeOut = false;

            }
        }
    }

    public void FadeIn()
    {
        isFadeIn = true;
        isFadeOut = false;
    }
    public void FadeOut()
    {

        isFadeIn = false;
        isFadeOut = true;
    }

    #endregion
    #region GUI方法
    private void UICanvasSelect()//根据当前场景调整UI所在画布
    {
        //theBlackScreen.transform.gameObject.SetActive(true);
        if (theLevel.currentSceneName == "MainMenu")
        {
            theMainmenuUI.SetActive(true);
            theGamePlayLevelUI.SetActive(false);
            theBlackScreen.transform.SetParent(theMainmenuUI.transform, false);
        }
        else
        {
            //theMainmenuUI.SetActive(false);
            theGamePlayLevelUI.SetActive(true);
            theBlackScreen.transform.SetParent(theGamePlayLevelUI.transform, false);

        }
    }
    public void UIObjectsUpdate()//根据游戏进度调整UI显示对象
    {
        if (theLevel.currentSceneName == "MainMenu")
        {
            if (!PlayerPrefs.HasKey("HasStartGame"))
            {
                //已经开始过游戏
                //theStartButton.transform.position = theFirstStartPos.position;
                //theContinueButton.SetActive(false);
                //theStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
                //theQuitButton.transform.position = theFirstQuitPos.position;
            }
            else
            {
                //还没有开始过游戏
            }
        }
        else
        {
            //Gameplay时候更新对象
        }
    }
    public void UIDisplayUpdate()//根据游戏进度更新UI显示内容
    {

    }


    public void TurnOnDialogCanvas()
    {
        theDialogeUI.SetActive(true);
        isDialogue = true;
    }
    public void TurnOffDialogCanvas()
    {
        theDialogeUI.SetActive(false);
        isDialogue = false;
    }
    public void GamePlayPause()
    {
        thePauseUI.SetActive(true);
    }

    public void GamePlayResume()
    {
        thePauseUI.SetActive(false);
    }
    #endregion

    #region 事件帧调用
    public void theStartFunction()
    {
        //PlayerPrefs.SetInt("AnimatonTransition", 1);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("HasStartGame", 1);
        PlayerPrefs.SetInt("Level1" + "_unlocked", 1);
        //theLevel.GoToLevelSelected("LevelSelect");
        theLevel.GoToLevelSelected("AnimationTransition");

    }
    public void theContinueFunction()
    {
        theLevel.BackToLevelSelect();
    }
    public void theQuitFunction()
    {
        Application.Quit();
        Debug.Log("Quit This Game");
    }

    public void theResumeFunction()
    {

        GamePlayResume();
        theLevel.GamePlayResume();
        if (theLevel.currentSceneName != "LevelSelect")
        {
            theInput.GamePlayInput();
        }
        else
        {
            theInput.LevelSelectInput();
        }
    }

    public void theMainMenuFunction()
    {
        theLevel.BackToMainMenu();
    }

    #endregion
}
