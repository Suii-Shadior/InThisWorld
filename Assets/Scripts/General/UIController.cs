using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CMRelatedEnum;
using System.Collections;
using StructForSaveData;
using Unity.VisualScripting;

public class UIController : MonoBehaviour
{
    #region 组件及其他
    private ControllerManager theCM;
    private DataController theData;
    private NewPlayerController thePlayer;
    private LevelController theLevel;
    private InputController theInput;
    private DialogeController theDC;//用于触发对话
    private EventController theEC;
    private ButtonLogic theButtons;
    [SerializeField] public UIConfig UIConfig;
    #endregion
    [Header("UI State")]
    [SerializeField]public UIWorkState thisUIState;
    [Header("Setting")]
    public GameObject theGameplayUI;
    public GameObject theGameMenuUI;
    public GameObject thePauseUI;
    public GameObject theGameSettingUI;
    public GameObject theGameSaveDatasUI;
    public GameObject theConfirmUI;
    //[SerializeField] private GameObject theDialogeUI;

    [Header("SceneLoading Related")]

    private bool hadPlayedGame;
    public List<SaveDataOverview> saveDataOverviews = new();
    private int hours, minutes, seconds;
    public List<SaveDataSlotEntry> SaveDataSlots;

    [Header("BlackScreen Related")]
    public Image theBlackScreen;
    public BlackScreenState blackScreenState;
    public float fadeInRatio;
    public float fadeOutRatio;
    public bool isFadeIn;
    public bool isFadeOut;

    [Header("General")]
    public GameObject theCurrentButton;
    public Image theCurrentButtonIcon;

    public float FadeCounter;


    [Header("Overview Related")]
    public float totalSeconds;



    private void Awake()
    {
        /* 
         * Step1.获取组件
         * 
         * 
         * 
         * 
        */
        theCM = GetComponentInParent<ControllerManager>();
        theData = theCM.theData;
        theLevel = theCM.theLevel;
        thePlayer = theCM.thePlayer;
        theInput = theCM.theInput;
        theDC = theCM.theDC;
        theEC = theCM.theEvent;

        theButtons = GetComponent<ButtonLogic>();


    }
    private void OnEnable()
    {

        /* 
         * Step1.根据DataController传来的参数对GameMenu进行设置
         * Step2.同时获取可选存档基本数据，展示在GameSaveDatas界面中
         * 
         */

    }
    private void Start()
    {
        /* 
         * Step1.过度淡出
         * Step2.GameMenu淡入
         * Step3.启动主菜单页面
         * 
         * 
         * 
         */
        //

        SceneLoad_Mainmenu();
        UIObjectOriginalSetting(); 

    }


    private void SceneLoad_Mainmenu()
    {
        /* 本方法用于初始化主菜单对象，适用于脚本开始运行时
         * 
         * Step1.根据PlayerPrefs传递内容设置展示按键内容及位置
         * Step2.
         * 
         * 
         */
        if (hadPlayedGame)
        {
            theButtons.theStartButton.SetActive(false);
            theButtons.theContinueButton.transform.position += (Vector3)UIConfig.gameMenuHadPlayedOffset;
            theButtons.theQuitButton.transform.position += (Vector3)UIConfig.gameMenuHadPlayedOffset;
        }
        else
        {
            //Debug.Log("第一次游玩");
        }

    }

    private void Update()
    {

        /* 
         * Step1.淡入淡出
         * Step2.更新其他全局UI相关信息，例如事件，血量，分数
         */
        
        //Step3.更新其他非全局UI信息，例如触发提示

        if(thisUIState == UIWorkState.isGameplaying)
        {
            totalSeconds += Time.deltaTime;
        }
    }









    #region 初始化相关方法

    public void GetPlayerPrefsFromData(bool _hadPlayedGame)
    {
        hadPlayedGame = _hadPlayedGame;
    
    }
    








    public void UIObjectOriginalSetting()
    {
        /* 
         * 
         */

        thisUIState = UIWorkState.isGameMenuing;

        theGameMenuUI.SetActive(true);
        thePauseUI.SetActive(false);
        theGameplayUI.SetActive(false);
        theGameSettingUI.SetActive(false);
        theGameSaveDatasUI.SetActive(false);


    }


    #endregion
    #region 各种UI对象的启用停用（包括淡入淡出


    public void BlackScreen_FadeIn()
    {
        StartCoroutine(BlackScreen_FadeInCo());
    }

