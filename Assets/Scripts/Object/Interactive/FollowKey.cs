using System.Collections;
using UnityEngine;
using SubInteractiveEnum;
using InteractiveAndInteractableEnums;

public class FollowKey : MonoBehaviour
{
    /* 该脚本是在接触到Opener_Key后生成，跟随玩家用于打开不特定的门的钥匙对象逻辑
     * 
     * Tip1.因其基本必然跨越多个场景，故DontDestroy
     * Tip2.因其主要作用是跟随玩家和打开门，故其并非像视觉上一样长时间包含wait状态(但是仍有，保留懒得改了），也不用考虑
     * Tip3.因为并不是区分不同种类，而只是不同阶段，故不用工厂
     * TODO：因FollowKey生成后，相关Opener即视为按下，则若未用钥匙保存后关闭游戏，则启动游戏钥匙永远不在，可能需要做一个简单的物品系统来保存当前的资源，或者通过关卡设计来避免这种情况
     */
    #region 组件
    private Animator thisAnim;
    #endregion
    #region 变量
    [Header("Setting")]
    public InteractiveConfigSO thisInteractiveConfig;
    [Header("Info")]
    public bool hasOpened;
    public followKeyState thisKeyState;

    [Header("Move Related")]
    public Transform theDestination;
    private NewPlayerController thePlayer;
    private DoorController theDoor;
    

    #endregion

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        thisAnim = GetComponent<Animator>();
    }

    void Start()
    {
        theDestination.parent = null;
    }

    void Update()
    {
        switch (thisKeyState)
        {
            case followKeyState.following:
                KeyFollowing();
                break;
            case followKeyState.openning:
                KeyOpenning();
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
         
        switch (thisKeyState)
        {
            case followKeyState.waiting:
                if (other.TryGetComponent<NewPlayerController>(out NewPlayerController _thePlayer))
                {
                    thePlayer = _thePlayer;
                    thisKeyState = followKeyState.following;
                }
                else
                {
                    //Debug.Log("有问题");
                }
                break;
            case followKeyState.following:
                if (other.TryGetComponent<DoorController>(out DoorController _theDoor))
                {
                    if (_theDoor.thisDoorType == DoorInteractType.opener_key)
                    {
                        thisKeyState = followKeyState.openning;
                        theDoor = _theDoor;
                        theDestination.position = theDoor.transform.position;
                    }
                    else
                    {
                        //Debug.Log("不是这个门");
                    }
                }
                else
                {
                    //Debug.Log("正常检测中");
                }
                break;
        }
    }



    private void KeyFollowing()
    {
        /* 该方法用于跟随钥匙跟随状态下位置的更新，适用于普通帧
         * 
         * Step1.计算目标地点的对应偏差向量，
         * Step2.计算目标地点
         * Step3.将脚本对象置于该位置与当前位置之间的相对位置
         */
        Vector3 offsetVec3 = new Vector3(thisInteractiveConfig.destinalOffset.x * thePlayer.faceDir, thisInteractiveConfig.destinalOffset.y, 0);
        theDestination.position = thePlayer.thisDP.position + offsetVec3;
        transform.position = Vector3.Lerp(transform.position, theDestination.position, thisInteractiveConfig.moveRatio);
    }

    private void KeyOpenning()
    {
        /* 该方法用于跟随钥匙开门状态下的位置的更新以及开门逻辑
         * 
         * Step1.将脚本对象置于目标位置与当前位置之间的相对位置
         * Step2.对当前位置与目标位置的距离进行判断，若足够小，且当前未进行开门动作，则进行开门动作（包括开门动作、门对象开门逻辑，动画机开门动画，启动销毁本对象协程）
         * 
         */
        transform.position = Vector3.Lerp(transform.position, theDestination.position, thisInteractiveConfig.moveRatio);
        if (Vector2.Distance(transform.position, theDestination.position) < 0.01f && !hasOpened)
        {
            hasOpened = true;
            theDoor.OpenTheDoor();
            thisAnim.SetTrigger(thisInteractiveConfig.USEDSTR);
            StartCoroutine(DestroyCo());

        }
    }

    private IEnumerator DestroyCo()
    {
        //因为本对象DontDestroy，所以不能通过停用的方式来使其在游戏中不存在，否则该对象会一直存在。
        yield return new WaitForSeconds(thisInteractiveConfig.destroyDuration);
        Destroy(this.gameObject);
    }



}
