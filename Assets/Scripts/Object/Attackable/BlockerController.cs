using AttackableInterfaces;
using BlockerFactoryRelated;
using InteractiveAndInteractableEnums;
using IPhysicalAttackableFactoryRelated;
using OtherEnum;
using StructForSaveData;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(EdgeCollider2D))]
public class BlockerController : MonoBehaviour,ISave<BlockerSaveData>
{
    #region 组件
    private SpriteRenderer thisSR;
    private EdgeCollider2D thisEdgeCol;
    private Animator thisAnim;
    private BoxCollider2D thisBoxCol;
    private EventController theEC;
    #endregion

    [Header("Blocker Setting")]
    public AttackableConfigSO attackConfigSO;
    public blockerType thisBlockerType;
    public BlockerFactory thisFactory;

    public int numToTriggered;//一般都是特定对象用，用于一些可被还原的对象
    [Header("Blocker Info")]
    [SerializeField]private IPhysicalAttackable currentBlocker;
    public bool isTriggered;
    public int needToTriggered;
    public bool hasAttacked;
    public float canBeAttackableCounter;//攻击计时
    public Vector2 canBeAttackedVec2;

    [Header("Blocker Related")]
    [Header("1、先将子对象的SR对象的大小调整至需要大小\n2、将父对象放到对应位置\n3、选择可攻击方向，即canAttackDir")]
    public BeAttackedDir canAttackDir;
    public BlockerSaveData thisBlockerSaveData;




    private void Awake()
    {
        thisEdgeCol = GetComponent<EdgeCollider2D>();
        thisSR = GetComponentInChildren<SpriteRenderer>();
        thisAnim = GetComponentInChildren<Animator>();
        thisBoxCol = GetComponent<BoxCollider2D>();
        switch (thisBlockerType)
        {
            case blockerType.attackble_stoner:
                //Debug.Log("运行了吗");
                thisFactory = new StonerFactory();
                break;
            default:
                break;
        }
        currentBlocker = thisFactory.CreateBlocker(this);
    }


    private void OnEnable()
    {
        theEC = ControllerManager.instance.theEvent;
        SceneLoadSetting_Itself();
        SceneLoadSetting_Relative();

    }

    private void Start()
    {
        SceneLoadSetting_Related();
    }

    public void SceneLoadSetting_Itself()
    {
        ISaveable thisSaveable = this.GetComponent<ISaveable>();
        RegisterSaveable(thisSaveable);
        isTriggered = thisBlockerSaveData.isTriggered;
        needToTriggered = thisBlockerSaveData.needToTriggered;
        if (isTriggered)
        {
            thisBoxCol.enabled = false;
            thisEdgeCol.enabled = false;
            thisAnim.SetBool(attackConfigSO.Blocker_ISTRIGGEREDSTR, true);
        }
        else
        {

            //needToTriggered = numToTriggered;
            thisBoxCol.enabled = true;
            thisEdgeCol.enabled = true;
            thisAnim.SetBool(attackConfigSO.Blocker_ISUNTRIGGEREDSTR, true);

        }
    }
    public void SceneLoadSetting_Relative()
    {

    }

    public void SceneLoadSetting_Related()
    {
        if (!hasAttacked)
        {
            thisBoxCol.size = thisSR.size;
            List<Vector2> theLocation = new List<Vector2>();
            switch (canAttackDir)
            {
                case BeAttackedDir.leftwardDir:
                    thisEdgeCol.offset = new Vector2(-thisBoxCol.size.x / 2, 0);
                    theLocation.Add(new Vector2(0f, thisBoxCol.size.y / 2));
                    theLocation.Add(new Vector2(0f, -thisBoxCol.size.y / 2));
                    canBeAttackedVec2 = new Vector2(1, 0);
                    break;
                case BeAttackedDir.rightwardDir:
                    thisEdgeCol.offset = new Vector2(thisBoxCol.size.x / 2, 0);
                    theLocation.Add(new Vector2(0f, thisBoxCol.size.y / 2));
                    theLocation.Add(new Vector2(0f, -thisBoxCol.size.y / 2));
                    canBeAttackedVec2 = new Vector2(-1, 0);
                    break;
                case BeAttackedDir.upwardDir:
                    thisEdgeCol.offset = new Vector2(0, thisBoxCol.size.y / 2);
                    theLocation.Add(new Vector2(-thisBoxCol.size.x / 2, 0f));
                    theLocation.Add(new Vector2(thisBoxCol.size.x / 2, 0f));
                    canBeAttackedVec2 = new Vector2(0, -1);

                    break;
                case BeAttackedDir.downwardDir:
                    thisEdgeCol.offset = new Vector2(0, -thisBoxCol.size.y / 2);
                    theLocation.Add(new Vector2(-thisBoxCol.size.x / 2, 0f));
                    theLocation.Add(new Vector2(thisBoxCol.size.x / 2, 0f));
                    canBeAttackedVec2 = new Vector2(0,1);
                    break;
            }
            thisEdgeCol.points = theLocation.ToArray();

        }
        else
        {
            thisEdgeCol.enabled = false;
            //this.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        if (canBeAttackableCounter > 0)
        {
            canBeAttackableCounter -= Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        ISaveable _saveable = GetComponent<ISaveable>();
        SaveDataSync();
        theEC.SaveableUnregisterPublish(_saveable);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if( other.TryGetComponent<AttackArea>(out AttackArea attackArea))
        {
            if (canBeAttackableCounter <= 0)
            {
                currentBlocker?. BePhysicalAttacked(attackArea);
            }
        }
    }


    #region 小方法
    public void AnimAttacking()
    {
        thisAnim.SetTrigger(attackConfigSO.Blocker_ATTACKINGSTR);
    }

    public void AnimTriggered()
    {
        thisAnim.SetBool(attackConfigSO.Blocker_ISTRIGGEREDSTR, true);
    }

    public void CollidersEnable()
    {
        thisBoxCol.enabled = false;
        thisEdgeCol.enabled = false;
    }
    #endregion


    #region ISave接口相关
    public BlockerSaveData GetSpecificDataByISaveable(SaveData _passedData)
    {
        BlockerSaveData thedata = JsonUtility.FromJson<BlockerSaveData>(_passedData.value);
        //Debug.Log(thedata.isOpened);
        return thedata;
    }

    public string GetISaveID()
    {
        return thisBlockerSaveData.SaveableID;
    }

    public void RegisterSaveable(ISaveable _saveable)
    {
        theEC.SaveableRegisterPublish(_saveable);
    }

    public void UnregisterSaveable(ISaveable _saveable)
    {
        theEC.SaveableUnregisterPublish(_saveable);
    }

    public void LoadSpecificData(SaveData _passingData)
    {
        thisBlockerSaveData.CopyData(GetSpecificDataByISaveable(_passingData));
    }

    public SaveData SaveDatalizeSpecificData()
    {
        SaveData _savedata = new()
        {
            key = thisBlockerSaveData.SaveableID,
            type = TypeRegistry.GetDataTypeFromReverseDict(typeof(BlockerSaveData)),
            value = JsonUtility.ToJson(thisBlockerSaveData)
        };
        return _savedata;
    }

    public void SaveDataSync()
    {
        thisBlockerSaveData.isTriggered = isTriggered;
        thisBlockerSaveData.needToTriggered = needToTriggered;
    }


    #endregion
}
