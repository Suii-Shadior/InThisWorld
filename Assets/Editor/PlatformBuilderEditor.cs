using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlatformBuilder))]
public class PlatformBuilderEditor : Editor
{
    public PlatformBuilder builder;

    private void OnEnable()
    {
        builder = (PlatformBuilder)target;
    }

    public override void OnInspectorGUI()
    {
        // 绘制默认Inspector
        DrawDefaultInspector();

        EditorGUILayout.Space(10);

        // 绘制模式控制
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("进入绘制模式"))
            {
                builder.isDrawingMode = true;
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("退出绘制模式"))
            {
                builder.isDrawingMode = false;
                builder.ClearPreview();
            }
        }
        EditorGUILayout.EndHorizontal();

        // 操作按钮
        if (builder.isDrawingMode)
        {
            EditorGUILayout.BeginVertical("Box");
            {
                if (GUILayout.Button("生成平台 (B)"))
                {
                    builder.GeneratePlatform();
                }

                if (GUILayout.Button("清空当前绘制 (C)"))
                {
                    builder.ClearPreview();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }

    // 场景视图交互
    private void OnSceneGUI()
    {
        if (!builder.isDrawingMode) return;

        Event e = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(e.mousePosition).origin;

        // 手动计算网格坐标
        Vector2Int gridPos = builder.WorldToGrid(mousePos);

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            if (!builder.coordinates.Contains(gridPos))
            {
                builder.coordinates.Add(gridPos);
                builder.UpdatePreviewVisual();
                e.Use();
            }
        }

        // 强制重绘场景视图
        SceneView.RepaintAll();
    }
}