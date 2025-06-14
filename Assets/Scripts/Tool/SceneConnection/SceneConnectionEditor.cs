// SceneConnectionEditor.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SceneConnectionEditor : EditorWindow
{

    /* �ñ༭�������ڽ�������֮��ļ��ع�ϵ�������ڷֿ������ʵ�ֳ�����Ԥ����
     * 
     * Ŀǰʵ�ֵĹ��ܣ�
     * 1���½������ļ���ѡ�������ļ����б༭��
     * 2�������ڸñ༭��ҳ����ع�ϵ���½���ɾ�����ھӹ�ϵ����ӡ�ɾ����
     * 3��ͨ����קSceneAsset�ļ�ʵ�ּ��ع�ϵ�����������ھӳ����ı༭��
     * 4��ͨ���������ı�ӳ�仺�桢���ʶ�Լ���ʱ���ͱ༭����Դʹ�á�
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
        #region Window��һ��
        EditorGUILayout.BeginHorizontal();//WindowUI��һ�е���ʼ��־
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
        
        EditorGUILayout.EndHorizontal();//WindowUI��һ�еĽ�����־
        if (manager == null) return;
        #endregion
        if (isCacheDirty || EditorApplication.timeSinceStartup - lastCacheClearTime > cacheRefreshInterval)
        {
            Debug.Log("ˢ��һ��");
            RefreshCache();
            isCacheDirty = false;
            lastCacheClearTime = EditorApplication.timeSinceStartup;
        }


        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);//��������λ�ã������������ڶ���

        for (int i = 0; i < manager.connections.Count; i++)//������������
        {
            EditorGUILayout.BeginVertical("Box");

            // �������ֶ�

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

            // �ڽ������б�
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

        EditorGUILayout.EndScrollView();//�����ײ�λ��

        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
        }
    }

    private SceneAsset FindSceneAssetByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            //Debug.Log("��������Ϊ��");
            return null;
        }

        if(sceneCache.TryGetValue(sceneName ,out var cachedAsset))
        {
            //Debug.Log("������");
            return cachedAsset;
        }

        var guids = AssetDatabase.FindAssets("t:SceneAsset " + sceneName);
        //Debug.Log($"���ڲ��ҳ���: '{sceneName}'�����ҵ� {guids.Length} �������ʲ�");
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
                        sceneCache[sceneName] = null;//������;�������ڳ�����������SceneConnectionManager��û�и���ʱ�ļ����˷Ѵ�����Դ
                        return null;
                    }
                }
            }
            Debug.Log("������Ҳ����");
            return null;
        }
        else
        {
            Debug.Log("������");
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
                //Debug.Log($"���ڲ��ҳ���: '{sceneName}'�����ҵ� {guids.Length} �������ʲ�");
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
                                sceneCache[manager.connections[i].mainScene] = null;//������;�������ڳ�����������SceneConnectionManager��û�и���ʱ�ļ����˷Ѵ�����Դ
                            }
                        }
                    }
                    //Debug.Log("������Ҳ����");
                }
                else
                {
                    Debug.Log("������");
                }
            }
            else
            {
                //Debug.Log("������");
            }
            for (int j = 0; j < manager.connections[i].neighborScenes.Count; j++)
            {
                if (!sceneCache.ContainsKey(manager.connections[i].neighborScenes[j]))
                {

                    var guids = AssetDatabase.FindAssets("t:SceneAsset " + manager.connections[i].neighborScenes[j]);
                    //Debug.Log($"���ڲ��ҳ���: '{sceneName}'�����ҵ� {guids.Length} �������ʲ�");
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
                                    sceneCache[manager.connections[i].neighborScenes[j]] = null;//������;�������ڳ�����������SceneConnectionManager��û�и���ʱ�ļ����˷Ѵ�����Դ
                                }
                            }
                        }
                        Debug.Log("������Ҳ����");
                    }
                    else
                    {
                        Debug.Log("������");
                    }
                }
                else
                {
                    //Debug.Log("������");
                }
            }
        }
        // Debug.Log("��������������");
    }
}
#endif
