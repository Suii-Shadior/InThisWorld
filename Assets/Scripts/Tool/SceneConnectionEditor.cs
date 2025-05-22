// SceneConnectionEditor.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

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
        #region Window中一行
        EditorGUILayout.BeginHorizontal();//WindowUI中一行的起始标志
        manager = (SceneConnectionManager)EditorGUILayout.ObjectField(
            "Connection Manager",
            manager,
            typeof(SceneConnectionManager),
            false
        );

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
        EditorGUILayout.EndHorizontal();//WindowUI中一行的结束标志
        #endregion

        if (manager == null) return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);//滑条顶部位置，并将滑条置于顶部

        for (int i = 0; i < manager.connections.Count; i++)//主场景的数量
        {
            EditorGUILayout.BeginVertical("Box");

            // 主场景字段
            manager.connections[i].mainScene = EditorGUILayout.TextField(
                "Main Scene",
                manager.connections[i].mainScene
            );

            // 邻近场景列表
            EditorGUILayout.LabelField("Neighbor Scenes");
            for (int j = 0; j < manager.connections[i].neighborScenes.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                manager.connections[i].neighborScenes[j] = EditorGUILayout.TextField(
                    manager.connections[i].neighborScenes[j]
                );
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

            // 删除按钮
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

        EditorGUILayout.EndScrollView();//滑条底部位置

        if (GUI.changed)
        {
            EditorUtility.SetDirty(manager);
        }
    }
}
#endif
