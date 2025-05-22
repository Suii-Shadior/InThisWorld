using System.Collections;
using UnityEngine;
using SubInteractiveEnum;

public class FollowKey : MonoBehaviour
{
    #region 组件
    private Animator thisAnim;
    #endregion
    #region 变量
    [Header("State Related")]
    public bool isFollowing;
    public bool isOpening;
    public bool hasOpened;

    [Header("Move Related")]
    public Transform theDestination;
    private NewPlayerController thePlayer;
    private DoorController theDoor;
    public Vector2 destinalOffset;
    public float moveRatio;
    #endregion
    #region 常量
    private const string USEDSTR = "isUsed";
    #endregion

    private void Awake()
    {
        //要单独dont destroy
        thisAnim = GetComponent<Animator>();
    }

    void Start()
    {
        theDestination.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.Lerp(transform.position, theDestination.position, moveRatio);
            if (Vector2.Distance(transform.position, theDestination.position) < 0.01f&& !hasOpened)
            {
                hasOpened = true;
                theDoor.OpenTheDoor();
                thisAnim.SetTrigger(USEDSTR);
                StartCoroutine(DestroyCo());

            }
        }
        else if (isFollowing)
        {
            Vector3 offsetVec3 = new Vector3(destinalOffset.x * thePlayer.faceDir, destinalOffset.y, 0);
            theDestination.position = thePlayer.thisDP.position + offsetVec3;
            transform.position = Vector3.Lerp(transform.position, theDestination.position, moveRatio);

        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFollowing)
        {
            if (other.GetComponent<NewPlayerController>())
            {
                isFollowing = true;
                isOpening = false;
                thePlayer = other.GetComponent<NewPlayerController>();
            }
            else
            {
                Debug.Log("有问题");
            }

        }
        else if (!isOpening)
        {
            if (other.GetComponent<DoorController>())
            {
                if (other.GetComponent<DoorController>().thisDoorType == DoorInteractType.opener_key)
                {
                    isOpening = true;
                    isFollowing = false;
                    theDoor = other.GetComponent<DoorController>();
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
        }
        else
        {
            Debug.Log("有问题");
        }
    }



    private IEnumerator DestroyCo()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("摧毁");
        Destroy(this.gameObject);
    }



}
