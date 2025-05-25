using System.Collections;
using UnityEngine;
using SubInteractiveEnum;
using InteractiveAndInteractableEnums;

public class FollowKey : MonoBehaviour
{
    /* �ýű����ڽӴ���Opener_Key�����ɣ�����������ڴ򿪲��ض����ŵ�Կ�׶����߼�
     * 
     * Tip1.���������Ȼ��Խ�����������DontDestroy
     * Tip2.������Ҫ�����Ǹ�����Һʹ��ţ����䲢�����Ӿ���һ����ʱ�����wait״̬(�������У��������ø��ˣ���Ҳ���ÿ���
     * Tip3.��Ϊ���������ֲ�ͬ���࣬��ֻ�ǲ�ͬ�׶Σ��ʲ��ù���
     * TODO����FollowKey���ɺ����Opener����Ϊ���£�����δ��Կ�ױ����ر���Ϸ����������ϷԿ����Զ���ڣ�������Ҫ��һ���򵥵���Ʒϵͳ�����浱ǰ����Դ������ͨ���ؿ�����������������
     */
    #region ���
    private Animator thisAnim;
    #endregion
    #region ����
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
                    //Debug.Log("������");
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
                        //Debug.Log("���������");
                    }
                }
                else
                {
                    //Debug.Log("���������");
                }
                break;
        }
    }



    private void KeyFollowing()
    {
        /* �÷������ڸ���Կ�׸���״̬��λ�õĸ��£���������ͨ֡
         * 
         * Step1.����Ŀ��ص�Ķ�Ӧƫ��������
         * Step2.����Ŀ��ص�
         * Step3.���ű��������ڸ�λ���뵱ǰλ��֮������λ��
         */
        Vector3 offsetVec3 = new Vector3(thisInteractiveConfig.destinalOffset.x * thePlayer.faceDir, thisInteractiveConfig.destinalOffset.y, 0);
        theDestination.position = thePlayer.thisDP.position + offsetVec3;
        transform.position = Vector3.Lerp(transform.position, theDestination.position, thisInteractiveConfig.moveRatio);
    }

    private void KeyOpenning()
    {
        /* �÷������ڸ���Կ�׿���״̬�µ�λ�õĸ����Լ������߼�
         * 
         * Step1.���ű���������Ŀ��λ���뵱ǰλ��֮������λ��
         * Step2.�Ե�ǰλ����Ŀ��λ�õľ�������жϣ����㹻С���ҵ�ǰδ���п��Ŷ���������п��Ŷ������������Ŷ������Ŷ������߼������������Ŷ������������ٱ�����Э�̣�
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
        //��Ϊ������DontDestroy�����Բ���ͨ��ͣ�õķ�ʽ��ʹ������Ϸ�в����ڣ�����ö����һֱ���ڡ�
        yield return new WaitForSeconds(thisInteractiveConfig.destroyDuration);
        Destroy(this.gameObject);
    }



}
