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

    /* ����������Ҫְ��
     * 1����������UI���󣬰���ȫ��UI�ͳ���UI
     * 2��ͳ��ȫ����Ϸ״̬��������ص�����
     * 
     * ע��㣺
     * 1����UI����������ͳ����Ϸ״̬���������ʵ��Խ������ʱ��ְ��Χ������ΪUI����ֱ�۵ı仯���ݶ�ѡ������ͳ�
     * 
     * 
     * 
     */
    #region ���������
    private ControllerManager theCM;
    private DataController theData;
    private NewPlayerController thePlayer;
    private LevelController theLevel;
    private InputController theInput;

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
         * Step1.��ȡ���
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
        theEC = theCM.theEvent;

        theButtons = GetComponent<ButtonLogic>();


    }
    private void OnEnable()
    {

        /* 
         * Step1.����DataController�����Ĳ�����GameMenu��������
         * Step2.ͬʱ��ȡ��ѡ�浵�������ݣ�չʾ��GameSaveDatas������
         * 
         */

    }
    private void Start()
    {
        /* 
         * Step1.���ȵ���
         * Step2.GameMenu����
         * Step3.�������˵�ҳ��
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
        /* ���������ڳ�ʼ�����˵����������ڽű���ʼ����ʱ
         * 
         * Step1.����PlayerPrefs������������չʾ�������ݼ�λ��
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
            //Debug.Log("��һ������");
        }

    }

    private void Update()
    {

        /* 
         * Step1.���뵭��
         * Step2.��������ȫ��UI�����Ϣ�������¼���Ѫ��������
         */
        
        //Step3.����������ȫ��UI��Ϣ�����紥����ʾ

        if(thisUIState == UIWorkState.isGameplaying)
        {
            totalSeconds += Time.deltaTime;
        }
    }









    #region ��ʼ����ط���

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
    #region ����UI���������ͣ�ã��������뵭��


    public void BlackScreen_FadeIn()
    {
        StartCoroutine(BlackScreen_FadeInCo());
    }

    public IEnumerator BlackScreen_FadeInCo()
    {
        blackScreenState = BlackScreenState.isFadingIn;
        //��Я������ʵ�ֻ��浭����Ҳ���Ǻ�Ļ�ĵ��룬��������Ϸ������һЩ����Ĺ��ȿ�ʼ
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
        //��Я������ʵ�ֻ��浭�룬Ҳ���Ǻ�Ļ�ĵ�������������Ϸ������һЩ����Ĺ��Ƚ���
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

    #region ��ʱ���õ�UI���뵭��
    public void GameMenuUIShow()
    {
        StartCoroutine(GameMenuUIShowCo());
    }

    public IEnumerator GameMenuUIShowCo()
    {
        //������Ļ��
        //��ǿ������ʾ
        //���ð�������
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
         * Step1.ͣ�ð�������
         * Step2.����������ʾ
         * Step3.�Ƴ���Ļ
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
        Debug.Log("������Ϸ");
        theInput.GamePlayInput();
    }
    private void GamePlayUIFade()
    {
        StartCoroutine(GamePlayFadeCo());
    }
    private IEnumerator GamePlayFadeCo()
    {
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("�˳���Ϸ");
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

    #region �ڲ�����

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










    #region Buttonͳ���߼�����


    public void FuncCall_GamePlay_Pause()
    {
        /* ������������gameplayʱ��ͣ��Ϸ�������ڰ����߼��������߼�
         * 
         * Step1.��Ļ��ʾ����������ͣ����UI
         * Step2.��ͣʱ�䣬���浱ǰʱ������
         * Step3.�����л�
         */
        GamePlay_Pause();
        thePlayer.SetIsGamePlay(false);
        theLevel.GamePlay_PauseTime();
        theInput.GamepauseInput();
    }
    public void FuncCall_GamePause_Resume()
    {
        /* ������������gamepauseʱ�ָ���Ϸ�������ڰ����߼��������߼�
         * 
         * Step1.��Ļȡ�����������������UI
         * Step2.�ָ���֮ͣǰ����
         * Step3.�����л�
         */
        GamePause_Resume();
        thePlayer.SetIsGamePlay(true);
        theLevel.GamePause_ResumeTime();
        theInput.GamePlayInput();
    }


    public void FuncCall_GamePause_BackToMenu()
    {
        /* ������������gamepauseʱ�ص��˵����棬�����ڰ����߼�
         * 
         * Step.���������й��ܶ���Level��Э���н���
         * 
         * 
         */
        theLevel.BackToMainMenu();
        theInput.GamemenuInput();
    }
    public void FuncCall_GameMenu_GameStart()
    {
        /* ������������gamemenuʱѡ���½���Ϸ��ҳ�棬�����ڰ����߼�
         * 
         * Step1.��PlayerPrefs�м��Ŀǰ�����浵��λ��Ӧ�浵�����ͨ��jsonת���ṹ��Ϊ�ı��ļ���
         * Step2.���ݼ�������¸��浵λ�õ���ʾ����
         * Step3.�����ô浵����UI
         * 
         */
        theData.SaveDataCheck();
        for(int i =0;i< saveDataOverviews.Count; i++)
        {
            if (saveDataOverviews[i].isValid)
            {
                SecondToHour(saveDataOverviews[i].playDuration, out hours, out minutes, out seconds);

                SaveDataSlots[i].playDurationText.text = "����ʱ�䣺" + hours.ToString("D2") + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2") + "\n";
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

        /* ������������savedata����ѡ�����´浵�ĵ���
         * 
         * Step1.�����з���ע�������ȷ�ϰ�����
         * Step2.����ȷ�Ͻ��棬������Ӧ����
         * 
         */
        theEC.RegisterConfirmation(() =>
        {
            //��Ļ����
            //����һ��Ĭ�ϴ浵Ϊ��ǰ�浵��
            //������е��Ѽ������ݣ��Ӹô浵��Ӧ���浵λ�ö�ȡ���ڴ�浵
            //ж�����е����йؿ����Գ�ʼ�ؿ�ΪĿ��ؿ����зֿ���أ���Ϊ�����ϴμ��ذ����ɴ洢���ݵĶ�����������У�����ȫ��ж�ظɾ����ٽ����µķֿ����
            //��ɫ���г�ʼ�趨
            //��ͷ˲��ת��
            //������GameplayUI
            //����gameplay����
            Debug.Log("�����´浵"+ _saveDataIndex);
        }
        );
        theConfirmUI.SetActive(true);
        theConfirmUI.GetComponentInChildren<TextMeshProUGUI>().text = "�Ƿ��ڴ˴����´浵?";
    }

    public void FuncCal_SaveData_Return()
    {
        //���������ڴ�savedata����ص�gamemenu��ҳ�棬�����ڰ����߼�
        saveDataOverviews.Clear();
        GameMenuUIOnly();
    }
    public void FuncCall_Confirm_Ensure()
    {
        //������������savedata����������ȷ��ҳ�棬�����ڰ����߼�
        theEC.ExecutePendingAction();
        //�ر����п�
    }
    public void FuncCall_Confirm_Cancle()
    {
        //������������savedata����������ȷ��ҳ�棬�����ڰ����߼�
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
