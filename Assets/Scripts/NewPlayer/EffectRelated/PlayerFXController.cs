using System.Collections;
using UnityEngine;

public class PlayerFXController : MonoBehaviour//应该把特效做成接口
{
    #region 组件
    private SpriteRenderer thisSR;
    private AudioSource thisAS;

    #endregion

    #region 变量
    [Header("Animation FX")]
    [SerializeField] private Material hitMat;
    private Material oriMat;
    [Header("Sound Fx")]
    public AudioClip playingAudio;
    public AudioClip[] FX_Sounds;

    #endregion

    private void Awake()
    {
        thisSR = GetComponentInChildren<SpriteRenderer>();
        thisAS = GetComponentInChildren<AudioSource>();
    }


    private void Start()
    {
        oriMat = thisSR.material;
        //将BGM加入mixer对应组
    }

    public IEnumerator HurtFlashEffect()
    {
        thisSR.material = hitMat;
        yield return new WaitForSeconds(.2f);
        thisSR.material = oriMat;
        yield return new WaitForSeconds(.2f);
        thisSR.material = hitMat;
        yield return new WaitForSeconds(.2f);
        thisSR.material = oriMat;
    }
    private void RedColorInfo()//PlayerFXController.InvokeRepeating("RedColorBlink", _delaytime, _gaptime);
    {
        if (thisSR.color != Color.white) thisSR.color = Color.white;
        else thisSR.color = Color.red;
    }

    private void CancelRedBlink()//PlayerFXController.Invoke("CancelRedBlink", _delaytime);
    {
        CancelInvoke();
        thisSR.color = Color.white;
    }

    private void PlayRelatedSFX(int _soundFX_Index)//需要播放音效的地方调用
    {
        thisAS.Stop();
        playingAudio = FX_Sounds[_soundFX_Index];
        thisAS.clip = playingAudio;
        thisAS.Play();
    }
    #region 外部调用方法
    public void DeadSound()
    {
        PlayRelatedSFX(0);
    }
    public void PlantSound()
    {
        PlayRelatedSFX(2);
    }
    public void ElectricSoundOn()
    {
        thisAS.Stop();
        Debug.Log("真放了");
        playingAudio = FX_Sounds[3];
        thisAS.clip = playingAudio;
        thisAS.loop = true;
        thisAS.Play();
    }
    public void ElectricSoundOff()
    {
        thisAS.Stop();
        thisAS.loop = false;
    }
    public void MushJumpSound()
    {
        PlayRelatedSFX(1);
    }

    #endregion
}
