using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;



public class DialogeController : MonoBehaviour
{
    private ControllerManager theCM;
    private UIController theUI;
    private InputController theInput;

    [Header("")]
    public Image Speaker;
    public Image TimoBack;
    public Sprite Timo_Body;
    public Sprite Timo_Smile;
    public Sprite Timo_Laugh;
    public Sprite Timo_Frustrate;
    public Sprite Timo_Angry;
    public Sprite Timo_Sturn;
    public Sprite Timo_Frown;
    public Sprite Heimerdinger;
    public Text wordsLabel;
    public TextAsset[] textFiles;
    public List<string> textList = new List<string>();
    public string currentText;
    public int printingIndex;


    [Header("bool")]
    public bool isDialogue;
    public bool isPrinting;
    public float printGap;
    public float autoNextSentenceCounter;
    public float autoNextSentenceDuration;
    private Coroutine printCor;



    private void Awake()
    {
        theCM = GetComponentInParent<ControllerManager>();
        theUI = theCM.theUI;
        theInput = theCM.theInput;
    }

    private void Update()
    {
        if (isDialogue && !isPrinting && autoNextSentenceCounter > 0) autoNextSentenceCounter -= Time.deltaTime;
        else if (isDialogue && !isPrinting && autoNextSentenceCounter < 0) theInput.DialoguingInput();
    }


    public void SetUpNewDialogue(TextAsset currentFile)
    {
        //wordsLabel.rectTransform.position = new Vector3(130f, 210f, 0f);
        wordsLabel.text = "";
        isDialogue = true;
        GetTextFromFile(currentFile);
        printingIndex = 0;
        currentText = textList[printingIndex];
        theUI.TurnOnDialogCanvas();
        Time.timeScale = 0.1f;
        printGap = 0.01f;
        autoNextSentenceCounter = autoNextSentenceDuration;
        printCor = StartCoroutine(PrintLetterCo());
    }
    private void GetTextFromFile(TextAsset currentFile)
    {
        textList.Clear();
        textList = currentFile.text.Split('\n', System.StringSplitOptions.RemoveEmptyEntries).ToList<string>();


        //foreach (var line in lineData)
        //{
        //    textList.Add(line);
        //}
    }
    private void SpeakerDisplay(string characterName)
    {
        //Debug.Log("判断了吗？");
        switch (characterName)
        {
            case "微笑提莫\r":
                TimoBack.gameObject.SetActive(true);
                Speaker.gameObject.SetActive(true);
                Speaker.sprite = Timo_Smile;
                TimoBack.sprite = Timo_Body;
                wordsLabel.alignment = TextAnchor.UpperLeft;
                currentText = textList[++printingIndex];
                break;
            case "大笑提莫\r":
                TimoBack.gameObject.SetActive(true);
                Speaker.gameObject.SetActive(true);
                Speaker.sprite = Timo_Laugh;
                TimoBack.sprite = Timo_Body;
                wordsLabel.alignment = TextAnchor.UpperLeft;
                currentText = textList[++printingIndex];
                break;
            case "生气提莫\r":
                TimoBack.gameObject.SetActive(true);
                Speaker.gameObject.SetActive(true);
                Speaker.sprite = Timo_Angry;
                TimoBack.sprite = Timo_Body;
                wordsLabel.alignment = TextAnchor.UpperLeft;
                currentText = textList[++printingIndex];
                break;
            case "沮丧提莫\r":
                TimoBack.gameObject.SetActive(true);
                Speaker.gameObject.SetActive(true);
                Speaker.sprite = Timo_Frustrate;
                TimoBack.sprite = Timo_Body;
                wordsLabel.alignment = TextAnchor.UpperLeft;
                currentText = textList[++printingIndex];
                break;
            case "皱眉提莫\r":
                TimoBack.gameObject.SetActive(true);
                Speaker.gameObject.SetActive(true);
                Speaker.sprite = Timo_Frown;
                TimoBack.sprite = Timo_Body;
                wordsLabel.alignment = TextAnchor.UpperLeft;
                currentText = textList[++printingIndex];
                break;
            case "严肃提莫\r":
                TimoBack.gameObject.SetActive(true);
                Speaker.gameObject.SetActive(true);
                Speaker.sprite = Timo_Sturn;
                TimoBack.sprite = Timo_Body;
                wordsLabel.alignment = TextAnchor.UpperLeft;
                currentText = textList[++printingIndex];
                break;
            case "大头\r":
                TimoBack.gameObject.SetActive(false);
                Speaker.gameObject.SetActive(true);
                Speaker.sprite = Heimerdinger;
                TimoBack.sprite = Timo_Body;
                wordsLabel.alignment = TextAnchor.UpperLeft;
                currentText = textList[++printingIndex];
                break;
            case "提示\r":
                //wordsLabel.rectTransform.position = new Vector3(0f, 210f, 0f);
                printGap = 0.005f;
                autoNextSentenceDuration = .4f;
                Speaker.gameObject.SetActive(false);
                TimoBack.gameObject.SetActive(false);
                wordsLabel.alignment = TextAnchor.MiddleCenter;
                currentText = textList[++printingIndex];
                break;
            default:
                Debug.Log("不是头像？");
                break;
        }

    }
    public void QuickPrint()
    {
        wordsLabel.text = currentText;
        isPrinting = false;
        StopCoroutine(printCor);
    }
    public void NextSentence()
    {
        if (printingIndex == textList.Count - 1)
        {
            currentText = "";
            CloseDialogue();
        }
        else
        {
            wordsLabel.text = "";
            currentText = textList[++printingIndex];
            //Debug.Log(textList.Count);
            //SpeakerDisplay();
            isPrinting = true;
            printCor = StartCoroutine(PrintLetterCo());
            autoNextSentenceCounter = autoNextSentenceDuration;

        }
        //waitTime = _waitTime;
        //switch textList[printingIndex]
        //{
        //    case "提莫\r":
        //        SpeakerDisplay("提莫", waitTime);
        //        printingIndex++;
        //        StartCoroutine(PrintLetterCo());
        //        break;
        //    case "大头\r":
        //        SpeakerDisplay("大头", waitTime);
        //        printingIndex++;
        //        StartCoroutine(PrintLetterCo());
        //        break;
        //    deflaut:
        //        StartCoroutine(PrintLetterCo());
        //        break;
        //}
    }
    private IEnumerator PrintLetterCo()
    {
        isPrinting = true;
        SpeakerDisplay(currentText);
        for (int i = 0; i < currentText.Length; i++)
        {
            wordsLabel.text += currentText[i];
            yield return new WaitForSeconds(printGap);
        }
        if (wordsLabel.text == currentText)
        {
            isPrinting = false;
        }

    }

    public void CloseDialogue()
    {
        Debug.Log("一号？");
        Time.timeScale = 1f;
        printGap = 0.2f;
        autoNextSentenceDuration = .1f;
        isDialogue = false;
        theUI.TurnOffDialogCanvas();
    }

}
