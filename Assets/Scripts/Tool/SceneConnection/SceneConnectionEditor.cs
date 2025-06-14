// SceneConnectionEditor.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SceneConnectionEditor : EditorWindow
{

    /* 该编辑器用于在建立场景之间的加载关系，进而在分块加载中实现场景的预加载
     * 
     * 目前实现的功能：
     * 1、新建配置文件、选择配置文件进行编辑；
     * 2、可以在该编辑器页面加载关系的新建、删除，邻居关系的添加、删除；
     * 3、通过拖拽SceneAsset文件实现加载关系的主场景、邻居场景的编辑；
     * 4、通过场景及文本映射缓存、脏标识以及计时降低编辑器资源使用。
     * 
     * 
     */
    private SceneConnectionManager manager;
    private Vector2 scrollPosition;
    private Dictionary<string, SceneAsset> sceneCache = new ();
    private bool isCacheDirty=true;
    private double lastCacheClearTime;
    private const double cacheRefreshInterval = 10.0;

    [MenuItem("Tools/Scene Connection Manager")]
    public static void ShowWindow()
    {
        GetWindow<SceneConnectionEditor>("Scene Connections");
    }

    void OnGUI()
    {
        #region Window中一行
        EditorGUILayout.BeginHorizontal();//WindowUI中一行的起始标志
        manager = (SceneConnectionManager)EditorGUILayout.ObjectField(
            "Connection Manager",
            manager,
            typeof(SceneConnectionManager),
            false
        );

        if (manager == null)
        {
            if (GUILayout.Button("Create New", GUILayout.Width(100)))
            {
                string path = EditorUtility.SaveFilePanelInProject(
                    "Create Scene Connection Manager",
                    "SceneConnectionManager",
                    "asset",
                    "Select save location"
                );
                if (!string.IsNullOrEmpty(path))
                {
                    manager = CreateInstance<SceneConnectionManager>();
                    AssetDatabase.CreateAsset(manager, path);
                    AssetDatabase.SaveAssets();
                }
            }
        }
        else
        {
            if(GUILayout.Button("Exit This", GUILayout.Width(100)))
            {
                manager = null;
            }
        }
        
        EditorGUILayout.EndHorizontal();//WindowUI中一行的结束标志
        if (manager == null) return;
        #endregion
        if (isCacheDirty || EditorApplication.timeSinceStartup - lastCacheClearTime > cacheRefreshInterval)
        {
            Debug.Log("刷新一次");
            RefreshCache();
            isCacheDirty = false;
            lastCacheClearTime = EditorApplication.timeSinceStartup;
        }


        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);//滑条顶部位置，并将滑条置于顶部

        for (int i = 0; i < manager.connections.Count; i++)//主场景的数量
        {
            EditorGUILayout.BeginVertical("Box");

            // 主场景字段

            EditorGUILayout.BeginHorizontal();
            var mainSceneAsset = FindSceneAssetByName(manager.connections[i].mainScene);
            var newMainSceneAsset = (SceneAsset)EditorGUILayout.ObjectField(
                "Main Scene",
                mainSceneAsset,
                typeof(SceneAsset),
                false
            );

            if (newMainSceneAsset != mainSceneAsset)
            {
                manager.connections[i].mainScene = newMainSceneAsset != null ?
                    newMainSceneAsset.name : "";
                isCacheDirty = true;
            }


            if (!string.IsNullOrEmpty(manager.connections[i].mainScene) && newMainSceneAsset == null)
            {
                EditorGUILayout.HelpBox(
                    $"Scene not found: {manager.connections[i].mainScene}",
                    MessageType.Error
                );
            }
            if(manager.connections[i].mainScene != null)
            {
                if (GUILayout.Button("Remove", GUILayout.Width(100)))
                {
                    manager.connections.RemoveAt(i);
                    i--;
                    isCacheDirty = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            // 邻近场景列表
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Neighbor Scenes");
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                manager.connections[i].neighborScenes.Add("");
                isCacheDirty = true;
            }
            EditorGUILayout.EndHorizontal();
            for (int j = 0; j < manager.connections[i].neighborScenes.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                var currentAsset = FindSceneAssetByName(manager.connections[i].neighborScenes[j]);
                var newAsset = (SceneAsset)EditorGUILayout.ObjectField(
                    currentAsset,
                    typeof(SceneAsset),
                    false
                );
                if (currentAsset != newAsset)
                {
                    manager.connections[i].neighborScenes[j] = newAsset != null ?
                        newAsset.name : "";
                    isCacheDirty = true;
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    manager.connections[i].neighborScenes.RemoveAt(j);
                    j--;
                    isCacheDirty = true;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (GUILayout.Button("Add New Connection"))
        {
            manager.connections.Add(new SceneConnectionManager.SceneConnection());
            isCacheDirty = true;
        }

        EditorGUILayout.EndScrollView();//滑条底部位置

        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
        }
    }

    private SceneAsset FindSceneAssetByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            //Debug.Log("场景名称为空");
            return null;
        }

        if(sceneCache.TryGetValue(sceneName ,out var cachedAsset))
        {
            //Debug.Log("缓存有");
            return cachedAsset;
        }

        var guids = AssetDatabase.FindAssets("t:SceneAsset " + sceneName);
        //Debug.Log($"正在查找场景: '{sceneName}'，共找到 {guids.Length} 个场景资产");
        if (guids.Length != 0) 
        {
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SceneAsset checkingAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                if(checkingAsset != null)
                {
                    if(checkingAsset.name == sceneName)
                    {
                        sceneCache[sceneName] = checkingAsset;
                        return checkingAsset;

                    }
                    else
                    {
                        sceneCache[sceneName] = null;//这句的用途是用于在场景重命名后，SceneConnectionManager还没有更新时的检测会浪费大量资源
                        return null;
                    }
                }
            }
            Debug.Log("找完了也不对");
            return null;
        }
        else
        {
            Debug.Log("有问题");
            return null;
        }
    }

    private void RefreshCache()
    {
        sceneCache.Clear();
        for (int i = 0; i < manager.connections.Count; i++)
        {
            if (!sceneCache.ContainsKey(manager.connections[i].mainScene))
            {

                var guids = AssetDatabase.FindAssets("t:SceneAsset " + manager.connections[i].mainScene);
                //Debug.Log($"正在查找场景: '{sceneName}'，共找到 {guids.Length} 个场景资产");
                if (guids.Length != 0)
                {
                    foreach (var guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        SceneAsset checkingAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                        if (checkingAsset != null)
                        {
                            if (checkingAsset.name == manager.connections[i].mainScene)
                            {
                                sceneCache[manager.connections[i].mainScene] = checkingAsset;
                            }
                            else
                            {
                                sceneCache[manager.connections[i].mainScene] = null;//这句的用途是用于在场景重命名后，SceneConnectionManager还没有更新时的检测会浪费大量资源
                            }
                        }
                    }
                    //Debug.Log("找完了也不对");
                }
                else
                {
                    Debug.Log("有问题");
                }
            }
            else
            {
                //Debug.Log("无问题");
            }
            for (int j = 0; j < manager.connections[i].neighborScenes.Count; j++)
            {
                if (!sceneCache.ContainsKey(manager.connections[i].neighborScenes[j]))
                {

                    var guids = AssetDatabase.FindAssets("t:SceneAsset " + manager.connections[i].neighborScenes[j]);
                    //Debug.Log($"正在查找场景: '{sceneName}'，共找到 {guids.Length} 个场景资产");
                    if (guids.Length != 0)
                    {
                        foreach (var guid in guids)
                        {
                            string path = AssetDatabase.GUIDToAssetPath(guid);
                            SceneAsset checkingAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                            if (checkingAsset != null)
                            {
                                if (checkingAsset.name == manager.connections[i].neighborScenes[j])
                                {
                                    sceneCache[manager.connections[i].neighborScenes[j]] = checkingAsset;
                                }
                                else
                                {
                                    sceneCache[manager.connections[i].neighborScenes[j]] = null;//这句的用途是用于在场景重命名后，SceneConnectionManager还没有更新时的检测会浪费大量资源
                                }
                            }
                        }
                        Debug.Log("找完了也不对");
                    }
                    else
                    {
                        Debug.Log("有问题");
                    }
                }
                else
                {
                    //Debug.Log("无问题");
                }
            }
        }
        // Debug.Log("场景缓存已重置");
    }
}
#endif
