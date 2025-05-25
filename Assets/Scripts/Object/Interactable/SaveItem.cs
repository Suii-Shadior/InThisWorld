using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerInterfaces;
using InteractableInterface;

public class SaveItem : MonoBehaviour,IInteract
{
    private DataController theData;
    private LevelController theLevel;
    private NewPlayerController thePlayer;
    public Transform thisLoadingPos;


    private void Start()
    {
        theData = ControllerManager.instance.theData;
        theLevel = ControllerManager.instance.theLevel;
        thePlayer = ControllerManager.instance.thePlayer;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController _thePlayer))
        {
            SetPlayer(_thePlayer);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<NewPlayerController>(out NewPlayerController _thePlayer))
        {
            ClearPlayer(_thePlayer);
        }
    }

    #region �ӿ����
    public void Interact()
    {
        Debug.Log("��ʼ����");
        thePlayer.InteractRelated_SaveItem(thisLoadingPos.position);
        theData.SaveByJson();
        theData.SetSaveDataRelationsByPlayerPrefs();
    }

    public void SetPlayer(NewPlayerController _thePlayer)
    {
        if (_thePlayer.theInteractable == null)
        {
            //Debug.Log("���뽻������");
            _thePlayer.theInteractable = this;
            thePlayer = _thePlayer;
        }
    }

    public void ClearPlayer(NewPlayerController _thePlayer)
    {
        if (_thePlayer.theInteractable == this.GetComponent<IInteract>())
        {
            _thePlayer.theInteractable = null;
            thePlayer = null;
            //Debug.Log("�Ƴ���������");
        }
    }
    #endregion
}
