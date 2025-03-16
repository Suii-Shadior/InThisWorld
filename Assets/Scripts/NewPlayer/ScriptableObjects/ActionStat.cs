using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Player Action Stats")]
public class ActionStat : ScriptableObject
{
    [Header("run")]
    [Range(1f, 10f)] public float groundThresholdSpeed = 4f;
    [Range(0.5f, 5f)] public float groundAcceleration = 1f;
    [Range(0.5f, 5f)] public float groundDeceleration = 1f;
    [Range(1f, 10f)] public float groundMaxSpeed = 4f;
    [Range(1f, 10f)] public float airThresholdSpeed = 4f;
    [Range(0.5f, 5f)] public float airAcceleration = 1f;
    [Range(0.5f, 5f)] public float airDeceleration = 1f;
    [Range(1f, 10f)] public float airMaxSpeed = 4f;



    [Header("jump")]
    public float jumpHeight = 6.5f;

    [Range(1f, 1.1f)] public float jumpHeightCompensationFctor = 1.054f;//？？
    [Range(.01f, 5f)] public float gravityOnrelseaseMultiplier = 2f;
    public float fallSpeedMax = 26f;
    [Range(1, 5)] public int jumpAllowed = 2;

    [Range(.02f, .3f)] public float timeForUpwardsCancel = .027f;//在apex时间的长度

    public float timeTillApex = .35f;
    [Range(.5f, 1f)] public float apexThreshold = .097f;
    [Range(.01f, 1f)] public float apexHangTime = .075f;

    [Range(0f, 1f)] public float jumpBufferTime = .125f;
    [Range(0f, 1f)] public float jumpCoyoteTime = .1f;


    [Header("debug")]
    public bool DebugShowIsGroundedBox;
    public bool DebugHeadBumpBox;

    [Header("VIsualization tool")]
    public bool showArc;
    public bool stoopOnCollision;
    public bool drawRight = true;
    [Range(5, 100)] public int ArcResolution = 20;
    [Range(0, 500)] public int VisualizationSteps = 90;




    public float gravity { get; private set; }
    public float initialJumpVelocity { get; private set; }

    public float adjustedJumpHeight{ get;private set; }

    public void OnValidate()
    {
        CalculateValues();
    }
    private void OnEnable()
    {
        CalculateValues();
    }
    private void CalculateValues()
        //抛物线公式=a(x^2)+bx+c 带入x=t b=v0 a =g/2 c=p
    {
        adjustedJumpHeight = jumpHeight * jumpHeightCompensationFctor;
        gravity =  -(2f * adjustedJumpHeight) / Mathf.Pow(timeTillApex, 2f);
        initialJumpVelocity = Mathf.Abs(gravity) * timeTillApex;
    }

}
