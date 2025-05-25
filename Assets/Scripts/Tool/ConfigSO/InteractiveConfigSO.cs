using System.Collections;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "Interactive", menuName = "Configs/Interactive Config")]


public class InteractiveConfigSO : ScriptableObject
{
    [Header("FollowKey Related")]
    public float moveRatio;
    public Vector2 destinalOffset;
    public float destroyDuration;
    [HideInInspector]public string USEDSTR = "isUsed";

    [Header("Opener Related")]
    public AnimatorController theButtonAnimator;
    public AnimatorController theDebrisAnimator;
    public AnimatorController theKeyAnimator;
    public GameObject theKeyPrefab;
    public string Opener_PRESSEDSTR = "isPressed";
    public string Opener_UNPRESSEDSTR = "isUnpressed";
    public string Opener_PRESSINGSTR = "isPressing";
    public string Opener_UNPRESSINGSTR = "isUnpressing";
}
