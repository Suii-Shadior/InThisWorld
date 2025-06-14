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
    /* 该编辑器用于对外存存档文件进行编辑，进而实现相应游戏内容配置
     * 
     * 目前实现的功能：
     * 1、选择目标文件夹，根据目标文件夹情况显示已有存档
     * 2、在目标文件夹内新建存档空白存档，或者选择已有存档进行编辑
     * 3、读取并显示目标存档当前内容，同时支持类型检索和ID的模糊搜索
     * 4、在目标存档内添加或删除存档条目，编辑存档条目内容
     * 5、检查已修改内容是否合规，若合规则保存至目标存档，若不合规则高亮显示
     * 
     * 
     * 目前实现的思路：
     * 1、分为CreateTab和EditTab，CreateTab主要包括文件名输入框、文件夹选取框、文件展示框、按钮区四部分，EditorTab主要包括编辑对象开关，筛选与搜索区，存档内容展示及编辑区、保存按钮组成；
     * 2、CreateTab中文件展示区通过Directory.GetFiles获取到所有存档对象，通过existingSaves遍历DrawFileListItem展示
     * 3、EditTab中存档内容通过DictionaryWrapper获取到目标存档内容，通过currentEntries遍历DrawEntry展示；筛选搜索是通过filteredEntries得到
     * 
     * 
     * 可能之后需要补充实现的功能：
     * 1、对应条目可以添加不定量TAG，用于更快捷和复杂的范围遍历
     * 2、添加场景检索和多条件检索，更方便查找对象。
     */


    #region 数据结构及其他
    private struct SaveDataEntry
    {
        public string id;
        public DataTypes dataType;
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
    private bool DefaultSaveDataMode = false;
    private string currentFolderPath;
    private string currentFilePath;
    [Header("第一选项卡")]
    private Vector2 fileListScrollPos;
    private string newFileName = "NewSave";
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
    private List<int> filteredIndices = new ();
    #endregion
    #region 常量
    [Header("Const Related")]
    private string SAVEFOLDERPATHSTR= "C:\\Users\\Administrator\\AppData\\LocalLow\\SeiDoge\\InThisWorld\\SaveData";//默认为Assets/SaveData/
    private string SAVEFOLDERPATHLOCALSTR = "Assets\\Settings\\SaveData";
    #endregion

    [MenuItem("Tools/Default Save Data Generator")]
    public static void ShowWindow()
    {

        GetWindow<DefaultSaveDataGenerator>("Save Data Generator");

    }

    private void OnGUI()
    {
        DefaultSaveDataMode = GUILayout.Toggle(DefaultSaveDataMode, "isDefaultSaveDataMode");

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

        if (DefaultSaveDataMode)
        {
            currentFolderPath = EditorGUILayout.TextField("Save Folder Path", SAVEFOLDERPATHLOCALSTR);
        }
        else
        {
            currentFolderPath = EditorGUILayout.TextField("Save Folder Path", SAVEFOLDERPATHSTR);

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
        GUILayout.BeginHorizontal();
        selectedSaveFile = (TextAsset)EditorGUILayout.ObjectField("Save File", selectedSaveFile, typeof(TextAsset), false);
        EditTab_EditMode();//若!isEditMode 读取存档，更新currentEntries，若isEditMode 清空currentEntries
        GUILayout.EndHorizontal();
        if (isEditMode)
        {
            //根据currentEntries更新展示框内容
            DrawEditComponents();
        }
        else
        {
            //清空
        }

    }
    #region Layout模块
    private void DrawFileList()
    {
        //Step1.清空已有的文件展示框内容,并验证该文件夹是否存在
        existingSaves.Clear();
        if (!Directory.Exists(currentFolderPath))
        {
            bool createFolder = EditorUtility.DisplayDialog(
            "文件夹不存在",
            $"路径 '{currentFolderPath}' 不存在。是否创建此文件夹？",
            "是", "否"
            );
            if (createFolder)
            {
                try
                {
                    Directory.CreateDirectory(currentFolderPath);
                    Debug.Log("创建成功");
                    AssetDatabase.Refresh();
                }
                catch (Exception ex)
                {
                    // 处理创建文件夹时可能出现的错误
                    EditorUtility.DisplayDialog("错误", $"创建文件夹失败: {ex.Message}", "确定");
                    return;
                }
            }
            else
            {
                if (DefaultSaveDataMode)
                {
                    currentFolderPath = EditorGUILayout.TextField("Save Folder Path", SAVEFOLDERPATHLOCALSTR);
                }
                else
                {
                    currentFolderPath = EditorGUILayout.TextField("Save Folder Path", SAVEFOLDERPATHSTR);
                }
                return;
            }

        }
        else
        {
            // Step2.获取该文件夹中所有.json文件，并忽略meta文件,并将其转换为文本文件\
            var files = Directory.GetFiles(currentFolderPath, "*.json")
                .Where(file => !file.EndsWith(".meta"))
                .ToList();
            foreach (var file in files)
            {
                string relativePath = file.Replace(currentFolderPath + "\\", "");
                existingSaves.Add(relativePath);
            }
        }

    }

    private void DrawEditComponents()
    {
        #region 搜索筛选
        GUILayout.BeginHorizontal();

        string newSearch = EditorGUILayout.TextField("模糊搜索", searchString);
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

            }

        }

        EditerTab_CancelFilter();
        GUILayout.EndHorizontal();
        #endregion

        #region 展示区功能键
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditTab_Add();
        Editor_Remove();
        GUILayout.EndHorizontal();
        #endregion

       #region 第三部分
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        //if (isFilterActive)
        //{
        //    for (int i = 0; i < filteredEntries.Count; i++)
        //    {
        //        int originalIndex = currentEntries.IndexOf(filteredEntries[i]);
        //        DrawEntry(originalIndex);
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < currentEntries.Count; i++)
        //    {
        //        DrawEntry(i);
        //    }
        //}
        for (int i = 0; i < filteredIndices.Count; i++)
        {
            int originalIndex = filteredIndices[i];
            DrawEntry(originalIndex);
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

            string newPath= EditorUtility.OpenFolderPanel("Select Save Folder", currentFolderPath, "");

            if (currentFolderPath != newPath)
            {
                if (!string.IsNullOrEmpty(newPath))
                {
                    currentFolderPath = newPath;
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
                currentFilePath = Path.Combine(currentFolderPath, existingSaves[selectedFileIndex]);
                if (DefaultSaveDataMode)
                {
                    selectedSaveFile = AssetDatabase.LoadAssetAtPath<TextAsset>(currentFilePath);
                }
                else
                {
                    selectedSaveFile = LoadTextAssetFromLocalPath(currentFilePath);
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
            if (DefaultSaveDataMode)
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
            //if (selectedEntryIndex != -1)
            //{
            //    currentEntries.RemoveAt(selectedEntryIndex);
            //    selectedEntryIndex = -1;

            //    ApplyFilter();
            //}
            //else
            //{
            //    EditorUtility.DisplayDialog("Error", "请选择要删除的可存储对象", "OK");
            //}

            if (selectedEntryIndex != -1)
            {
                // 从索引列表中移除
                int filteredIndex = filteredIndices.IndexOf(selectedEntryIndex);
                if (filteredIndex != -1)
                {
                    filteredIndices.RemoveAt(filteredIndex);
                    // 从主列表中移除
                    currentEntries.RemoveAt(selectedEntryIndex);

                    // 更新索引：移除后所有大于该索引的值减1
                    for (int i = 0; i < filteredIndices.Count; i++)
                    {
                        if (filteredIndices[i] > selectedEntryIndex)
                        {
                            filteredIndices[i]--;
                        }
                    }

                    selectedEntryIndex = -1;
                    Repaint();
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "请选择要删除的可存储对象", "OK");
                }


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
                if (type == DataTypes.Empty) continue;//这句话用途不明

                typeMenu.AddItem(
                    new GUIContent(type.ToString()),//选项名
                    false,//是否默认
                    (userData) =>//回调lamda表达式
                    {
                        int newIndex = currentEntries.Count;
                        DataTypes selectedType = (DataTypes)userData;
                        currentEntries.Add(new SaveDataEntry
                        {
                            id = "NEW_ID_" + currentEntries.Count,
                            dataType = selectedType,
                            jsonData = "",
                            isExpanded = true
                        });
                        selectedFilterType = selectedType;
                        //ApplyFilter();
                        if (ShouldIncludeInFilter(currentEntries[newIndex]))
                        {
                            filteredIndices.Add(newIndex);
                        }


                        Repaint();
                    },
                    type//传递给回调函数的参数
                );
            }
            typeMenu.ShowAsContext();
        }
    }

    private void EditTab_EditMode()
    {
        if (GUILayout.Button(isEditMode ? "Exit Edit Mode" : "Enter Edit Mode", GUILayout.Width(120)))
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
    private void EditerTab_CancelFilter()
    {
        if (GUILayout.Button("×", GUILayout.Width(20)))
        {
            searchString = "";
            selectedFilterType = StructForSaveData.DataTypes.Empty;
            ApplyFilter();
        }
    }

    private void EidtTab_Save()
    {
        string tempSearch = searchString;
        DataTypes tempFilter = selectedFilterType;
        List<int> errorIndices = new ();
        if (GUILayout.Button("Save Changes"))
        {

            searchString = "";
            selectedFilterType = allTypesMask;
            ApplyFilter();
            errorIndices = ValidateAllEntries();

         
        }

        if (errorIndices.Count > 0)
        {
            EditorUtility.DisplayDialog("Error", $"发现 {errorIndices.Count} 处数据错误", "OK");
        }
        else
        {
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
            Debug.Log(currentFilePath + "保存成功");
            File.WriteAllText(currentFilePath, JsonUtility.ToJson(wrapper));
            //AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "保存成功", "OK");
        }
        searchString = tempSearch;
        selectedFilterType = tempFilter;
        ApplyFilter();
        Repaint();

        }
        // 数据验证
    

    private List<int> ValidateAllEntries()
    {
        List<int> errorIndices = new ();
        HashSet<string> idSet = new ();
        for (int i = 0; i < currentEntries.Count; i++)//遍历并进行数据检测，同时将错误数据进行标记。标记完毕后替代原有数据
        {
            var entry = currentEntries[i];
            entry.hasError = false;

            // 验证规则
            if (string.IsNullOrEmpty(entry.id))
            {
                entry.hasError = true;

            }
            else if (idSet.Contains(entry.id))
            {
                entry.hasError = true;

            }
            else
            {
                idSet.Add(entry.id);
            }

            if (entry.dataType == StructForSaveData.DataTypes.Empty)
            {
                entry.hasError = true;

            }

            currentEntries[i] = entry;
            if (entry.hasError)
            {
                errorIndices.Add(i);
            }
        }
        return errorIndices;
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

        for (int i = 0; i < currentEntries.Count; i++)
        {
            var entry = currentEntries[i];

            // 类型匹配检查
            bool typeMatch = (selectedFilterType == DataTypes.Empty)
                ? (entry.dataType == DataTypes.Empty)
                : ((selectedFilterType & entry.dataType) != 0);

            // 搜索匹配检查
            bool searchMatch = entry.id.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;

            if (typeMatch && searchMatch)
            {
                filteredIndices.Add(i);
            }
        }



        //filteredEntries = currentEntries
        //.Where(entry => 
        //{
        //    //不用return (entry.dataType == selectedFilterType);的原因是因为entry.dataType永远为单一状态，而selectedFilterType可能是多个状态集合
        //    if (selectedFilterType == DataTypes.Empty)
        //    {
        //        return (entry.dataType == DataTypes.Empty);
        //    }
        //    else
        //    {
        //        return
        //        ((selectedFilterType & entry.dataType) != 0);

        //    }
        //})
        //.Where(entry => 
        //    entry.id.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
        //)
        //.ToList();
        isFilterActive = !string.IsNullOrEmpty(searchString) || selectedFilterType != allTypesMask;
        Repaint();

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

    private bool ShouldIncludeInFilter(SaveDataEntry entry)
    {
        bool typeMatch = (selectedFilterType == DataTypes.Empty)
            ? (entry.dataType == DataTypes.Empty)
            : ((selectedFilterType & entry.dataType) != 0);

        bool searchMatch = entry.id.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;

        return typeMatch && searchMatch;
    }
    #endregion
}
