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
    private int selectedEntryIndex = -1;//该变量是标识在currentEntries中是否选取的Entry索引号
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
        

        GUILayout.Space(10);
        TabSelect();

        if (selectedTab == 0)
        {
            //Debug.Log("怎么回事1");
            DrawCreateTab();
            //if (isEditMode)
            //{
            //    // 退出编辑模式
            //    currentEntries.Clear();
            //    selectedSaveFile = null;//可以不变？
            //    selectedEntryIndex = -1;
            //    isEditMode = false;
            //}
        }
        else
        {
            // Debug.Log("怎么回事2");
            DrawEditorTab(); 
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

    #region Layout逻辑
    private void DrawCreateTab()
    {
        // 本方法用于完整绘制CreateTab内容，包括默认模式目标文件部分、存档文件展示部分、存档相关功能按钮部分
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        CreateTab_DrawFileComponent();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        CreateTab_DrawFolderComponent();
        GUILayout.EndHorizontal();
        GUILayout.Label("Existing Save Files:");
        Rect listRect = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
        CreateTab_DrawFileDetailComponent();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        CreateTab_GenerateButton();
        CreateTab_TransferButton();
        GUILayout.EndHorizontal();
        #endregion
    }
    private void DrawEditorTab()
    {
        // 本方法用于完整绘制EditorTab的内容，包括抬头目标文件选择部分和编辑模式下核心内容部分

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        selectedSaveFile = (TextAsset)EditorGUILayout.ObjectField("Save File", selectedSaveFile, typeof(TextAsset), false);
        EditorTab_EditButton();
        GUILayout.EndHorizontal();


        if (isEditMode)
        {
            EditorTab_DrawCoreComponents();
        }
        else
        {
            //清空
        }
    }






    private void CreateTab_DrawFileListItem(string filePath, bool isSelected, int index)
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

    private void CreateTab_DrawFileComponent()
    {
        newFileName = EditorGUILayout.TextField("Save File Name", newFileName);
        DefaultSaveDataMode = GUILayout.Toggle(DefaultSaveDataMode, "isDefaultSaveDataMode");

    }

    private void CreateTab_DrawFolderComponent()
    {
        if (DefaultSaveDataMode)
        {
            currentFolderPath = EditorGUILayout.TextField("Save Folder Path", SAVEFOLDERPATHLOCALSTR);
        }
        else
        {
            currentFolderPath = EditorGUILayout.TextField("Save Folder Path", SAVEFOLDERPATHSTR);

        }
        CreateTab_BrowerButton();
    }

    private void CreateTab_DrawFileDetailComponent()
    {

        fileListScrollPos = EditorGUILayout.BeginScrollView(fileListScrollPos, GUILayout.Height(150));
        GetFileList();
        for (int i = 0; i < existingSaves.Count; i++)
        {
            bool isSelected = i == selectedFileIndex;
            CreateTab_DrawFileListItem(existingSaves[i], isSelected, i);
        }
    }

    private void EditorTab_DrawCoreComponents()
    {
        /* 本方法主要用于Editor在绘制编辑模式下最核心的搜索筛选、存档内容编辑栏和展示框内容
         * 
         * Part1.搜索筛选部分，包括模糊搜索和数据类型筛选
         * Part2.存档内容功能包括添加或删除存储内容条目
         * Part3.存档内容展示将展示目标存档内所有存档条目，并根据需要进行具体内容展示及筛选
         * Part4.保存按键对正在编辑中的存档文件内容写入该存档文件
         */

        GUILayout.BeginHorizontal();
        EditorTab_DrawFilterComponent();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EidtorTab_DrawEntryFunctionComponent();
        GUILayout.EndHorizontal();


        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        Editor_DrawEntryDisplayComponent();
        EditorGUILayout.EndScrollView();

        EidtorTab_SaveButton();



    }

    private void EditorTab_DrawFilterComponent()
    {
        string newSearch = EditorGUILayout.TextField("模糊搜索", searchString);
        DataTypes newType = (DataTypes)EditorGUILayout.EnumFlagsField("类型筛选", selectedFilterType);
        if (selectedFilterType == DataTypes.Empty)
        {
            if (newType != DataTypes.Empty)
            {
                selectedFilterType = newType;
                ApplyFilter();
                Repaint();

            }
        }
        else
        {
            if (newType == DataTypes.Empty)
            {

                selectedFilterType = DataTypes.Empty;
                ApplyFilter();
                Repaint();
            }
            else
            {
                selectedFilterType = newType;
                ApplyFilter();
                Repaint();
            }

        }

        EditorTab_CancelFilterButton();
    }

    private void EidtorTab_DrawEntryFunctionComponent()
    {
        EditTab_AddButton();
        EditorTab_RemoveButton();
    }

    private void Editor_DrawEntryDisplayComponent()
    {
        if (isFilterActive)
        {
            for (int i = 0; i < filteredIndices.Count; i++)
            {
                int originalIndex = filteredIndices[i];
                EditorTab_DrawEntry(originalIndex);
            }

        }
        else
        {
            for (int i = 0; i < currentEntries.Count; i++)
            {
                EditorTab_DrawEntry(i);
            }
        }
    }


    private void EditorTab_DrawEntry(int index)
    {

        var entry = currentEntries[index];
        bool isSelected= index == selectedEntryIndex;
        //if (selectedEntryIndex != -1)
        //{
        //    isSelected = (isFilterActive) ? (index == filteredIndices[selectedEntryIndex]) : (index == selectedEntryIndex);

        //}
        //else
        //{
        //    isSelected = false;
        //}

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
                EditorTab_DrawEntryDetails(ref entry);
            }
            GUILayout.EndVertical();
        }
        currentEntries[index] = entry;
    }


    private void EditorTab_DrawEntryDetails(ref SaveDataEntry entry)//ref得作用是可以通过该方法直接修改参数的内容
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

            data = EditorTab_DrawStructFields(data, type);
            entry.jsonData = JsonUtility.ToJson(data);
        }
    }
    private object EditorTab_DrawStructFields(object data, System.Type type)//多少有点看不明白
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



    #region 按键逻辑
    private void TabSelect()
    {
        /* 本方法是生成一个编辑器选项卡，用于实现CreateTab和EditorTab的切换
         *
         * 考虑了处在编辑情况下的误点
         */
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

    private void CreateTab_BrowerButton()
    {
        // 该方法用于生成一个编辑器按钮，用于CreateTab中选择一个新的目标文件夹
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
    private void CreateTab_TransferButton()
    {
        /* 该方法用于生成一个编辑器按键，用于CreateTab中选中的存档对象送入EditTab的编辑存档对象并转到EditTab
         * 
         * 
         * Step1-a.若无选中目标，则报错并终止
         * Step1-b.当有选中目标时，将当前文件夹和该选中目标的名称组合为可用的文件路径
         * Step2.根据当前是否是测试模式，获取存档文件具体内容，对selectedSaveFile赋值
         * Step3.转向EditTab
         */
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
                    GUI.FocusControl(null);
                }
            }
        }
    }
    private void CreateTab_GenerateButton()
    {
        /* 该方法用于生成一个编辑器按键，用于CreateTab中在目标文件夹下新建一个存档文件
         * 
         * Step1.若新建文件名字为空，则报错，退出
         * Step2.目标文件夹路径不存在，则报错，退出
         * Step3.若目标文件夹下存在同名文件，则报错，退出
         * Step4.在目标文件夹下创建当前名字下的存档文件，并将当前选中对象作为选中对象
         * 
         */


        if (GUILayout.Button("Generate New Save", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(newFileName))//健壮性检测1
            {
                EditorUtility.DisplayDialog("Error", "文件名不能为空", "OK");
                return;
            }
            else
            {
                string fullPath;//这里没有用currentFilePath的原因主要是此处的作用和后面作用存在割裂，这里仅作为本按键范围内使用
                if (!Directory.Exists(currentFolderPath))
                {
                    EditorUtility.DisplayDialog("Error", "不存在该路径，请修改文件夹路径", "OK");
                    return;
                }
                else
                {
                    fullPath = Path.Combine(currentFolderPath, newFileName + ".json");
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
    private void EditorTab_RemoveButton()
    {

        /* 该方法用于生成一个编辑器按钮，用于在EditorTab中删除存储内容条目
         * 
         * Step1-a.若没有选定任何存储内容条目，则报错并终止
         * Step1-b.若有选择的目标，判断当前是否是筛选状态
         * Step2-a.若是筛选状态，首先从currentEntries删除对应Entry，然后从filteredIndices中删除对应键值对，同时对齐filteredIndices
         * Step2-b.若不是筛选状态，则直接从currentEntries删除
         */
        if (GUILayout.Button("Remove Selected Entry (-)") && isEditMode)
        {

            if (selectedEntryIndex != -1)
            {
                if (isFilterActive)
                {
                    
                    currentEntries.RemoveAt(selectedEntryIndex);
                    filteredIndices.Remove(selectedEntryIndex);
                    //filteredIndices.RemoveAt(filteredIndices.IndexOf(selectedEntryIndex));
                    for (int i = 0; i < filteredIndices.Count; i++)
                    {
                        if (filteredIndices[i] > selectedEntryIndex)
                        {
                            filteredIndices[i]--;
                        }
                    }
                }
                else
                {
                    currentEntries.RemoveAt(selectedEntryIndex);
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
    private void EditTab_AddButton()
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


    private void EditorTab_EditButton()
    {
        /* 本方法生成一个编辑器按钮，用于在EidtorTab中
         * 
         * Step1.判断当前是否处于编辑状态
         * Step2-a.若为编辑状态下，清空currentEntries、selectedSaveFile，重置选取索引。
         * Step2-b.若不是编辑状态下进入健壮性判断，若未通过，报错并退出若通过进行以下步骤
         *      1）从目标文件按照DictionaryWrapper格式读取文件，将其Entry格式化后存入currentEntries
         *      2）重置筛选并重绘
         * Step3.编辑状态置反
         *      
         * 
         */
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
                    Repaint();
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
    private void EditorTab_CancelFilterButton()
    {
        /* 用于生成一个编辑器按键，用于在EditorTab中取消筛选
         * 
         * isFilterActive 会在ApplyFilter中判断
         */


        if (GUILayout.Button("×", GUILayout.Width(20)))
        {
            searchString = "";
            selectedFilterType = DataTypes.Empty;
            
            ApplyFilter();
            Repaint();
        }
    }
    private void EidtorTab_SaveButton()
    {
        /* 本方法用于生成一个编辑器按钮，用于在EditorTab中保存当前正在编辑的可存储条目内容
         * 
         * Step1.若处在筛选模式下，则先取消筛选
         * Step2.新建一个int表用于接收数据有效性检测结果，并根据该结果进行判断
         * Step3.若结果中无内容，表明没有数据错误，可以保存
         * Step4.若结果中有内容，根据内容进行报错
         * Step5.若处在筛选模式下，则重新取消
         * 
         * 之后可以考虑完善的内容：
         * 1、有错误的时候考虑用一个在报错框内展示，通过某种形式可以快速定位到错误内容位置
         * 
         */
        if (GUILayout.Button("Save Changes"))
        {

                if (isFilterActive)
                {
                    string tempSearch = searchString;
                    DataTypes tempFilter = selectedFilterType;
                    searchString = "";
                    selectedFilterType = allTypesMask;
                    ApplyFilter();


                    List<int> errorIndices = new();
                    errorIndices = ValidateAllEntries();
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
                else
                {
                    List<int> errorIndices = new();
                    errorIndices = ValidateAllEntries();
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

                }

            }

        
    }

 #endregion



    #region 小方法
    private void GetFileList()
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
    private class BackgroundColorScope : GUI.Scope
    {
        /* 该方法用于更改编辑器组成部分的颜色
         * 
         * 
         * 
         */
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
    private TextAsset LoadTextAssetFromLocalPath(string filePath)
    {
        /* 该方法用于通过本地存档所在路径加载本地外存存档
         * 
         * Step1.健壮性检测
         * Step2.尝试以TextAsset格式读取文件内容，放入内存中，并命名。若失败则报错。
         * 
         */
        if (!File.Exists(filePath))
        {
            Debug.LogError($"文件不存在: {filePath}");
            return null;
        }

        try
        {
            string fileContent = File.ReadAllText(filePath);
            return new TextAsset(fileContent)
            {
                //此处命名仅仅为了方便调试，无实际作用
                name = Path.GetFileNameWithoutExtension(filePath)
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            return null;
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
        /* 该方法主要用于在筛选内容改变之后对展示内容应用筛选条件
         * 
         * Step1.清除已有筛选内容
         * Step2.遍历currentEntries，判断是否满足当前筛选搜索要求。若满足，添加进filteredIndices
         * Step3.根据筛选搜索条件重绘工具栏
         * 
         */
        filteredIndices.Clear();
        selectedFileIndex = -1;
        isFilterActive = !string.IsNullOrEmpty(searchString) || selectedFilterType != allTypesMask;
        for (int i = 0; i < currentEntries.Count; i++)
        {
            var entry = currentEntries[i];

            if (ShouldIncludeInFilter(entry))
            {
                filteredIndices.Add(i);
            }
        }
        

    }

    private bool ShouldIncludeInFilter(SaveDataEntry entry)
    {
        /* 本方法用于返回bool，用于对输入Entry进行是否符合搜索筛选条件进行判断
         * 
         * Step1.对筛选条件进行判断，若格式为空或者格式并未包含于筛选条件中，则认为不满足筛选条件
         * Step2.对搜索条件进行判断，若id并不模糊包含搜索条件，则认为不满足收缩条件
         * Step3.对搜索筛选条件取或并返回
         */
        bool typeMatch = (selectedFilterType == DataTypes.Empty)
            ? (entry.dataType == DataTypes.Empty)
            : ((selectedFilterType & entry.dataType) != 0);

        bool searchMatch = entry.id.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;

        return typeMatch && searchMatch;
    }
    private List<int> ValidateAllEntries()
    {

        /* 本方法用于返回一个int表，用于在保存时进行数据有效性检测后总结结果
         * 
         * Step1.新建一个int表和string哈希表，用于进行同名Entry或空名的判断和进行有效性判断总结
         * Step2.遍历currentEntries，拷贝内容便于进行后续判断
         *  Step2-a.判断是否空名或重名。若是则标记，若否则添加进idset
         *  Step2-b.判断数据类型是否为空，若是则标记
         * Step3.将经判断后的Entry返回到currentEntries中，若当前Entry有标记，统计进int表内
         * 
         * 
         */
        List<int> errorIndices = new ();
        HashSet<string> idSet = new ();
        for (int i = 0; i < currentEntries.Count; i++)
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
}
