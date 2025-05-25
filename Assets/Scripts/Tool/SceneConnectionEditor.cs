// SceneConnectionEditor.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using System.Linq;

public class SceneConnectionEditor : EditorWindow
{
    private SceneConnectionManager manager;
    private Vector2 scrollPosition;

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

        if (manager == null) return;
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
        EditorGUILayout.EndHorizontal();//WindowUI��һ�еĽ�����־
        #endregion

        if (manager == null) return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);//��������λ�ã������������ڶ���

        for (int i = 0; i < manager.connections.Count; i++)//������������
        {
            EditorGUILayout.BeginVertical("Box");

            // �������ֶ�
            manager.connections[i].mainScene = EditorGUILayout.TextField(
                "Main Scene",
                manager.connections[i].mainScene
            );

            var mainSceneAsset = FindSceneAssetByName(manager.connections[i].mainScene);
            var newMainSceneAsset = (SceneAsset)EditorGUILayout.ObjectField(
                "Main Scene",
                mainSceneAsset,
                typeof(SceneAsset),
                false
            );
            if (newMainSceneAsset != mainSceneAsset)
            {
                manager.connections[i].mainScene = newMainSceneAsset ? newMainSceneAsset.name : "";
            }




            // �ڽ������б�
            EditorGUILayout.LabelField("Neighbor Scenes");
            for (int j = 0; j < manager.connections[i].neighborScenes.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();


                manager.connections[i].neighborScenes[j] = EditorGUILayout.TextField(
                    manager.connections[i].neighborScenes[j]
                );

                var currentAsset = FindSceneAssetByName(manager.connections[i].neighborScenes[j]);
                var newAsset = (SceneAsset)EditorGUILayout.ObjectField(
                    currentAsset,
                    typeof(SceneAsset),
                    false
                );

                if (newAsset != currentAsset)
                {
                    manager.connections[i].neighborScenes[j] = newAsset ? newAsset.name : "";
                }







                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    manager.connections[i].neighborScenes.RemoveAt(j);
                    j--;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Neighbor"))
            {
                manager.connections[i].neighborScenes.Add("");
            }

            // ɾ����ť
            if (GUILayout.Button("Remove This Connection"))
            {
                manager.connections.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        if (GUILayout.Button("Add New Connection"))
        {
            manager.connections.Add(new SceneConnectionManager.SceneConnection());
        }

        EditorGUILayout.EndScrollView();//�����ײ�λ��

        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
        }
    }

    private SceneAsset FindSceneAssetByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return null;

        var guids = AssetDatabase.FindAssets("t:SceneAsset " + sceneName);
        if (guids.Length == 0) return null;

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
    }
}
#endif
