using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerSwordState_Attack : NewPlayerState,IMove_horizontally, IFall_vertically
{
    public NewPlayerSwordState_Attack(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        /* 
         * Work1.继承状态机进入逻辑
         * Work2.Attack进入逻辑
         * Work3.进入帧Can判断
         * 
         */
        base.Enter();
        AttackEnter();
        CurrentStateCandoChange();

    }

    public override void Exit()
    {
        //Itenuse行为结束才开始计算冷却
        base.Exit();
        AttackExit();
    }

    public override void Update()
    {
        /* 
         * 
         * Work1.普通帧Can判断
         * Work2.Fall逻辑 TODO：其实涉及了物理判断的，应该放在FixedUpdate
         * Work3.对碰撞体的条件检测
         * Work3.退出逻辑检测
         * 
         */
        base.Update();//空内容
        //KeepInertiaCount();
        CurrentStateCandoUpdate();
        Fall();
        BoxColUpdate();
        WhetherExit();

    }


    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HorizontalMove();

    }
    protected override void CurrentStateCandoChange()
    {
        /* 本方法用于进行一次状态下Can判断进入变化，适用于进入帧
         * 
         * Work1.进入Attack时，可以转向
         * Work2.进入Attack时，可以水平移动，不能垂直移动
         * Work3.进入Attack时，根据当前物理环境判断是否刷新延后跳计时，同时判断Can跳跃，
         * 
         */
        base.CurrentStateCandoChange();
        player.canTurnAround = false;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        if (player.thisPR.IsOnFloored())
        {
            player.RefreshCanJump();
        }
        player.WhetherCanJumpOrWallJump();

    }

    protected override void CurrentStateCandoUpdate()
    {
        /* 本方法用于持续的进行Can判断，适用于普通帧
         * 
         * Work1.Attack时，根据当前物理环境判断是否刷新延后跳计时，检测Can跳跃
         */
        base.CurrentStateCandoUpdate();
        if (player.thisPR.IsOnFloored())
        {
            player.RefreshCanJump();
        }
        player.WhetherCanJumpOrWallJump();

    }



    private void AttackEnter()
    {
        //Debug.Log("真按了");
        player.canItemUse1 = false;

    }


    private void WhetherExit()
    {
        /* 
         * Work1.Attack=>normal
         */
        if (player.isNewAC)
        {

        }
        else
        {
            if (player.thisAC.isAttackingPlaying() && player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1 >= .9f)
            {
                //Debug.Log(player.thisAC.thisAnim.GetCurrentAnimatorClipInfo(1)[0].clip.name);
                player.StateOver();
            }
            else
            {
                //Debug.Log(player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1);
                //Debug.Log(player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).shortNameHash);
            }

        }
    }

    #region 小方法
    public void BoxColUpdate()
    {
        if (player.thisRB.velocity.y < 0)
        {

            player.thisBoxCol.enabled = true;
        }
        else
        {
            player.thisBoxCol.enabled = false;

        }
    }
    private void AttackExit()
    {
        player.ItemUse1Counter = player.playerConfig.sworduse1_CooldownDuration;
        player.sword_ContinueAttackCounter = player.playerConfig.sworduse1_ContinueAttackDuration;
    }

    #endregion
    #region 接口实现
    public void HorizontalMove()
    {
        /* 本接口方法用于通过方向键实现不同状态下的水平位移
         * 
         * 
         * Step1.判断是否在gameplay中.若是，进入Step2;若否，无内容
         * Step2.判断是否isUncontrol。若是，进入Step3-a；若否，进入Step3-b；
         * Step3-a.
         * Step3-b.根据是否有输入、输入方向是否与当前面朝方向相同、当前的水平位移速度进行不同的逻辑判断。
         *      若面朝方向和输入方向一致——当速度大于等于最高速度，则在面朝方向上维持最高速度；
         *                                  当速度小于最高速度且大于门槛速度，则在面朝方向进行加速；
         *                                  当速度小于门槛速度，则在面朝方向以门槛速度移动。
         *      若面朝方向和输入方向不一致——当速度大于门槛速度，则在面朝方向上进行减速；
         *                                    当毒素小于等于门槛速度，则以输入方向以门槛速度移动//TODO：如果加入了转身动作，其实应该在原地停留几秒等待镜头转向
         *      若无输入——当速度大于门槛速度，则在面朝方向上进行减速；
         *                  当毒素小于等于门槛速度，则面朝速度置0
         * 
         */
        if (player.GetIsGamePlay())
        {
            if (player.GetIsUncontrol())
            {
                
            }
            else
            {
                if (player.horizontalInputVec != 0)
                {
                    if (player.faceDir != player.horizontalInputVec)
                    {
                        player.ClearXVelocity();
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                    }
                    else
                    {
                        if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalMoveSpeedMax)
                        {
                            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                            {
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                            }
                            else
                            {
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedAccleration, 0f);
                            }
                        }
                        else
                        {
                            player.ClearXVelocity();
                            player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedMax, 0f);
                        }
                    }
                }
                else//无键盘输入时，根据当前速度不同进行减速、停止
                {
                    if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed || player.thisPR.IsOnWall())
                    {
                        //当前速度小于等于启动速度，则停止
                        player.ClearXVelocity();
                    }
                    else
                    {
                        //当前速度大于启动速度，则减速
                        player.thisRB.velocity += new Vector2(-player.faceDir * player.horizontalMoveSpeedAccleration, 0f);
                    }
                }
            }
        }
    }

    public void Fall()
    {        
        /* 本接口方法用于实现不同状态下的自由下落
         * 
         * 判断当前速度是否小于等于下落最大速度。若是，朝下落方向以最大速度移动，若否，无内容（通过重力向下落方向进行加速）
         * TODO：输入重力方向应该怎加速下落速度及最大下落速度。
         * TODO: 物理系统改过之后应该要区分的上升和下降的代码
         */

        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.ClearYVelocity();
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax);
        }
        else if(player.thisRB.velocity.y <0)
        {
            //Debug.Log("??");
        }
    }
    #endregion
}
