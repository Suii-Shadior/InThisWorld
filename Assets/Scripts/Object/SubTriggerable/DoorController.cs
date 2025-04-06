using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubInteractiveEnum;

public class DoorController : MonoBehaviour
{
    #region 组件和其他
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;

    #endregion



    [Header("————使用说明————\n1、选择适用的门类型，thisDoorType")]
    [Space(3)]
    [Header("——————————Door Setting——————————")]
    public DoorInteractType thisDoorType;
    public bool isOpened;
    [Header("——————————Door Info——————————")]
    [Header("Opener Related")]//通过物品交互打开，其中button指可分开交互保存的开启器，debris指可分开交互但必须一齐保存的开启器，key指可在不同场景中分开交互保存的开启器
    [Header("2、除Key外，添加相应的开门对象\n3、手动添加开门需要数量，即needToOpen")]
    public OpenerController[] Openers;
    public int needToOpen;

    [Header("Eventer Related")]//通过事件打开，其中local指场景内的事件，马上触发相应内容，record指存档内的内容，让相应内容在加载时改变


    [Header("Animator Related")]
    private const string OPENEDSTR = "isOpened";
    private const string CLOSEDSTR = "isClosed";
    private const string OPENNINGSTR = "isOpenning";
    private const string CLOSINGSTR = "isClosing";

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
        SceneLoadSetting_OpenerControllers();//对于Door的涉及Button和Debris对象进行初始化，放在Awake主要是Openner本身在Start进行本身的初始化
    }
    private void Start()
    {
        SceneLoadSetting_DoorItself();
    }
    // Update is called once per frame
    void Update()//也可以用代码更新
    {
        
    }
    #region 初始化相关


    private void SceneLoadSetting_OpenerControllers()
    {
        switch (thisDoorType)
        {
            case DoorInteractType.opener_button:
                Opener_ButtonAwake();//读取存档，更新计数
                break;
            case DoorInteractType.opener_debris://读取存档
                Opener_DebrisAwake();
                break;
            case DoorInteractType.opener_key://读取存档，读取计数
                Opener_KeyAwake();
                break;
        }
    }
    private void Opener_ButtonAwake()
    {
        //读取存档
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            thisAnim.SetBool(OPENEDSTR, true);
            needToOpen = 0;
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = true;
            }
        }
        else
        {
            thisBoxCol.enabled = true;
            thisAnim.SetBool(CLOSEDSTR, true);
            foreach (OpenerController _opener in Openers)
            {
                //查询各自的存档情况,若没有被按下，设其bool，同时needToOpen+1
                
            }
        }

    }
    private void Opener_DebrisAwake()
    {
        //读取存档
        if (isOpened)
        {
            thisBoxCol.enabled = false;
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = true;
            }
        }
        else
        {
            thisBoxCol.enabled = true;
            //计数部分
            needToOpen = Openers.Length;
            foreach (OpenerController _opener in Openers)
            {
                _opener.isPressed = false;
            }
        }

    }
    private void Opener_KeyAwake()
    {
        //读取存档
        if (isOpened)
        {
            thisBoxCol.enabled = false;
        }
        else
        {
            thisBoxCol.enabled = true;
        }
        //读档计数

    }

    private void SceneLoadSetting_DoorItself()//仅仅是动画同步
    {
        if (isOpened)
        {
            thisAnim.SetBool(OPENEDSTR, true);

        }
        else
        {
            thisAnim.SetBool(CLOSEDSTR, true);

        }
    }


    #endregion



    #region 重置相关
    public void ResetThisDoor()//这个要监听触发,目前只有debris有这个需求
    {

        switch (thisDoorType)
        {
            case DoorInteractType.opener_debris:
                Opener_DebtisReset();
                break;
        }
    }

    private void Opener_DebtisReset()
    {
        needToOpen = Openers.Length;
        foreach (OpenerController _opener in Openers)
        {
            _opener.Opener_DebrisReset();
        }
    }

    #endregion


    #region 外部调用


    public void OpenTheDoor()
    {
        needToOpen -= 1;
        if (needToOpen == 0)
        {
            isOpened = true;
            thisBoxCol.enabled = false;
            thisAnim.SetTrigger(OPENNINGSTR);
            //存档
        }
    }



     public void CloseTheDoor()
    {
        isOpened = false;
        thisAnim.SetTrigger(CLOSEDSTR);
        thisBoxCol.enabled = false;
    }
    #endregion
}
