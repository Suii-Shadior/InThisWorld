// 静态数据生成器

using StructForSaveData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DefaultSaveDataGenerator : EditorWindow
{
    #region 数据结构及其他
    private struct SaveDataEntry
    {
        public string id;
        public StructForSaveData.DataTypes dataType;
        public string jsonData;
        public bool hasError;
        public bool isExpanded;

    }
    private Texture2D CreateColorTexture(Color color)
    {
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, color);
        tex.Apply();
        return tex;
    }
    #endregion
    #region 变量
    private int selectedTab = 0;
    private bool RelativeAddressMode = true;
    [Header("第一选项卡")]
    private Vector2 fileListScrollPos;
    //private bool pendingTab;
    private string newFileName = "NewSave";//默认为NewSave
    private List<string> existingSaves = new List<string>();
    private int selectedFileIndex = -1;
    [Header("第二选项卡")]
    private Vector2 scrollPos;
    private TextAsset selectedSaveFile;
    private bool isEditMode = false;
    private List<SaveDataEntry> currentEntries = new List<SaveDataEntry>();
    private int selectedEntryIndex = -1;
    private bool isEditing;
    private int editingIdIndex = -1;
    [Header("筛选及搜索")]
    private bool isFilterActive;
    private string searchString = "";
    private DataTypes selectedFilterType;
    private DataTypes allTypesMask;
    private List<SaveDataEntry> filteredEntries = new List<SaveDataEntry>();
    #endregion
    #region 常量
    [Header("Const Related")]
    private string SAVEFOLDERPATHSTR= "C:\\Users\\Administrator\\AppData\\LocalLow\\SeiDoge\\InThisWorld\\SaveData";//默认为Assets/SaveData/
    private string SAVEFOLDERPATHLOCALSTR = "Assets\\SaveData";
    private const string FILEPATHSTR = "LastGamePlayInfo_FilePath";
    private const string FILEPATH1STR = "\\SaveData1";
    private const string FILEPATH2STR = "\\SaveData2";
    private const string FILEPATH3STR = "\\SaveData3";
    #endregion

    [MenuItem("Tools/Default Save Data Generator")]
    public static void ShowWindow()
    {

        GetWindow<DefaultSaveDataGenerator>("Save Data Generator");

    }

    private void OnGUI()
    {
        RelativeAddressMode = GUILayout.Toggle(RelativeAddressMode,"isRelativeMode" );

        GUILayout.Space(10);
        TabSelect();

        if (selectedTab == 0)
        {
            //Debug.Log("怎么回事1");
            DrawCreateTab();
            if (isEditMode)
            {
                // 退出编辑模式
                currentEntries.Clear();
                selectedSaveFile = null;//可以不变？
                selectedEntryIndex = -1;
                isEditMode = false;
            }
        }
        else
        {
            // Debug.Log("怎么回事2");
            DrawEditTab(); 
        }
    }
    private void OnEnable()
    {
            allTypesMask = Enum.GetValues(typeof(DataTypes))
        .Cast<DataTypes>()
        .Where(t => t != DataTypes.Empty)
        .Aggregate((current, next) => current | next);
        selectedFilterType = allTypesMask;
    }

    private void DrawCreateTab()
    {
        GUILayout.Space(10);
        #region 第一行 文档名输入框
        GUILayout.BeginHorizontal();
        newFileName = EditorGUILayout.TextField("Save File Name", newFileName);
        GUILayout.EndHorizontal();
        #endregion


        #region 第二行 文件夹输入框
        GUILayout.BeginHorizontal();

        if (RelativeAddressMode)
        {
            SAVEFOLDERPATHLOCALSTR = EditorGUILayout.TextField("Save Folder Path", SAVEFOLDERPATHLOCALSTR);
        }
        else
        {
            SAVEFOLDERPATHSTR = EditorGUILayout.TextField("Save Folder Path", SAVEFOLDERPATHSTR);

        }
        CreateTab_Brower();
        GUILayout.EndHorizontal();
        #endregion 


        #region 第三部分 文件展示框
        GUILayout.Label("Existing Save Files:");
        Rect listRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
        fileListScrollPos = EditorGUILayout.BeginScrollView(fileListScrollPos, GUILayout.Height(150));
        DrawFileList();
        for (int i = 0; i < existingSaves.Count; i++)
        {
            bool isSelected = i == selectedFileIndex;
            DrawFileListItem(existingSaves[i], isSelected, i);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
        #endregion 


        #region 第四部分 按钮部分
        GUILayout.BeginHorizontal();
        CreateTab_Generate();
        CreateTab_Translate();
        GUILayout.EndHorizontal();
        #endregion
    }//
    private void DrawEditTab()
    {
        GUILayout.Space(10);
        selectedSaveFile = (TextAsset)EditorGUILayout.ObjectField("Save File", selectedSaveFile, typeof(TextAsset), false);
        EditTab_EditMode();//若!isEditMode 读取存档，更新currentEntries，若isEditMode 清空currentEntries
        if (isEditMode)
        {
            //根据currentEntries更新展示框内容
            DrawEditControls();
        }
        else
        {
            //清空
        }

    }
    #region Layout模块
    private void DrawFileList()
    {
        //Step1.清空已有的文件展示框内容
        existingSaves.Clear();
        //try
        //{
        //若手动输入文件夹，若不存在，视为创建文件夹

        if (RelativeAddressMode)
        {

            if (!Directory.Exists(SAVEFOLDERPATHLOCALSTR))
            {
                //Directory.CreateDirectory(SAVEFOLDERPATHLOCALSTR);
                EditorUtility.DisplayDialog("Error", "不存在的文件夹，请确认后重试", "OK");
            }
        }
        else
        {
            if (!Directory.Exists(SAVEFOLDERPATHSTR))
            {
                //Directory.CreateDirectory(SAVEFOLDERPATHSTR);
                EditorUtility.DisplayDialog("Error", "不存在的文件夹，请确认后重试", "OK");
            }
        }

        // Step2.获取该文件夹中所有.json文件，并忽略meta文件,并将其转换为文本文件\

        if (RelativeAddressMode)
        {
            var files = Directory.GetFiles(SAVEFOLDERPATHLOCALSTR, "*.json")
                .Where(file => !file.EndsWith(".meta"))
                .ToList();
            foreach (var file in files)
            {
                string relativePath = file.Replace(SAVEFOLDERPATHLOCALSTR + "\\", "");
                existingSaves.Add(relativePath);
            }
        }
        else
        {
            var files = Directory.GetFiles(SAVEFOLDERPATHSTR, "*.json")
                .Select(Path.GetFileName)
                .ToList();
            foreach (var file in files)
            {
                string relativePath = file.Replace(SAVEFOLDERPATHSTR + "\\", "");
                existingSaves.Add(relativePath);
            }
        }
    }

    private void DrawEditControls()
    {
        #region 第一部分
        GUILayout.BeginHorizontal();

        // 搜索框
        string newSearch = EditorGUILayout.TextField("搜索", searchString);
        //if (newSearch != searchString)
        //{
        //    searchString = newSearch;
        //    ApplyFilter();
        //}

        // 类型筛选下拉菜单
        //DataTypes newType = (StructForSaveData.DataTypes)EditorGUILayout.EnumPopup("类型筛选", selectedFilterType);

        DataTypes newType = (DataTypes)EditorGUILayout.EnumFlagsField("类型筛选", selectedFilterType);

            if (selectedFilterType== DataTypes.Empty)
            {
                if(newType != DataTypes.Empty)
                {
                    selectedFilterType = newType;
                    ApplyFilter();
                }
            }
            else
            {
                if(newType == DataTypes.Empty)
                {
                
                    selectedFilterType = DataTypes.Empty;
                    ApplyFilter();
                }
                else
                {
                    selectedFilterType = newType;
                    ApplyFilter();
                //if (selectedFilterType > newType)
                //{
                //    selectedFilterType &= newType;
                //    ApplyFilter();
                //}
                //else if (selectedFilterType < newType)
                //{
                //    selectedFilterType |= newType;
                //    if ((selectedFilterType & allTypesMask) == allTypesMask)
                //    {
                //        selectedFilterType = allTypesMask;
                //    }
                //    ApplyFilter();
                //}
                //else
                //{

                //}
            }

        }
        
        //if (newType != selectedFilterType)
        //{
        //    Debug.Log(newType.HasFlag(DataTypes.Empty));
        //    //selectedFilterType = newType;
        //    //ApplyFilter();
        //    if (selectedFilterType == DataTypes.Empty)
        //    {
        //        selectedFilterType = newType & (~DataTypes.Empty);
        //        ApplyFilter();
        //    }
        //    else
        //    {
        //        if (newType.HasFlag(DataTypes.Empty))
        //        {
        //            //Debug.Log("看不懂啊");
        //            selectedFilterType = DataTypes.Empty;
        //        }
        //        else
        //        {
        //            //Debug.Log("？？？");
        //            selectedFilterType = newType;
        //        }
        //        ApplyFilter();
        //    }
        //}


        // 清除筛选按钮
        if (GUILayout.Button("×", GUILayout.Width(20)))
        {
            searchString = "";
            selectedFilterType = StructForSaveData.DataTypes.Empty;
            ApplyFilter();
        }

        GUILayout.EndHorizontal();
        #endregion

        #region 第二部分
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditTab_Add();
        Editor_Remove();
        GUILayout.EndHorizontal();
        #endregion

        #region 第三部分
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        if (isFilterActive)
        {
            for (int i = 0; i < filteredEntries.Count; i++)
            {
                int originalIndex = currentEntries.IndexOf(filteredEntries[i]);
                DrawEntry(originalIndex);
            }
        }
        else
        {
            for (int i = 0; i < currentEntries.Count; i++)
            {
                DrawEntry(i);
            }
        }
        EditorGUILayout.EndScrollView();
        EidtTab_Save();

        #endregion

    }


    private void DrawEntry(int index)
    {

        var entry = currentEntries[index];
        bool isSelected = index == selectedEntryIndex;

        var bgColor = entry.hasError ? Color.red * 0.3f : (isSelected ? new Color(0.2f, 0.2f, 0.2f, .5f) : new Color(1f, 1f, 1f, 0.2f));

        GUIStyle trigger = new GUIStyle(EditorStyles.foldout)
        {
            fixedWidth = 5

        };
        GUIStyle selectedStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            fontSize = 13,
            fixedHeight = 20,
            normal = new GUIStyleState { textColor = new Color(0.4f, 0.6f, 1f), background = CreateColorTexture(new Color(0.9f, 0.9f, 1f)) }


        };
        GUIStyle unselectedStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleLeft,
            fontSize = 11,
            fixedHeight = 15,

        };
        using (new GUILayout.VerticalScope("box"))//啥啊...
        {
            using (new BackgroundColorScope(bgColor))//啥啊...
                GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            // 折叠标题
            using (new BackgroundColorScope(GUI.backgroundColor))
                entry.isExpanded = EditorGUILayout.Foldout(entry.isExpanded, "", trigger);
            using (new BackgroundColorScope(bgColor))
                if (isSelected)
                {
                    if (index == editingIdIndex)
                    {
                        entry.id = EditorGUILayout.TextField(entry.id);
                        if((Event.current.keyCode == KeyCode.Return))
                        {
                            Debug.Log("回车");
                            editingIdIndex = -1;    // 退出编辑模式
                            GUI.FocusControl(null);  // 移除焦点
                            Repaint();
                        }
                    }
                    else
                    {
                        GUILayout.Label(entry.id, selectedStyle);
                        Rect headerRect = GUILayoutUtility.GetLastRect();
                        HandleDoubleClick(headerRect, index);
                    }

                }
                else
                {
                    GUILayout.Label(entry.id, unselectedStyle);
                    if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))//如果在框内点击，则选中
                    {
                        selectedEntryIndex = index;
                        editingIdIndex = -1;
                        GUI.FocusControl(null);
                        Repaint();
                    }
                }
            GUILayout.EndHorizontal();
            if (entry.isExpanded)
            {
                DrawEntryDetails(ref entry);
            }
            GUILayout.EndVertical();
        }
        currentEntries[index] = entry;
    }


    private void DrawEntryDetails(ref SaveDataEntry entry)//ref得作用是可以通过该方法直接修改参数的内容
    {


        // 数据类型选择
        var newType = (DataTypes)EditorGUILayout.EnumPopup("Data Type", entry.dataType);
        if (newType != entry.dataType)
        {
            entry.dataType = newType;
            entry.jsonData = "";
        }

        // 动态字段绘制
        if (entry.dataType != DataTypes.Empty)
        {
            var type = TypeRegistry.GetTypeFromDict(entry.dataType);
            object data = null;
            //这个结构又是什么
            try
            {
                data = !string.IsNullOrEmpty(entry.jsonData) ? JsonUtility.FromJson(entry.jsonData, type) : System.Activator.CreateInstance(type);//动态创建对象实例，不用确定具体类型，开销大 但是只用于编辑器使用，所以不会影响游戏性能
            }
            catch
            {
                data = System.Activator.CreateInstance(type);
            }

            data = DrawStructFields(data, type);
            entry.jsonData = JsonUtility.ToJson(data);
        }
    }
    private object DrawStructFields(object data, System.Type type)//多少有点看不明白
    {
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            var value = field.GetValue(data);
            if (field.FieldType == typeof(int))
            {
                value = EditorGUILayout.IntField(field.Name, (int)value);
            }
            else if (field.FieldType == typeof(float))
            {
                value = EditorGUILayout.FloatField(field.Name, (float)value);
            }
            else if (field.FieldType == typeof(bool))
            {
                value = EditorGUILayout.Toggle(field.Name, (bool)value);
            }
            else if (field.FieldType == typeof(string))
            {
                value = EditorGUILayout.TextField(field.Name, (string)value);
            }
            field.SetValue(data, value);
        }
        return data;
    }

    private void DrawFileListItem(string filePath, bool isSelected, int index)//将列表中的每一个存档本身当作一个按钮对待
    {
        GUIStyle fileStyle = new GUIStyle(GUI.skin.button);
        fileStyle.alignment = TextAnchor.MiddleLeft;//设定按钮的排列

        if (isSelected)//如果被选中，设定按钮的背景颜色
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(0.2f, 0.4f, 0.6f));
            tex.Apply();
            fileStyle.normal.background = tex;
        }
        //string fileName = Path.GetFileNameWithoutExtension(filePath);//设定按钮的文字内容
        if (GUILayout.Button(filePath, fileStyle))//对于按钮的逻辑则是设定被选中
        {
            selectedFileIndex = index;
        }
    }
    #endregion


    #region 按键逻辑
    private void TabSelect()
    {
        int clickedTab = GUILayout.Toolbar(selectedTab, new[] { "Create New", "Edit Existing" });//选项卡槽

        // 如果用户点击了不同选项卡且处于编辑模式
        if (clickedTab != selectedTab && isEditMode)
        {
            // 弹出确认对话框（同步阻塞）
            bool confirm = EditorUtility.DisplayDialog(
                "警告",
                "当前正在编辑模式，是否放弃编辑？",
                "确定",
                "取消"
            );

            if (confirm)
            {
                // 确认切换：清理编辑模式状态
                currentEntries.Clear();
                selectedSaveFile = null;
                selectedEntryIndex = -1;
                isEditMode = false;
                selectedTab = clickedTab; // 正式切换选项卡
            }
            else
            {
                // 取消切换：保持原选项卡
                clickedTab = selectedTab; // 阻止实际切换
            }
        }

        // 更新最终选中的选项卡
        selectedTab = clickedTab;
    }

    private void CreateTab_Brower()
    {
        if (GUILayout.Button("Browse", GUILayout.Width(80)))
        {
            string newPath;
            if (RelativeAddressMode)
            {
                newPath = EditorUtility.OpenFolderPanel("Select Save Folder", SAVEFOLDERPATHLOCALSTR, "");//第一个参数是弹出的选择文件夹窗口名字，第二个是当前的位置，第三个当前选择的文件夹名字
                if (!string.IsNullOrEmpty(newPath))
                {
                    SAVEFOLDERPATHLOCALSTR = newPath;
                }
            }
            else
            {
                newPath = EditorUtility.OpenFolderPanel("Select Save Folder", SAVEFOLDERPATHSTR, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    SAVEFOLDERPATHSTR = newPath;
                }
            }
        }
    }
    

    private void CreateTab_Translate()
    {
        if (GUILayout.Button("Transfer to Edit Mode", GUILayout.Height(30)))
        {
            if (selectedFileIndex == -1)
            {
                EditorUtility.DisplayDialog("Error", "未选择任何存档文件，请选择", "OK");
            }
            else
            {
                if (RelativeAddressMode)
                {
                    selectedSaveFile = AssetDatabase.LoadAssetAtPath<TextAsset>((Path.Combine(SAVEFOLDERPATHLOCALSTR, existingSaves[selectedFileIndex])));

                }
                else
                {
                    selectedSaveFile = LoadTextAssetFromLocalPath(Path.Combine(SAVEFOLDERPATHSTR, existingSaves[selectedFileIndex]));
                }

                if (selectedSaveFile != null)
                {
                    selectedTab = 1;
                    GUI.FocusControl(null);// 这句话没理解到
                }
            }
        }
    }



    private void CreateTab_Generate()
    {
        if (GUILayout.Button("Generate New Save", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(newFileName))//健壮性检测1
            {
                EditorUtility.DisplayDialog("Error", "文件名不能为空", "OK");
                return;
            }

            string fullPath;
            //string fullPath = Path.Combine(SAVEFOLDERPATHSTR, newFileName + ".json");
            //if (!Directory.Exists(SAVEFOLDERPATHSTR))//健壮性检测2
            if (RelativeAddressMode)
            {
                if (!Directory.Exists(SAVEFOLDERPATHLOCALSTR))
                {
                    EditorUtility.DisplayDialog("Error", "不存在该路径，请修改文件夹路径", "OK");
                    return;
                }
                else
                {
                    fullPath = Path.Combine(SAVEFOLDERPATHLOCALSTR, newFileName + ".json");
                    if (File.Exists(fullPath))//健壮性检测3
                    {
                        EditorUtility.DisplayDialog("Error", "存在同名存档文件，请修改文件名", "OK");
                        return;
                    }
                    else
                    {
                        File.WriteAllText(fullPath, "{}");
                        //Debug.Log(fullPath);

                        selectedFileIndex = existingSaves.FindIndex(p => p.EndsWith(newFileName + ".json"));
                        EditorUtility.DisplayDialog("Success", "存档文件创建成功", "OK");

                    }
                }
                

            }
            else
            {
                if (!Directory.Exists(SAVEFOLDERPATHSTR))
                {
                    EditorUtility.DisplayDialog("Error", "不存在该路径，请修改文件夹路径", "OK");
                    return;
                }
                else
                {
                    fullPath = Path.Combine(SAVEFOLDERPATHSTR, newFileName + ".json");
                    if (File.Exists(fullPath))//健壮性检测3
                    {
                        EditorUtility.DisplayDialog("Error", "存在同名存档文件，请修改文件名", "OK");
                        return;
                    }
                    else
                    {
                        File.WriteAllText(fullPath, "{}");
                        //Debug.Log(fullPath);

                        selectedFileIndex = existingSaves.FindIndex(p => p.EndsWith(newFileName + ".json"));
                        EditorUtility.DisplayDialog("Success", "存档文件创建成功", "OK");

                    }
                }
            }

        }
    }
    private void Editor_Remove()
    {
        if (GUILayout.Button("Remove Selected Entry (-)") && isEditMode)
        {
            if (selectedEntryIndex != -1)
            {
                currentEntries.RemoveAt(selectedEntryIndex);
                selectedEntryIndex = -1;

                ApplyFilter();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "请选择要删除的可存储对象", "OK");
            }
        }
    }

    private void EditTab_Add()
    {
        /* 
         * Step1.创建一个右键菜单实例
         * Step2.遍历所有的可选项（除empty），创建对应的菜单选项内容
         * Step3.若选择某选项，创建一个该选项的新建可存储对象内容，并将展示框筛选条件设为该选项对应类型。
         * Step4.展示该右键菜单
         */
        if (GUILayout.Button("Add Entry (+)") && isEditMode)
        {
            GenericMenu typeMenu = new GenericMenu();
            foreach (DataTypes type in Enum.GetValues(typeof(DataTypes)))
            {
                if (type == DataTypes.Empty) continue;

                typeMenu.AddItem(
                    new GUIContent(type.ToString()),//选项名
                    false,//是否默认
                    (userData) =>//回调lamda表达式
                    {
                        DataTypes selectedType = (DataTypes)userData;
                        currentEntries.Add(new SaveDataEntry
                        {
                            id = "NEW_ID_" + currentEntries.Count,
                            dataType = selectedType,
                            jsonData = "",
                            isExpanded = true
                        });
                        selectedFilterType = selectedType;
                        ApplyFilter();
                    },
                    type//传递给回调函数的参数
                );
            }
            typeMenu.ShowAsContext();

        }
    }

    private void EditTab_EditMode()
    {
        if (GUILayout.Button(isEditMode ? "Exit Edit Mode" : "Enter Edit Mode"))
        {
            //isEditMode取反
            if (isEditMode)
            {
                // 退出编辑模式
                currentEntries.Clear();
                selectedSaveFile = null;//可以不变？
                selectedEntryIndex = -1;
                isEditMode = false;
            }
            else
            {
                // 进入编辑模式
                if (selectedSaveFile == null)
                {
                    EditorUtility.DisplayDialog("Error", "未选择任何存档对象", "OK");
                    return;
                }
                try
                {
                    var jsonDict = JsonUtility.FromJson<DictionaryWrapper>(selectedSaveFile.text);//使用包装主要是为了防止字典在序列化和反序列化时候出现问题
                    currentEntries = jsonDict.items.Select(item => new SaveDataEntry//这个函数带自动遍历功能，同时lamda表达式在这里也是必须的
                    {
                        id = item.key,
                        dataType = item.type,
                        jsonData = item.value,
                        isExpanded = false,
                        hasError = false
                    }).ToList();

                    ApplyFilter();
                }
                catch
                {

                    EditorUtility.DisplayDialog("Error", "存档文件格式错误", "OK");
                    return;
                }
                isEditMode = true;

            }
        }
    }


    private void EidtTab_Save()
    {
        if (GUILayout.Button("Save Changes"))
        {
            var errorIndices = new List<int>();
            var idSet = new HashSet<string>();

            for (int i = 0; i < currentEntries.Count; i++)//遍历并进行数据检测，同时将错误数据进行标记。标记完毕后替代原有数据
            {
                var entry = currentEntries[i];
                entry.hasError = false;

                // 验证规则
                if (string.IsNullOrEmpty(entry.id))
                {
                    entry.hasError = true;
                    errorIndices.Add(i);
                }
                else if (idSet.Contains(entry.id))
                {
                    entry.hasError = true;
                    errorIndices.Add(i);
                }
                else
                {
                    idSet.Add(entry.id);
                }

                if (entry.dataType == StructForSaveData.DataTypes.Empty)
                {
                    entry.hasError = true;
                    errorIndices.Add(i);
                }

                currentEntries[i] = entry;
            }

            if (errorIndices.Count > 0)
            {
                EditorUtility.DisplayDialog("Error", $"发现 {errorIndices.Count} 处数据错误", "OK");
                Repaint();
                return;
            }

            // 保存数据
            var wrapper = new DictionaryWrapper
            {
                items = currentEntries.Select(e => new SaveData//这个又是什么
                {
                    key = e.id,
                    type = e.dataType,
                    value = e.jsonData
                }).ToList()
            };

            if (RelativeAddressMode)
            {

                File.WriteAllText(Path.Combine(SAVEFOLDERPATHLOCALSTR, existingSaves[selectedFileIndex]), JsonUtility.ToJson(wrapper));

            }
            else
            {
                File.WriteAllText(Path.Combine(SAVEFOLDERPATHSTR, existingSaves[selectedFileIndex]), JsonUtility.ToJson(wrapper));
            }
            //AssetDatabase.Refresh();

            ApplyFilter();
            Repaint();

            EditorUtility.DisplayDialog("Success", "保存成功", "OK");
        }
        // 数据验证
    }

    #endregion



    #region 小方法
    private class BackgroundColorScope : GUI.Scope
    {
        private readonly Color originalColor;
        public BackgroundColorScope(Color color)
        {
            originalColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }
        protected override void CloseScope()
        {
            GUI.backgroundColor = originalColor;
        }
    }

    private void HandleDoubleClick(Rect rect, int index)
    {
        Event current = Event.current;

        if (current.type == EventType.MouseDown &&
            current.clickCount == 2 &&
            rect.Contains(current.mousePosition))
        {
            editingIdIndex = index;
            GUI.FocusControl("ID_Field_" + index);
            current.Use();
        }
    }

    private void ApplyFilter()
    {
        filteredEntries.Clear();
        filteredEntries = currentEntries
        .Where(entry => 
        {
            //不用return (entry.dataType == selectedFilterType);的原因是因为entry.dataType永远为单一状态，而selectedFilterType可能是多个状态集合
            if (selectedFilterType == DataTypes.Empty)
            {
                return (entry.dataType == DataTypes.Empty);
            }
            else
            {
                return
                ((selectedFilterType & entry.dataType) != 0);

            }
        })
        .Where(entry => 
            entry.id.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
        )
        .ToList();
        Repaint();
        isFilterActive = !string.IsNullOrEmpty(searchString) || selectedFilterType != allTypesMask;

    }
    private TextAsset LoadTextAssetFromLocalPath(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"文件不存在: {filePath}");
            return null;
        }

        try
        {
            // 读取文件内容
            string fileContent = File.ReadAllText(filePath);

            // 创建TextAsset（仅内存中存在，不写入项目）
            return new TextAsset(fileContent)
            {
                name = Path.GetFileNameWithoutExtension(filePath)
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            return null;
        }
    }
    #endregion
}