    public IEnumerator BlackScreen_FadeInCo()
    {
        blackScreenState = BlackScreenState.isFadingIn;
        //本携程用于实现画面淡出，也就是黑幕的淡入，适用于游戏结束和一些特殊的过度开始
        float fadeInCounter = UIConfig.blackScreenFadeInDuration;
        float nowAlpha = 1- (fadeInCounter / UIConfig.blackScreenFadeInDuration);
        while (fadeInCounter > 0)
        {
            theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, nowAlpha);
            fadeInCounter -= (Time.timeScale == 0)?Time.unscaledDeltaTime:Time.deltaTime;
            nowAlpha = 1 - (fadeInCounter / UIConfig.blackScreenFadeInDuration);
            yield return new WaitForSecondsRealtime((Time.timeScale==0)?Time.unscaledDeltaTime:Time.deltaTime);
        }
        theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, 1f);
        blackScreenState = BlackScreenState.isBlackScreening;
    }


    public IEnumerator BlackScreen_FadeOutCo()
    {
        blackScreenState = BlackScreenState.isFadingOut;
        //本携程用于实现画面淡入，也就是黑幕的淡出，适用于游戏启动和一些特殊的过度结束
        float fadeOutCounter = UIConfig.blackScreenFadeOutDuration;
        float nowAlpha = fadeOutCounter / UIConfig.blackScreenFadeOutDuration;
        while (fadeOutCounter > 0)
        {
            theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, nowAlpha);
            fadeOutCounter -=( Time.timeScale == 0)?Time.unscaledDeltaTime:Time.deltaTime;
            nowAlpha = fadeOutCounter / UIConfig.blackScreenFadeOutDuration;
            yield return new WaitForSecondsRealtime((Time.timeScale == 0) ? Time.unscaledDeltaTime : Time.deltaTime);
        }
        theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, 0f);
        blackScreenState = BlackScreenState.isFadedOut;
    }

    #region 暂时不用的UI淡入淡出
    public void GameMenuUIShow()
    {
        StartCoroutine(GameMenuUIShowCo());
    }

    public IEnumerator GameMenuUIShowCo()
    {
        //移入屏幕内
        //加强按键显示
        //启用按键功能
        float FadeCounter = UIConfig.gameMenuFadeInDuration;
        float MoveCounter = UIConfig.gameMenuMoveInDuration;
        Vector2 destinationPos = (Vector2)theGameMenuUI.transform.position + UIConfig.gameMenuMoveVec;
        Image[] ButtonImages = theGameMenuUI.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] ButtonTexts = theGameMenuUI.GetComponentsInChildren<TextMeshProUGUI>();
        while (MoveCounter > 0)
        {
            theGameMenuUI.transform.position = Vector2.Lerp(theGameMenuUI.transform.position, destinationPos, UIConfig.gameMenuMoveInRatio);
            MoveCounter -= Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        theGameMenuUI.transform.position = destinationPos;

        float nowAlpha = Mathf.Lerp(UIConfig.gameMenuOrignalAlpha, 1, UIConfig.gameMenuFadeInRatio);
        while (FadeCounter > 0)
        {
            foreach(Image ButtonImage in ButtonImages)
            {
                ButtonImage.color = new Vector4(ButtonImage.color.r, ButtonImage.color.g, ButtonImage.color.b, nowAlpha);
            }
            foreach(TextMeshProUGUI ButtonText in ButtonTexts)
            {
                ButtonText.color = new Vector4(ButtonText.color.r, ButtonText.color.g, ButtonText.color.b, nowAlpha);
            }
            nowAlpha = Mathf.Lerp(nowAlpha, 1, UIConfig.gameMenuFadeInRatio);
            FadeCounter -= Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        foreach (Image ButtonImage in ButtonImages)
        {
            ButtonImage.color = new Vector4(ButtonImage.color.r, ButtonImage.color.g, ButtonImage.color.b, 1);
        }
        foreach (TextMeshProUGUI ButtonText in ButtonTexts)
        {
            ButtonText.color = new Vector4(ButtonText.color.r, ButtonText.color.g, ButtonText.color.b, 1);
        }
    }




    public void GameMenuUIFade()
    {
        StartCoroutine(GameMenuUIFadeCo());
    }

    private IEnumerator GameMenuUIFadeCo()
    {
        /* 
         * 
         * Step1.停用按键功能
         * Step2.减弱按键提示
         * Step3.移出屏幕
         * 
         */
        float FadeCounter = UIConfig.gameMenuFadeOutDuration;
        float MoveCounter = UIConfig.gameMenuMoveOutDuration;
        Vector2 destinationPos = (Vector2)theGameMenuUI.transform.position - UIConfig.gameMenuMoveVec;
        Image[] ButtonImages = theGameMenuUI.GetComponentsInChildren<Image>();
        TextMeshProUGUI[] ButtonTexts = theGameMenuUI.GetComponentsInChildren<TextMeshProUGUI>();
        float nowAlpha = Mathf.SmoothStep(1, UIConfig.gameMenuOrignalAlpha, UIConfig.gameMenuFadeInRatio);
        while (FadeCounter > 0)
        {
            foreach (Image ButtonImage in ButtonImages)
            {
                ButtonImage.color = new Vector4(ButtonImage.color.r, ButtonImage.color.g, ButtonImage.color.b, nowAlpha);
            }
            foreach (TextMeshProUGUI ButtonText in ButtonTexts)
            {
                ButtonText.color = new Vector4(ButtonText.color.r, ButtonText.color.g, ButtonText.color.b, nowAlpha);
            }
            nowAlpha = Mathf.SmoothStep(nowAlpha, UIConfig.gameMenuOrignalAlpha, UIConfig.gameMenuFadeOutRatio);
            FadeCounter -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        foreach (Image ButtonImage in ButtonImages)
        {
            ButtonImage.color = new Vector4(ButtonImage.color.r, ButtonImage.color.g, ButtonImage.color.b, UIConfig.gameMenuOrignalAlpha);
        }
        foreach (TextMeshProUGUI ButtonText in ButtonTexts)
        {
            ButtonText.color = new Vector4(ButtonText.color.r, ButtonText.color.g, ButtonText.color.b, UIConfig.gameMenuOrignalAlpha);
        }


        while (MoveCounter > 0)
        {
            theGameMenuUI.transform.position = Vector2.Lerp(theGameMenuUI.transform.position, destinationPos, UIConfig.gameMenuMoveOutRatio);
            MoveCounter -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        theGameMenuUI.transform.position = destinationPos;
    }


    private void GamePlayShow()
    {
        StartCoroutine(GamePlayShowCo());
    }
    private IEnumerator GamePlayShowCo()
    {
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("进入游戏");
        theInput.GamePlayInput();
    }
    private void GamePlayUIFade()
    {
        StartCoroutine(GamePlayFadeCo());
    }
    private IEnumerator GamePlayFadeCo()
    {
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("退出游戏");
    }
    #endregion

    public void GameMenuUIOnly()
    {
        thisUIState = UIWorkState.isGameMenuing;
        theGameMenuUI.SetActive(true);
        thePauseUI.SetActive(false);
        theGameplayUI.SetActive(false);
        theGameSettingUI.SetActive(false);
        theGameSaveDatasUI.SetActive(false);

    }


    private void GamePauseUIOnly()
    {

        thisUIState = UIWorkState.isGamePausing;

        theGameMenuUI.SetActive(false);
        thePauseUI.SetActive(true);
        theGameplayUI.SetActive(false);
        theGameSettingUI.SetActive(false);
        theGameSaveDatasUI.SetActive(false);
    }

    private void GamePlayUIOnly()
    {
        thisUIState = UIWorkState.isGameplaying;
        theGameMenuUI.SetActive(false);
        thePauseUI.SetActive(false);
        theGameplayUI.SetActive(true);
        theGameSettingUI.SetActive(false);
        theGameSaveDatasUI.SetActive(false);
    }

    private void SaveDataUIOnly()
    {
        theGameMenuUI.SetActive(false);
        thePauseUI.SetActive(false);
        theGameplayUI.SetActive(false);
        theGameSettingUI.SetActive(false);
        theGameSaveDatasUI.SetActive(true);
    }

    public void SettingUIOnly()
    {
        theGameMenuUI.SetActive(false);
        thePauseUI.SetActive(false);
        theGameplayUI.SetActive(false);
        theGameSettingUI.SetActive(true);
        theGameSaveDatasUI.SetActive(false);
    }






    #endregion

    #region 内部调用

    private void GamePlay_Pause()
    {
        GamePauseUIOnly();
        theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, UIConfig.blackScreenPauseAlpha);
        blackScreenState = BlackScreenState.isPausing;
    }

    private void GamePause_Resume()
    {

        GamePlayUIOnly();
        theBlackScreen.color = new Color(theBlackScreen.color.r, theBlackScreen.color.g, theBlackScreen.color.b, 0f);
        blackScreenState = BlackScreenState.isFadedOut;
    }
    #endregion










    #region Button统筹逻辑调用


    public void FuncCall_GamePlay_Pause()
    {
        /* 本方法用于在gameplay时暂停游戏，适用于按键逻辑与输入逻辑
         * 
         * Step1.黑幕显示，仅启用暂停界面UI
         * Step2.暂停时间，保存当前时间速率
         * Step3.输入切换
         */
        GamePlay_Pause();
        thePlayer.SetIsGamePlay(false);
        theLevel.GamePlay_PauseTime();
        theInput.GamepauseInput();
    }
    public void FuncCall_GamePause_Resume()
    {
        /* 本方法用于在gamepause时恢复游戏，适用于按键逻辑与输入逻辑
         * 
         * Step1.黑幕取消，仅启用游玩界面UI
         * Step2.恢复暂停之前速率
         * Step3.输入切换
         */
        GamePause_Resume();
        thePlayer.SetIsGamePlay(true);
        theLevel.GamePause_ResumeTime();
        theInput.GamePlayInput();
    }


    public void FuncCall_GamePause_BackToMenu()
    {
        /* 本方法用于在gamepause时回到菜单界面，适用于按键逻辑
         * 
         * Step.基本上所有功能都在Level的协程中进行
         * 
         * 
         */
        theLevel.BackToMainMenu();
        theInput.GamemenuInput();
    }
    public void FuncCall_GameMenu_GameStart()
    {
        /* 本方法用于在gamemenu时选择新建游戏的页面，适用于按键逻辑
         * 
         * Step1.在PlayerPrefs中检查目前各个存档槽位对应存档情况（通过json转换结构体为文本文件）
         * Step2.根据检查结果更新各存档位置的显示内容
         * Step3.仅启用存档界面UI
         * 
         */
        theData.SaveDataCheck();
        for(int i =0;i< saveDataOverviews.Count; i++)
        {
            if (saveDataOverviews[i].isValid)
            {
                SecondToHour(saveDataOverviews[i].playDuration, out hours, out minutes, out seconds);

                SaveDataSlots[i].playDurationText.text = "游玩时间：" + hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2") + "\n";
                if (saveDataOverviews[i].gameHasFinished)
                {
                    SaveDataSlots[i].gameCompleteDegreeText.text = saveDataOverviews[i].gameCompleteDegree.ToString() + "%";
                }
                if (saveDataOverviews[i].attackAbility)
                {
                    SaveDataSlots[i].skillIcons[0].color = Color.white;
                }
                if (saveDataOverviews[i].umbrellaAbility)
                {
                    SaveDataSlots[i].skillIcons[1].color = Color.white;
                }
                if (saveDataOverviews[i].candleAbility)
                {
                    SaveDataSlots[i].skillIcons[2].color = Color.white;
                }
            }
            else
            {
                SaveDataSlots[i].playDurationText.text = UIConfig.NOSAVEDATASTR;
                SaveDataSlots[i].noSaveDataImage.SetActive(true);
            }
        }
        SaveDataUIOnly();
    }

    public void FuncCall_SaveData_NewGame(string _saveDataIndex)
    {

        /* 本方法用于在savedata界面选择建立新存档的调用
         * 
         * Step1.拟运行方法注册进后续确认按键中
         * Step2.启用确认界面，设置相应内容
         * 
         */
        theEC.RegisterConfirmation(() =>
        {
            //黑幕淡入
            //复制一个默认存档为当前存档号
            //清空所有的已加载内容，从该存档对应外存存档位置读取进内存存档
            //卸载所有的已有关卡，以初始关卡为目标关卡进行分块加载（因为害怕上次加载包含可存储内容的对象会留在其中，所以全部卸载干净后再进行新的分块加载
            //角色进行初始设定
            //镜头瞬间转换
            //仅启用GameplayUI
            //启用gameplay输入
            Debug.Log("创建新存档"+ _saveDataIndex);
        }
        );
        theConfirmUI.SetActive(true);
        theConfirmUI.GetComponentInChildren<TextMeshProUGUI>().text = "是否在此创建新存档?";
    }

    public void FuncCal_SaveData_Return()
    {
        //本方法用于从savedata界面回到gamemenu的页面，适用于案件逻辑
        saveDataOverviews.Clear();
        GameMenuUIOnly();
    }
    public void FuncCall_Confirm_Ensure()
    {
        //本方法用于在savedata界面引出的确认页面，适用于案件逻辑
        theEC.ExecutePendingAction();
        //关闭所有框
    }
    public void FuncCall_Confirm_Cancle()
    {
        //本方法用于在savedata界面引出的确认页面，适用于案件逻辑
        theEC.CancleConfirmation();
        theConfirmUI.SetActive(false);
    }

    public void FuncCall_GameMenu_Continue()
    {
        GamePlayUIOnly();
        theInput.GamePlayInput();
    }

    private void SecondToHour(int totalSecondDutarion,out int hours_, out int minutes_,out int seconds_)
    {
        hours_ = totalSecondDutarion / 3600;
        minutes_ = (totalSecondDutarion % 3600) / 60;
        seconds_ = totalSecondDutarion % 60;
    }
    #endregion

}
