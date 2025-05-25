using System.Collections;
using SwitchFactoryRelated;
using UnityEngine;
using InteractiveAndInteractableEnums;
using InteractableInterface;

public class SwitchController : MonoBehaviour, IInteract
{
    #region ���
    private Animator thisAnim;
    public CombineInteractableManager theCombineManager;
    #endregion



    [Header("��������ʹ��˵����������\n1��ѡ�񱾽ű����õĿ������ͣ���thisSwitchType\n2���趨�ö����Ƿ��ʼ״̬is Triggered���Ƿ�����Ҫ����is Primary Switch")]
    [Space(3)]
    #region ����
    [Header("Swtich Setting")]
    public InteractableConfigSO interactableConfigSO;
    public switchTpye thisSwitchType;
    public ISwitch currentSwitch;
    public bool isPrimarySwitch;//ͨ���������ж����ڳ�ʼ���Ķ���Χ�������ظ�����ͻ

    [Header("Switch Info")]
    public bool isTriggered;
    public bool canTriggered;

    [Header("Alternative Related")]//�漰����ֻ�п��ض�Ӧ������״̬�߼�
    [Header("3����Ӹ�λʱ�䣬��canTriggeredDuration\n4����Ӷ��񿪹�״̬��Ӧ����")]

    public float canTriggeredCounter;
    public PlatformController[] triggeredPlatforms;
    public PlatformController[] unTriggeredPlatforms;

    [Header("Autoresetable Related")]//�漰����ֻ��һ��״̬�߼��������߼��п��ܴ��ڶ���ɿؽ��
    [Header("3����Ӹÿ��صĶ�Ӧ����λ�ã���thisElevatorArrivalPoint\n4����ӵ��ݶ���")]
    public Transform thisElevatorArrivalPoint;
    public PlatformController theElevator;




    #endregion

    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        theCombineManager = GetComponentInParent<CombineInteractableManager>();//��Ҫ����ͬ�����switch��״̬
        //����Switch���漰������г�ʼ��������Awake��Ҫ����PlaytformUnit����Start����Platform������Ҳ���г�ʼ�����������������Start֮ǰ
        switch (thisSwitchType)
        {
            case switchTpye.alternative_pair:
                currentSwitch = new PairerFactory().CreateSwitcher(this);
                break;
            case switchTpye.autoresetable_elevator:
                currentSwitch = new ElevatorSwitchFactory().CreateSwitcher(this);
                break;
        }
        currentSwitch?.SceneLoad_Awake();
    }

    void Start()
    {
        //����Switch���������Start���г�ʼ��
        //���񿪹ر���һ��Ҫ�������ó�ʼ���߸��ݴ浵����������Ƕ���ͬ��
        currentSwitch?.SceneLoad_Start();
    }


    void Update()
    {
        currentSwitch?.SceneExist_Updata();
    }




    #region �ӿ����

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController thePlayer))
        {
            SetPlayer(thePlayer);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController thePlayer))
        {

            ClearPlayer(thePlayer);
        }
    }
    public void SetPlayer(NewPlayerController _thePlayer)
    {
        if (_thePlayer.theInteractable == null)
        {
            //Debug.Log("���뽻������");
            _thePlayer.theInteractable = this;
        }
    }

    public void ClearPlayer(NewPlayerController _thePlayer)
    {
        if (_thePlayer.theInteractable == this.GetComponent<IInteract>())
        {
            _thePlayer.theInteractable = null;
            //Debug.Log("�Ƴ���������");
        }
    }
    public void Interact()//���ݲ�ͬ���ͽ��в�ͬ�Ľ�������
    {
        if (canTriggered)
        {
            if (isTriggered)
            {
                currentSwitch?.Interact1();
            }
            else
            {
                currentSwitch?.Interact2();
            }
        }
    }
    #endregion

    #region С�������ⲿ����




    #endregion

    #region ��������ط���
    public void SetAnimTriggered()
    {
        thisAnim.SetBool(interactableConfigSO.Switch_TRIGGEREDSTR, true);
    }

    public void SetAnimUntriggered()
    {
        thisAnim.SetBool(interactableConfigSO.Switch_UNTRIGGEREDSTR, true);
    }

    public void SetAnimTrigger()
    {
        thisAnim.SetTrigger(interactableConfigSO.Switch_TRIGGERINGSTR);
    }


    #endregion
}
