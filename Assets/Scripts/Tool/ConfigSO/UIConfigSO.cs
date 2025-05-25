using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "Configs/UI Config")]

public class UIConfig : ScriptableObject
{
    [Header("GameMenu Related")]
    public float gameMenuFadeInDuration;
    public float gameMenuMoveInDuration;
    public float gameMenuFadeOutDuration;
    public float gameMenuMoveOutDuration;
    public Vector2 gameMenuMoveVec;
    public float gameMenuMoveInRatio;
    public float gameMenuMoveOutRatio;
    public float gameMenuOrignalAlpha;
    public float gameMenuFadeInRatio;
    public float gameMenuFadeOutRatio;
    public Vector2 gameMenuHadPlayedOffset;

    [Header("BlackScreen Related")]
    public float blackScreenFadeInDuration;
    public float blackScreenFadeOutDuration;
    public float blackScreenPauseAlpha;

    [Header("SaveData Select Related")]
    public string NOSAVEDATASTR = "ø’¥ÊµµŒª÷√";

}
