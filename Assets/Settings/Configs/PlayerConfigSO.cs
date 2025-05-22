using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/Player Config")]
public class PlayerConfig : ScriptableObject
{
    #region 基础运动等参数
    [Header("Ground_Movement")]

    public float idle_MoveAccleration;
    public float idle_MoveSpeedMax;
    public float idle_MoveThresholdSpeed;
    [Header("Air_Movement")]
    public float air_MoveAccleration;
    public float air_MoveSpeedMax;
    public float air_MoveThresholdSpeed;

    [Header("Air_Jump")]
    public float normal_JumpForce;
    public float coyoteJumpLength;
    public float jumpBufferLength;
    [Header("Air_Fall")]
    public float air_FallSpeedMax;

    public float hurtUnactDuration;
    public float deadUnactDuration;
    #endregion





    #region 道具相关参数
    [Header("Sword")]

    public float sworduse1_CooldownDuration;
    public float sworduse1_ContinueAttackDuration;
    public float sworduse2_CooldownDuration;
    public Vector2[] attackHitVec2s;

    [Header("umbrella")]
    public float umbrella_CooldownDuration;
    public float umbrellaMoveAccelaration;
    public float umbrellaMoveThresholdSpeed;
    public float umbrellaMoveSpeedMax;
    public float umbrellaFallSpeedMax;
    #endregion
    [Header("uncontrol")]
    //public float knockedBackDuration;
    //public float knockedBackForce;
    public float uncontrolDuration;
    #region 健康相关参数
    [Header("Invinsible")]
    public float invinsibleDuration;
    #endregion
    //[Header("Jump")]
    //public float moveSpeedMax = 8f;
    //public float jumpForce = 12f;
    //public float airmoveSpeedMax = 6f;
    //public float coyoteJumpLength = 0.15f;

    //// 技能参数
    //[Header("Abilities")]
    //public float umbrellaFallSpeedMax = 4f;
    //public float attackCooldownDuration = 0.5f;

    //// 物理参数
    //[Header("Physics")]
    //public float peakSpeed = 0.5f;
    //public float knockedBackForce = 10f;
}