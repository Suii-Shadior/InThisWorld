using UnityEngine;

[CreateAssetMenu(fileName = "Interactable", menuName = "Configs/Interactable Config")]

public class InteractableConfigSO : ScriptableObject
{
    [Header("Handler Related")]

    [Header("Switch Related")]
    public float canTriggeredDuration;
    public string Switch_TRIGGEREDSTR = "isTriggered";
    public string Switch_UNTRIGGEREDSTR = "isUntriggered";
    public string Switch_TRIGGERINGSTR = "isTriggering";
    public string Switch_UNTRIGGERINGSTR = "isUntriggering";
}
