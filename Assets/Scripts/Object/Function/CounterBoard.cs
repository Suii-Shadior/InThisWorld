using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CounterBoard : MonoBehaviour
{
    public float timerCounter;
    public float timerDuration;
    public int theNum;
    public int theMinuteTens;
    public int theMinuteOnes;
    public int theSecondTens;
    public int theSecondOnes;
    public int theMirSecondTens;
    public int theMirSecondOnes;
    public bool isCounting;
    private EventController theEC;
    public float canActivateDuration;

    public string localSubscriberChannel_CountStart;
    public string localSubscriberChannel_PuzzleFail;
    public string localSubscriberChannel_PuzzleFinish;
    public string localPublisherChannel_CounterEnd;
    public string localPublisherChannel_PuzzleCanActivateable;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        theEC = ControllerManager.instance.theEvent;
        theEC.OnLocalEvent += OnLocalEventer;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCounting)
        {
            if (timerCounter > 0)
            {
                timerCounter -= Time.deltaTime;
                NumCalculate();

            }
            else
            {
                theEC.LocalEventPublish(localPublisherChannel_CounterEnd);
                PuzzleFailDispalay();
            }
        }
        else
        {
            //Debug.Log("正常等待");
        }
    }

    private void OnLocalEventer(string _localEventerChannel)
    {

        if (_localEventerChannel== localSubscriberChannel_CountStart)
        {
            if (!isCounting)
            {
                isCounting = true;
                timerCounter = timerDuration;
            }

        }
        else if(_localEventerChannel == localSubscriberChannel_PuzzleFail)
        {
            if (isCounting)
            {
                PuzzleFailDispalay();

            }
        }
        else if(_localEventerChannel == localSubscriberChannel_PuzzleFinish)
        {
            if (isCounting)
            {
                PuzzleFinishDisplay();
            }
        }
    }

    private void NumCalculate()
    {
        theMinuteTens = (int)(timerCounter / 600f);
        theMinuteOnes = (int)(timerCounter / 60f- theMinuteTens*10);
        theSecondTens = (int)((timerCounter - (theMinuteTens * 10+ theMinuteOnes)*60f)/10f);
        theSecondOnes = (int)(timerCounter % 10f);
        theMirSecondTens = (int)((timerCounter%1)/ .1f);
        theMirSecondOnes = (int)((timerCounter % .1f)/.01f);
        switch (theNum)
        {
            case 1:

                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 0:
                break;
        }
    }
    private void PuzzleFailDispalay()
    {
        isCounting = false;

        //显示失败
        //携程过一段时间全部置为unatack即可
        StartCoroutine(PuzzleCanActivateCo());


    }
    private void PuzzleFinishDisplay()
    {
        isCounting = false;
        //显示祝贺
        //携程过一段时间全部置为unatack即可
        StartCoroutine(PuzzleCanActivateCo());
    }


    private IEnumerator PuzzleCanActivateCo()
    {
        yield return new WaitForSeconds(canActivateDuration);

    }
}
