using UnityEngine;

[CreateAssetMenu(fileName = "AttackableConfig", menuName = "Configs/Attackable Config")]

public class AttackableConfigSO : ScriptableObject
{
    [Header("Blocker Related")]
    public float Blocker_canBeAttackableDuration;
    [HideInInspector]public string Blocker_ISTRIGGEREDSTR = "isTriggered";
    [HideInInspector] public string Blocker_ISUNTRIGGEREDSTR = "isUntriggered";
    [HideInInspector] public string Blocker_ATTACKINGSTR = "isAttacking";

    [Header("Leveler Related")]
    public float Leveler_canBeInteractedDuration;
    [HideInInspector] public string Leveler_ISINTERACTINGSTR = "isInteracting";
    [HideInInspector] public string Leveler_ISALTINTERACTINGSTR = "isAltInteracting";
    [HideInInspector] public string Leveler_CANBEINTERACTED = "canBeInteracted";
    
    [Header("Nailer Related")]
    [HideInInspector] public string Nailer_ISTRIGGEREDSTR = "isTriggered";
    [HideInInspector] public string Nailer_ISUNTRIGGEREDSTR = "isUntriggered";
    [HideInInspector] public string Nailer_ATTACKINGSTR = "isAttacking";
}
