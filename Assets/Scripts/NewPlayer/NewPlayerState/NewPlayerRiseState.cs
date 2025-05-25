using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerRiseState : NewPlayerState, IMove_horizontally,IJump
{
    public NewPlayerRiseState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        /* 
         * Work1.�̳�״̬�������߼�
         * Work2.����Rise�߼�
         * Work3.��Ծ����
         * Work3.����֡Can�ж�
         * 
         */
        base.Enter();
        RiseEnter();
        Jump();
        CurrentStateCandoChange();
    }
    public override void Exit()
    {
        base.Exit();

    }

    public override void Update()
    {
        base.Update();
        CurrentStateCandoUpdate();
        WhetherExit();

    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HorizontalMove();//�����������X���ƶ��ٶ���Ȼ������ͬ�ļ��ٶ�
    }
    protected override void CurrentStateCandoChange()
    {
        /* ���������ڽ���һ��״̬��Can�жϽ���仯�������ڽ���֡
         * 
         * Work1.����Riseʱ������ת��
         * Work2.����Riseʱ������ˮƽ�ƶ������ܴ�ֱ�ƶ�
         * Work3.����Riseʱ���ж�Can��Ծ,��Ҫ�ǵ����п��ܳԵ�һЩ���ߣ�ʹ��ʵ�ֿ���������Ծ����������Ҫ�ж��Ƿ�����Ծ
         * Work4.����Riseʱ������Can����ʹ���ж�
         * Work5.����Riseʱ������Can�����ж�
         * 
         */
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();

    }

    private void RiseEnter()
    {
        /* 
         * Work1.�ָ���ײ��
         * Work3.player��ز����仯
         */
        player.thisBoxCol.enabled = false;

        player.horizontalMoveSpeedAccleration = player.playerConfig.air_MoveAccleration;
        player.horizontalmoveThresholdSpeed = player.playerConfig.air_MoveThresholdSpeed;
        player.horizontalMoveSpeedMax = player.playerConfig.air_MoveSpeedMax;
        player.verticalFallSpeedMax = player.playerConfig.air_FallSpeedMax;
    }

    private void WhetherExit()//
    {
        /* 
         * Work1.rise=>fall
         * Work2.rise=>apex/TODO:����������;�в��ᱻ
         */
        if (player.thisPR.IsHead())
        {
            player.ClearYVelocity();
            player.ChangeToFallState();
        }
        else if (player.thisRB.velocity.y < 0)
        {

            player.ChangeToFallState();
        } 
        //if (player.thisRB.velocity.y < -player.peakSpeed) //Ҫ���peak״̬ʱ����
        //{
        //    player.ChangeToFallState();  
        //}
    }

    protected override void CurrentStateCandoUpdate()
    {
        /* ���������ڳ����Ľ���Can�жϣ���������ͨ֡
         * 
         * Work1,Riseʱ�����Can��Ծ,��Ϊ���ܳԵ�һЩ���ߣ�ʹ��ʵ�ֿ���������Ծ����������Ҫ�ж��Ƿ�����Ծ
         * Work2.Riseʱ,����ʱʱ�̼��Can����ʹ��
         * Work3.Riseʱ,����ʱʱ�̼��Can����
         */
        base.CurrentStateCandoUpdate();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();

    }
    #region �ӿ�ʵ��
    public void HorizontalMove()
    {
        /* ���ӿڷ�������ͨ�������ʵ�ֲ�ͬ״̬�µ�ˮƽλ��
         * 
         * 
         * Step1.�ж��Ƿ���gameplay��.���ǣ�����Step2;����������
         * Step2.�ж��Ƿ�isUncontrol�����ǣ�����Step3-a�����񣬽���Step3-b��
         * Step3-a.
         * Step3-b.�����Ƿ������롢���뷽���Ƿ��뵱ǰ�泯������ͬ����ǰ��ˮƽλ���ٶȽ��в�ͬ���߼��жϡ�
         *      ���泯��������뷽��һ�¡������ٶȴ��ڵ�������ٶȣ������泯������ά������ٶȣ�
         *                                  ���ٶ�С������ٶ��Ҵ����ż��ٶȣ������泯������м��٣�
         *                                  ���ٶ�С���ż��ٶȣ������泯�������ż��ٶ��ƶ���
         *      ���泯��������뷽��һ�¡������ٶȴ����ż��ٶȣ������泯�����Ͻ��м��٣�
         *                                    ������С�ڵ����ż��ٶȣ��������뷽�����ż��ٶ��ƶ�
         *      �������롪�����ٶȴ����ż��ٶȣ������泯�����Ͻ��м��٣�
         *                  ������С�ڵ����ż��ٶȣ����泯�ٶ���0
         * 
         */
        if (player.GetIsGamePlay())
        {
            if (player.GetIsUncontrol())
            {
                //
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
                else
                {
                    if (Mathf.Abs(player.thisRB.velocity.x) < player.horizontalmoveThresholdSpeed || player.thisPR.IsOnWall())
                    {
                        player.ClearXVelocity();
                    }
                    else
                    {
                        player.thisRB.velocity += new Vector2(-player.faceDir * player.horizontalMoveSpeedAccleration, 0f);
                    }
                }
            }
        }
    }
    public void Jump()
    {

        /* ���ӿڷ�������ʵ�ֲ�ͬ״̬�µ���Ծ
         * 
         * Step1.���ӳ�����ʱ��0
         * Step2.��������ٶ�
         * Step3.�ڴ�ֱ����ʩ��������������Ծ//TODO����������ʵ�ַ�ʽ����ʽҪ��
         */
        player.CoyoteCounterZero();
        player.ClearYVelocity();
        player.thisRB.AddForce(Vector2.up * player.playerConfig.normal_JumpForce, ForceMode2D.Impulse);
        //Debug.Log(thisRB.velocity.y);
    }
    #endregion
}
