using AttackableInterfaces;
using InteractiveAndInteractableEnums;
using IPhysicalAttackableFactoryRelated;
using UnityEngine;

public class LevelerController : MonoBehaviour
{
    #region ���

    private Animator thisAnim;
    public CombineInteractableManager theCombineManager { get; private set; }
    #endregion



    [Header("��������ʹ��˵����������\n1��ѡ�񱾽ű����õĲ��ݸ����ͣ���thisLevelerType")]
    [Space(3)]
    #region ����
    [Header("Leveler Setting")]
    public AttackableConfigSO attackConfigSO;
    public levelerType thisLevelerType;
    public LevelerFactory thisFactory;

    [Header("Leveler Info")]
    public bool isInteracted;
    public bool canBeInteracted;
    private IPhysicalAttackable currentLeveler;

    [Header("Rotater Related")]//�漰����Ϊ������תƽ̨������˳ʱ����ת����ʱ����ת��ֹͣ����״̬
    [Header("2�������תƽ̨���󣬼�rotatePlatforms\n3����Ӹ�λʱ��")]
    public PlatformController[] rotatePlatforms;
    private float canBeInteractedCounter;



    [Header("Elevator Related")]//�漰����Ϊ���ݣ������������½���ֹͣ����״̬
    [Header("2����ӵ��ݶ���elevatorPlatform\n3������������Ϊ���ݵ��Ӷ���")]
    public PlatformController elevatorPlatform;




    #endregion


    #region ��ʼ�����
    private void Awake()
    {
        thisAnim = GetComponent<Animator>();
        theCombineManager = GetComponentInParent<CombineInteractableManager>();//��ȡ������Ҫ��ʵ�ֶ����֮���ͬ��
        switch (thisLevelerType)
        {
            case levelerType.attackable_rotater:
                thisFactory = new RotaterFactory();
                break;
            case levelerType.attackable_elevator:

                thisFactory = new ElevatorCallerFactory();
                break;
            default:
                break;

        }
        currentLeveler = thisFactory?.CreateLeveler(this);
    }
    private void Start()//Leveler��ʼֻ�ڳ�ʼ״̬�����ý����κγ�ʼ��
    {

    }



    #endregion

    #region Update���
    private void Update()//Leveler��ʹ�����˶���ܿ츴λ����������������Ӧ����������ű�������
    {
        if (!canBeInteracted)
        {
            if (canBeInteractedCounter > 0)
            {
                canBeInteractedCounter -= Time.deltaTime;
            }
            else
            {
                canBeInteracted = true;
                thisAnim.SetBool(attackConfigSO.Leveler_CANBEINTERACTED, true);
            }
        }
        else
        {
            //Debug.Log("�����ȴ�");
        }
    }

    #endregion

    #region Interact���
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<AttackArea>(out AttackArea attackArea))
        {
            currentLeveler?.BePhysicalAttacked(attackArea);

        }
    }



    #endregion


    #region С�������ⲿ����


    public void JustInteract()
    {
        thisAnim.SetTrigger(attackConfigSO.Leveler_ISINTERACTINGSTR);
        SetInteracted();
    }

    public void JustAltInteract()
    {
        thisAnim.SetBool(attackConfigSO.Leveler_CANBEINTERACTED, false);
        SetInteracted();
    }

    private void SetInteracted()
    {
        thisAnim.SetTrigger(attackConfigSO.Leveler_ISALTINTERACTINGSTR);
        canBeInteracted = false;
        canBeInteractedCounter = attackConfigSO.Leveler_canBeInteractedDuration;
    }

    #endregion
}
