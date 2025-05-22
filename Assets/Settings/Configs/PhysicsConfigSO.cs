using UnityEngine;

[CreateAssetMenu(fileName = "PhysicsConfig", menuName = "Configs/Physics Config")]

public class PhysicsConfig : ScriptableObject
{

    [Header("Check Related")]
    public float theGroundCheckRadius;
    public float theWallCheckRadius;
    public float theForwardCheckRadius;
    public float theBackWallCheckRadius;
    public float theHeadCheckRadius;
    public float raycastLength;

    public float hasGroundDuration;
    public float hasFlooredDuration;

    public float raysCheckStep;
    [Header("ParaChange Related")]

    public float peakSpeed;

    public float normalGravity;
    public float peakGravity;
    public float riseGravity;
    public float risegravityMultiplier ;
    public float fallGravity;
    public float fallGravityMultiplier;
    public float normalDrag;
    public float airDrag;
    public float fallMaxGravity;

    public float hasWallDuration;
    public float hasForwardDuration;
    public float hasRaycastGoundDuration;

}

