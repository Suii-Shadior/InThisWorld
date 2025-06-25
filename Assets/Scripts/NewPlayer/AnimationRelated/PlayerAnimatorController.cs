using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    #region 组件
    private SpriteRenderer thisSR;
    private NewPlayerController player;
    public Animator thisAnim;
    #endregion
    #region 变量
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



    #region 状态机调用
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
    //    用于在逻辑中停止当前动画的播放
    //    thisAnim.speed = 0f;
    //}
    //public void ContinueChipPlay()
    //{
    //    //用于在逻辑中继续当前动画的播放
    //    thisAnim.speed = 1f;
    //}
    public void SetAttackCounter()
    {
        thisAnim.SetInteger("attackSignal", player.attackSignal);
    }



    public void TBool(string _boolname)//用于在状态进入和退出时进行动画切换
    {
        thisAnim.SetBool(_boolname, true);
        player.nowState = _boolname;
    }
    public void FBool(string _boolname)
    {
        thisAnim.SetBool(_boolname, false);
    }

    #endregion

    #region 事件调用

    public void DeadAnimEnd()//用于再死亡后实现相关重置
    {
       
    }

    #endregion

    #region 不合理的外部调用
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
