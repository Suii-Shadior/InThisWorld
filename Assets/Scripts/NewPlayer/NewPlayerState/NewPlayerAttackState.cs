using MoveInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerAttackState : NewPlayerState,IMove_horizontally, IFall_vertically
{
    public NewPlayerAttackState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        CurrentStateCandoChange();
        AttackEnter();

    }

    public override void Exit()
    {
        base.Exit();
        AttackExit();
    }

    public override void Update()
    {
        base.Update();
        //KeepInertiaCount();//先注释掉，可以改
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
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        //才跳跃，操作逻辑上来看不可能马上又能跳跃的，所以并不刷新操作相关的脱台跳时间
        //但是有可能吃到一些道具，使得实现可以无限跳跃，所以依旧要判断是否能跳跃
        player.WhetherCanJumpOrWallJump();

    }

    protected override void CurrentStateCandoUpdate()
    {
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanAttack();

    }



    private void AttackEnter()
    {
        //Debug.Log("真按了");
        player.canAttack = false;

    }


    private void WhetherExit()
    {
        if (player.thisAC.isAttackingPlaying() && player.thisAC.thisAnim.GetCurrentAnimatorStateInfo(1).normalizedTime%1 >= .9f)
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
    private void AttackExit()
    {
        if (player.attackCounter == 1)
        {
            player.continueAttackCounter = player.continueAttackDuration;
        }
        else
        {
            player.continueAttackCounter = 0;
        }
        player.attackCooldownCounter = player.attackCooldownDuration;
    }
    public void HorizontalMove()
    {
        if (player.isGameplay)
        {
            if (player.isUncontrol)//受限移动时候的移动
            {
                //
            }
            else//非限制移动时的移动
            {
                if (player.horizontalInputVec != 0)//有键盘输入时
                {
                    if (player.faceDir != player.horizontalInputVec)//输入与人物朝向反向时，直接消除原先的速度，朝输入方向开始移动
                    {
                        player.ClearXVelocity();
                        player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                    }
                    else//输入与人物朝向同向时，根据当前速度不同进行启动、加速、满速移动
                    {
                        if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalMoveSpeedMax)
                        {
                            if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed)
                            {
                                //当前速度小于启动速度，则以启动速度移动
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalmoveThresholdSpeed, 0f);
                            }
                            else
                            {
                                //当前速度大于启动速度小于满速，则加速移动
                                player.thisRB.velocity += new Vector2(player.horizontalInputVec * player.horizontalMoveSpeedAccleration, 0f);
                            }
                        }
                        else
                        {
                            //当前速度超过满速，则满速前进
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

    public void Fall()
    {
        if (player.thisRB.velocity.y < -player.verticalFallSpeedMax)
        {
            player.ClearYVelocity();
            player.thisRB.velocity += new Vector2(0, -player.verticalFallSpeedMax);
        }
        else
        {
            //Debug.Log("??");
        }
    }

}
