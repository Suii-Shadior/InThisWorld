using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    #region ���
    private SpriteRenderer thisSR;
    private NewPlayerController player;
    public Animator thisAnim;
    #endregion
    #region ����
    public GameObject currentAttack;
    public GameObject[] bladeAttackOnGoundIdentity;
    public GameObject axeAttackOnGoundIdentity;
    private GameObject lostAttack;
    public Vector2 moveVec2;
    #endregion

    private void Awake()
    {
        thisSR = GetComponent<SpriteRenderer>();
        player = GetComponentInParent<NewPlayerController>();
        thisAnim = GetComponent<Animator>();

    }
    private void Start()
    {


    }
    void Update()
    {
        //thisAnim.SetFloat("velocityY", player.thisRB.velocity.y);
    }



    #region ״̬������
    //public void SetVelocityY()
    //{
    //    thisAnim.SetFloat("velocityY", player.thisRB.velocity.y);
    //}
    //public void FlipX()
    //{
    //    thisSR.flipX = !thisSR.flipX;
    //}
    //public void FlipY()
    //{
    //    thisSR.flipY = !thisSR.flipY;
    //}
    //public void StopChipPlay()
    //{
    //    �������߼���ֹͣ��ǰ�����Ĳ���
    //    thisAnim.speed = 0f;
    //}
    //public void ContinueChipPlay()
    //{
    //    //�������߼��м�����ǰ�����Ĳ���
    //    thisAnim.speed = 1f;
    //}
    public void SetAttackCounter()
    {
        thisAnim.SetInteger("attackSignal", player.attackSignal);
    }



    public void TBool(string _boolname)//������״̬������˳�ʱ���ж����л�
    {
        thisAnim.SetBool(_boolname, true);
        player.nowState = _boolname;
    }
    public void FBool(string _boolname)
    {
        thisAnim.SetBool(_boolname, false);
    }

    #endregion

    #region �¼�����

    public void DeadAnimEnd()//������������ʵ���������
    {
       
    }

    #endregion

    #region ��������ⲿ����
    public bool isAttackingPlaying()
    {
        if (player.isNewAC)
        {
            return player.thisNewAC.thisAnim.GetCurrentAnimatorStateInfo(1).IsName("Attack.NewDownwardAttack") || player.thisNewAC.thisAnim.GetCurrentAnimatorStateInfo(1).IsName("Attack.NewAltAttack")
            || player.thisNewAC.thisAnim.GetCurrentAnimatorStateInfo(1).IsName("Attack.NewAttack") || player.thisNewAC.thisAnim.GetCurrentAnimatorStateInfo(1).IsName("Attack.NewUpwardAttack");

        }
        else
        {
        return player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).IsName("Attack.NewDownwardAttack") || player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).IsName("Attack.NewAltAttack")
            || player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).IsName("Attack.NewAttack") || player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).IsName("Attack.NewUpwardAttack");

        }
    }
    #endregion


}
