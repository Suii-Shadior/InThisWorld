using UnityEngine;
using System.Collections.Generic;

public class PlayerAnimationController : MonoBehaviour
{
    #region ���������
    private NewPlayerController player;
    private SpriteRenderer spriteRenderer;//������ʲô
    #endregion

    #region �������ݽṹ
    [System.Serializable]
    public class AnimationDefinition
    {
        public string stateName;
        public AnimationClip clip;
        public bool loop = true;
        public float playbackSpeed = 1f;//?
        public Vector2 offset;
    }

    [System.Serializable]
    public class AnimationEvent
    {
        public string eventName;
        public float triggerTime; // 0-1֮��ı���ֵ
        public bool triggered;
    }

    [System.Serializable]
    public class StateAnimationSet
    {
        public string stateName;
        public List<AnimationDefinition> animations = new List<AnimationDefinition>();
        public List<AnimationEvent> events = new List<AnimationEvent>();//Ӧ��ʱһ����������һ����������һ��״̬һ����
        public float frameRate = 12; // Ĭ��֡��

        // ����ѡ���������Զ��壩
        public System.Func<NewPlayerController, int> animationSelector;//��������
    }
    #endregion

    #region ����
    [Header("��������")]
    public List<StateAnimationSet> animationSets = new List<StateAnimationSet>();//ΪʲôҪ��������һ����

    #endregion

    #region ����ʱ����
    private Dictionary<string, StateAnimationSet> animationSetDict = new Dictionary<string, StateAnimationSet>();
    private AnimationDefinition currentAnimation;
    private float currentAnimationTime;
    private string currentStateName;
    #endregion

    private void Awake()
    {
        player = GetComponentInParent<NewPlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ��ʼ���������ֵ�
        foreach (var set in animationSets)
        {
            animationSetDict[set.stateName] = set;
        }
    }

    private void Start()
    {
        // ��ʼ״̬����
        if (player != null)
        {
            HandleStateChange(player.CurrentState().ToString());
        }
    }

    private void Update()
    {
        if (currentAnimation != null)
        {
            // ���¶�������ʱ��
            currentAnimationTime += Time.deltaTime * currentAnimation.playbackSpeed;

            // ����ѭ������
            if (currentAnimation.loop)
            {
                currentAnimationTime %= currentAnimation.clip.length;
            }
            else
            {
                // ��ѭ��������������
                if (currentAnimationTime >= currentAnimation.clip.length)
                {
                    player.StateOver();
                }
            }

            // ���¾���֡
            UpdateSpriteFrame();

            // �������¼�
            ProcessAnimationEvents();
        }
    }

    // ����״̬���
    public void HandleStateChange(string newStateName)
    {
        if (currentStateName == newStateName) return;

        currentStateName = newStateName;

        if (animationSetDict.TryGetValue(newStateName, out StateAnimationSet set))
        {
            // �����¼�����״̬
            foreach (var e in set.events)
            {
                e.triggered = false;
            }

            // ѡ�񶯻�
            AnimationDefinition selectedAnimation = null;

            if (set.animationSelector != null)
            {
                int animationIndex = set.animationSelector(player);
                if (animationIndex >= 0 && animationIndex < set.animations.Count)
                {
                    selectedAnimation = set.animations[animationIndex];
                }
            }

            // ���û��ѡ������ѡ��ʧ�ܣ�ʹ�õ�һ������
            if (selectedAnimation == null && set.animations.Count > 0)
            {
                selectedAnimation = set.animations[0];
            }

            // Ӧ���¶���
            if (selectedAnimation != null)
            {
                currentAnimation = selectedAnimation;
                currentAnimationTime = 0f;
                UpdateSpriteFrame();
            }
        }
        else
        {
            Debug.LogWarning($"No animation set found for state: {newStateName}");
        }
    }

    // ���µ�ǰ֡�ľ���
    private void UpdateSpriteFrame()
    {
        if (currentAnimation != null && currentAnimation.clip != null)
        {
            // ���㵱ǰ֡����
            float normalizedTime = currentAnimationTime / currentAnimation.clip.length;
            int frameCount = currentAnimation.clip.frameCount;
            int frameIndex = Mathf.FloorToInt(normalizedTime * frameCount) % frameCount;

            // Ӧ�þ���
            if (frameIndex < currentAnimation.clip.sprites.Length)
            {
                spriteRenderer.sprite = currentAnimation.clip.sprites[frameIndex];
            }
        }
    }

    // �������¼�
    private void ProcessAnimationEvents()
    {
        if (animationSetDict.TryGetValue(currentStateName, out StateAnimationSet set))
        {
            float normalizedTime = currentAnimationTime / currentAnimation.clip.length;

            foreach (var e in set.events)
            {
                if (!e.triggered && normalizedTime >= e.triggerTime)
                {
                    e.triggered = true;
                    player.OnAnimationEvent(e.eventName);
                }
            }
        }
    }

    // �ⲿ���ã�ǿ�Ƹ��¶���������״̬�������仯��
    public void RefreshAnimation()
    {
        HandleStateChange(currentStateName);
    }
}
