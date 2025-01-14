using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    #region 组件
    private SpriteRenderer thisSR;
    private PlayerController player;
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
        player = GetComponentInParent<PlayerController>();
        thisAnim = GetComponent<Animator>();

    }
    private void Start()
    {
        thisSR.sortingLayerName = "Player";

    }
    void Update()//当前状态机下没有需要Update的内容
    {
        thisAnim.SetFloat("velocityY", player.thisRB.velocity.y);
    }



    #region 状态机调用
    public void SetVelocityY()
    {
        thisAnim.SetFloat("velocityY", player.thisRB.velocity.y);
    }
    public void DashTrigger()
    {

        thisAnim.SetBool("DashEnd", player.dashEnd);
    }
    public void AttackTrigger()//用于在攻击动作中更新下一个的动作
    {
        thisAnim.SetInteger("attackCounter", player.attackCounter);
    }
    public void StopChipPlay()//用于在逻辑中停止当前动画的播放
    {
        thisAnim.speed = 0f;
    }

    public void ContinueChipPlay()//用于在逻辑中继续当前动画的播放
    {
        thisAnim.speed = 1f;
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
    public void PlayerAnimEnd()//用于在动画中实现直接中止当前状态
    {
        player.StateOver();
    }
    public void DeadAnimEnd()//用于再死亡后实现相关重置
    {
        player.PlayerReset();
    }

    #endregion
}
