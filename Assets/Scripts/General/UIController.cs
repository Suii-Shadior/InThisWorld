using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    //public static UIController instance;
    private ControllerManager thisCM;
    private PlayerController thePlayer;
    private LevelController theLevel;
    private InputController theInput;
    private DialogeController theDC;

    [SerializeField] private GameObject theMainmenuUI;
    [SerializeField] private GameObject theGamePlayLevelUI;
    [SerializeField] private GameObject theLevelSelectUI;
    [SerializeField] private GameObject theGameEndUI;
    [SerializeField] private GameObject thePauseUI;
    [SerializeField] private GameObject theDialogeUI;

    [Header("Transition")]
    public Image theBlackScreen;
    public float fadeInRatio;
    public float fadeOutRatio;
    public bool isFadeIn;
    public bool isFadeOut;


    [Header("MainMenu Info")]
    public GameObject theStartButton;
    public GameObject theContinueButton;
    public GameObject theQuitButton;
    public Transform theFirstStartPos;
    public Transform theFirstQuitPos;



    [Header("Skill Info")]
    public Image thePullInfo;
    public Image thePullGrayInfo;
    public Image theBoomInfo;
    public Image theElectricInfo;
    public Image thePlantInfo;
    public Image theBabbleInfo;
    public TextMeshProUGUI thePullText;
    public TextMeshProUGUI theBoomText;
    public TextMeshProUGUI theElectricText;
    public TextMeshProUGUI thePlantText;
    public TextMeshProUGUI theBabbleText;
    private Vector4 uncooldownColor = new Vector4(1, 1, 1, 0);
    private Vector4 cooldownColor = new Vector4(0, 0, 0, 1);
    public Sprite NoAbilitySprite;
    public List<Image> needCooldownSkills = new List<Image>();
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
        UICanvas();
        UIDisplay();
        FadeIn();
    }


    private void Update()
    {
        FadeInOrFadeOut();
        SkillDisplayUpdate();
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
    public void UIDisplay()//根据游戏进度调整UI显示范围
    {
        if (theLevel.currentSceneName == "LevelSelect" || theLevel.currentSceneName == "GameEnd")
        {

        }
        else if (theLevel.currentSceneName == "MainMenu")
        {
            if (!PlayerPrefs.HasKey("HasStartGame"))
            {
                theStartButton.transform.position = theFirstStartPos.position;
                theContinueButton.SetActive(false);
                theStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
                theQuitButton.transform.position = theFirstQuitPos.position;
            }
            else
            {
            }
        }
        else
        {
            if (thePlayer.babbleAbility)
            {
                needCooldownSkills.Add(theBabbleInfo);
            }
            else
            {
                theBabbleInfo.sprite = NoAbilitySprite;

            }
        }
    }
    public void SkillDisplayUpdate()
    {
        //if (在Gameplay时)
        //{   对需要进行冷却的所有对象进行遍历更新UI展示（包括计时和数值）
        //    foreach (Image needCooldownSkill in needCooldownSkills)
        //    {

        //        switch (needCooldownSkill.name)
        //        {
        //            case "Q-Icon":
        //                float TempRatio = thePlayer.pullCooldownCounter / thePlayer.pullCooldownDuration;
        //                needCooldownSkill.fillAmount = 1 - TempRatio;
        //                break;
        //            case "W-Icon":
        //                TempRatio = thePlayer.boomCooldownCounter / thePlayer.boomCooldownDuration;
        //                needCooldownSkill.fillAmount = 1 - TempRatio;
        //                break;
        //        }
        //    }
        //    if (thePlayer.canPull) thePullText.color = cooldownColor;
        //    else thePullText.color = uncooldownColor;
        //}
    }

    private void UICanvas()
    {
        //theBlackScreen.transform.gameObject.SetActive(true);
        if (theLevel.currentSceneName == "LevelSelect")
        {
            theMainmenuUI.SetActive(false);
            theGamePlayLevelUI.SetActive(false);
            theLevelSelectUI.SetActive(true);
            theGameEndUI.SetActive(false);
            theBlackScreen.transform.SetParent(theLevelSelectUI.transform, false);


        }
        else if (theLevel.currentSceneName == "MainMenu")
        {
            theMainmenuUI.SetActive(true);
            theGamePlayLevelUI.SetActive(false);
            theLevelSelectUI.SetActive(false);
            theGameEndUI.SetActive(false);
            theBlackScreen.transform.SetParent(theMainmenuUI.transform, false);
        }
        else if (theLevel.currentSceneName == "GameEnd")
        {
            theMainmenuUI.SetActive(false);
            theGamePlayLevelUI.SetActive(false);
            theLevelSelectUI.SetActive(false);
            theGameEndUI.SetActive(true);
            theBlackScreen.transform.SetParent(theGameEndUI.transform, false);

        }
        else
        {
            //theMainmenuUI.SetActive(false);
            theGamePlayLevelUI.SetActive(true);
            theLevelSelectUI.SetActive(false);
            theGameEndUI.SetActive(false);
            theBlackScreen.transform.SetParent(theGamePlayLevelUI.transform, false);

        }
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

    #region
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
