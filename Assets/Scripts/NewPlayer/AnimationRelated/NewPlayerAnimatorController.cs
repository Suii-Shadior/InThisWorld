using UnityEngine;
using System.Collections.Generic;

public class PlayerAnimationController : MonoBehaviour
{
    #region 组件和引用
    private NewPlayerController player;
    private SpriteRenderer spriteRenderer;//作用是什么
    #endregion

    #region 动画数据结构
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
        public float triggerTime; // 0-1之间的比例值
        public bool triggered;
    }

    [System.Serializable]
    public class StateAnimationSet
    {
        public string stateName;
        public List<AnimationDefinition> animations = new List<AnimationDefinition>();
        public List<AnimationEvent> events = new List<AnimationEvent>();//应该时一个动画对象一个表，而不是一个状态一个表
        public float frameRate = 12; // 默认帧率

        // 条件选择函数（可自定义）
        public System.Func<NewPlayerController, int> animationSelector;//？？？？
    }
    #endregion

    #region 配置
    [Header("动画配置")]
    public List<StateAnimationSet> animationSets = new List<StateAnimationSet>();//为什么要单独配置一个？

    #endregion

    #region 运行时变量
    private Dictionary<string, StateAnimationSet> animationSetDict = new Dictionary<string, StateAnimationSet>();
    private AnimationDefinition currentAnimation;
    private float currentAnimationTime;
    private string currentStateName;
    #endregion

    private void Awake()
    {
        player = GetComponentInParent<NewPlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初始化动画集字典
        foreach (var set in animationSets)
        {
            animationSetDict[set.stateName] = set;
        }
    }

    private void Start()
    {
        // 初始状态动画
        if (player != null)
        {
            HandleStateChange(player.CurrentState().ToString());
        }
    }

    private void Update()
    {
        if (currentAnimation != null)
        {
            // 更新动画播放时间
            currentAnimationTime += Time.deltaTime * currentAnimation.playbackSpeed;

            // 处理循环动画
            if (currentAnimation.loop)
            {
                currentAnimationTime %= currentAnimation.clip.length;
            }
            else
            {
                // 非循环动画结束处理
                if (currentAnimationTime >= currentAnimation.clip.length)
                {
                    player.StateOver();
                }
            }

            // 更新精灵帧
            UpdateSpriteFrame();

            // 处理动画事件
            ProcessAnimationEvents();
        }
    }

    // 处理状态变更
    public void HandleStateChange(string newStateName)
    {
        if (currentStateName == newStateName) return;

        currentStateName = newStateName;

        if (animationSetDict.TryGetValue(newStateName, out StateAnimationSet set))
        {
            // 重置事件触发状态
            foreach (var e in set.events)
            {
                e.triggered = false;
            }

            // 选择动画
            AnimationDefinition selectedAnimation = null;

            if (set.animationSelector != null)
            {
                int animationIndex = set.animationSelector(player);
                if (animationIndex >= 0 && animationIndex < set.animations.Count)
                {
                    selectedAnimation = set.animations[animationIndex];
                }
            }

            // 如果没有选择器或选择失败，使用第一个动画
            if (selectedAnimation == null && set.animations.Count > 0)
            {
                selectedAnimation = set.animations[0];
            }

            // 应用新动画
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

    // 更新当前帧的精灵
    private void UpdateSpriteFrame()
    {
        if (currentAnimation != null && currentAnimation.clip != null)
        {
            // 计算当前帧索引
            float normalizedTime = currentAnimationTime / currentAnimation.clip.length;
            int frameCount = currentAnimation.clip.frameCount;
            int frameIndex = Mathf.FloorToInt(normalizedTime * frameCount) % frameCount;

            // 应用精灵
            if (frameIndex < currentAnimation.clip.sprites.Length)
            {
                spriteRenderer.sprite = currentAnimation.clip.sprites[frameIndex];
            }
        }
    }

    // 处理动画事件
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

    // 外部调用：强制更新动画（用于状态内条件变化）
    public void RefreshAnimation()
    {
        HandleStateChange(currentStateName);
    }
}
