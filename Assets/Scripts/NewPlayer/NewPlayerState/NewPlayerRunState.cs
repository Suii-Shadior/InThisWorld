using PlayerInterfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class NewPlayerRunState : NewPlayerState, IMove_horizontally
{
    public NewPlayerRunState(NewPlayerController _player, NewPlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        /* 
         * Work1.�̳�״̬�������߼�
         * Work2.����HorizontalMove�߼�
         * Work3.����֡Can�ж�
         * 
         */
        base.Enter();
        MoveEnter();
        CurrentStateCandoChange();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        //KeepInertiaCount();
        CurrentStateCandoUpdate();
        WhetherExit();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HorizontalMove();
    }

    protected override void CurrentStateCandoChange()
    {

        /* ���������ڽ���һ��״̬��Can�жϽ���仯�������ڽ���֡
         * 
         * Work1.����ʱ��Ȼ�������ҷ���
         * Work2.����ʱ��Ȼ��������λ��
         * Work3.����ʱ��������λ��
         * Work4.����ʱ��Ӧ���ڵ��棬������Ծˢ�£�Ȼ��Can��Ծһ�����ж���true
         */
        base.CurrentStateCandoChange();
        player.canTurnAround = true;
        player.canHorizontalMove = true;
        player.canVerticalMove = false;
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();
    }

    protected override void CurrentStateCandoUpdate()
    {
        /* ���������ڳ����Ľ���Can�жϣ���������ͨ֡
         * 
         * Work1.�ܲ�ʱʱ��ˢ���Ӻ����Ŀ�ˮ��Ŷ�����Can��Ծ
         * Work2.����ʱʱ�̼��Can����ʹ��
         * Work3.����ʱʱ�̼��Can����
         */
        base.CurrentStateCandoUpdate();
        player.RefreshCanJump();
        player.WhetherCanJumpOrWallJump();
        player.WhetherCanItemUse1();

    }
    private void MoveEnter()
    {

        /* 
         * Step1.������ײ��
         * Step2.player�˶���������
         * Step3.�����ż��ٶ�
         */
        player.thisBoxCol.enabled = true;

        player.horizontalMoveSpeedAccleration = player.playerConfig.idle_MoveAccleration;
        player.horizontalMoveSpeedMax = player.playerConfig.idle_MoveSpeedMax;
        player.horizontalmoveThresholdSpeed = player.playerConfig.idle_MoveThresholdSpeed;
        HorizontalMove();
    }



    private void WhetherExit()
    {

        /* 
         * Work1.horizontalMove=>fall
         * Work2.horizontalMove=>idle
         * 
         */
        if (!player.thisPR.IsOnFloored())
        {
            player.thisPR.LeaveFloor();
            player.ChangeToFallState();
            return;
        }
        else if (player.horizontalInputVec == 0)
        {
            //Debug.Log("ͣ��");
            player.ChangeToIdleState();
            return;
        }
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
         *                                    ������С�ڵ����ż��ٶȣ��������뷽�����ż��ٶ��ƶ�//TODO�����������ת��������ʵӦ����ԭ��ͣ������ȴ���ͷת��
         *      �������롪�����ٶȴ����ż��ٶȣ������泯�����Ͻ��м��٣�
         *                  ������С�ڵ����ż��ٶȣ����泯�ٶ���0
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
    #endregion
}
