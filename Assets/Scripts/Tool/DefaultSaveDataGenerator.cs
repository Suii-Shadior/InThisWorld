// ��̬����������

using StructForSaveData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DefaultSaveDataGenerator : EditorWindow
{
    /* �ñ༭�����ڶ����浵�ļ����б༭������ʵ����Ӧ��Ϸ��������
     * 
     * Ŀǰʵ�ֵĹ��ܣ�
     * 1��ѡ��Ŀ���ļ��У�����Ŀ���ļ��������ʾ���д浵
     * 2����Ŀ���ļ������½��浵�հ״浵������ѡ�����д浵���б༭
     * 3����ȡ����ʾĿ��浵��ǰ���ݣ�ͬʱ֧�����ͼ�����ID��ģ������
     * 4����Ŀ��浵����ӻ�ɾ���浵��Ŀ���༭�浵��Ŀ����
     * 5��������޸������Ƿ�Ϲ棬���Ϲ��򱣴���Ŀ��浵�������Ϲ��������ʾ
     * 
     * 
     * Ŀǰʵ�ֵ�˼·��
     * 1����ΪCreateTab��EditTab��CreateTab��Ҫ�����ļ���������ļ���ѡȡ���ļ�չʾ�򡢰�ť���Ĳ��֣�EditorTab��Ҫ�����༭���󿪹أ�ɸѡ�����������浵����չʾ���༭�������水ť��ɣ�
     * 2��CreateTab���ļ�չʾ��ͨ��Directory.GetFiles��ȡ�����д浵����ͨ��existingSaves����DrawFileListItemչʾ
     * 3��EditTab�д浵����ͨ��DictionaryWrapper��ȡ��Ŀ��浵���ݣ�ͨ��currentEntries����DrawEntryչʾ��ɸѡ������ͨ��filteredEntries�õ�
     * 
     * 
     * ����֮����Ҫ����ʵ�ֵĹ��ܣ�
     * 1����Ӧ��Ŀ������Ӳ�����TAG�����ڸ���ݺ͸��ӵķ�Χ����
     * 2����ӳ��������Ͷ�������������������Ҷ���
     */


    #region ���ݽṹ������
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
    #region ����
    private int selectedTab = 0;
    private bool DefaultSaveDataMode = false;
    private string currentFolderPath;
    private string currentFilePath;
    [Header("��һѡ�")]
    private Vector2 fileListScrollPos;
    private string newFileName = "NewSave";
    private List<string> existingSaves = new List<string>();
    private int selectedFileIndex = -1;
    [Header("�ڶ�ѡ�")]
    private Vector2 scrollPos;
    private TextAsset selectedSaveFile;
    private bool isEditMode = false;
    private List<SaveDataEntry> currentEntries = new List<SaveDataEntry>();
    private int selectedEntryIndex = -1;
    private bool isEditing;
    private int editingIdIndex = -1;
    [Header("ɸѡ������")]
    private bool isFilterActive;
    private string searchString = "";
    private DataTypes selectedFilterType;
    private DataTypes allTypesMask;
    private List<SaveDataEntry> filteredEntries = new List<SaveDataEntry>();
    private List<int> filteredIndices = new ();
    #endregion
    #region ����
    [Header("Const Related")]
    private string SAVEFOLDERPATHSTR= "C:\\Users\\Administrator\\AppData\\LocalLow\\SeiDoge\\InThisWorld\\SaveData";//Ĭ��ΪAssets/SaveData/
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
            //Debug.Log("��ô����1");
            DrawCreateTab();
            if (isEditMode)
            {
                // �˳��༭ģʽ
                currentEntries.Clear();
                selectedSaveFile = null;//���Բ��䣿
                selectedEntryIndex = -1;
                isEditMode = false;
            }
        }
        else
        {
            // Debug.Log("��ô����2");
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
        #region ��һ�� �ĵ��������
        GUILayout.BeginHorizontal();
        newFileName = EditorGUILayout.TextField("Save File Name", newFileName);
        GUILayout.EndHorizontal();
        #endregion


        #region �ڶ��� �ļ��������
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


        #region �������� �ļ�չʾ��
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


        #region ���Ĳ��� ��ť����
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
        EditTab_EditMode();//��!isEditMode ��ȡ�浵������currentEntries����isEditMode ���currentEntries
        GUILayout.EndHorizontal();
        if (isEditMode)
        {
            //����currentEntries����չʾ������
            DrawEditComponents();
        }
        else
        {
            //���
        }

    }
    #region Layoutģ��
    private void DrawFileList()
    {
        //Step1.������е��ļ�չʾ������,����֤���ļ����Ƿ����
        existingSaves.Clear();
        if (!Directory.Exists(currentFolderPath))
        {
            bool createFolder = EditorUtility.DisplayDialog(
            "�ļ��в�����",
            $"·�� '{currentFolderPath}' �����ڡ��Ƿ񴴽����ļ��У�",
            "��", "��"
            );
            if (createFolder)
            {
                try
                {
                    Directory.CreateDirectory(currentFolderPath);
                    Debug.Log("�����ɹ�");
                    AssetDatabase.Refresh();
                }
                catch (Exception ex)
                {
                    // �������ļ���ʱ���ܳ��ֵĴ���
                    EditorUtility.DisplayDialog("����", $"�����ļ���ʧ��: {ex.Message}", "ȷ��");
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
            // Step2.��ȡ���ļ���������.json�ļ���������meta�ļ�,������ת��Ϊ�ı��ļ�\
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
        #region ����ɸѡ
        GUILayout.BeginHorizontal();

        string newSearch = EditorGUILayout.TextField("ģ������", searchString);
        DataTypes newType = (DataTypes)EditorGUILayout.EnumFlagsField("����ɸѡ", selectedFilterType);
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

        #region չʾ�����ܼ�
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        EditTab_Add();
        Editor_Remove();
        GUILayout.EndHorizontal();
        #endregion

       #region ��������
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
        using (new GUILayout.VerticalScope("box"))//ɶ��...
        {
            using (new BackgroundColorScope(bgColor))//ɶ��...
                GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            // �۵�����
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
                            Debug.Log("�س�");
                            editingIdIndex = -1;    // �˳��༭ģʽ
                            GUI.FocusControl(null);  // �Ƴ�����
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
                    if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))//����ڿ��ڵ������ѡ��
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


    private void DrawEntryDetails(ref SaveDataEntry entry)//ref�������ǿ���ͨ���÷���ֱ���޸Ĳ���������
    {


        // ��������ѡ��
        var newType = (DataTypes)EditorGUILayout.EnumPopup("Data Type", entry.dataType);
        if (newType != entry.dataType)
        {
            entry.dataType = newType;
            entry.jsonData = "";
        }

        // ��̬�ֶλ���
        if (entry.dataType != DataTypes.Empty)
        {
            var type = TypeRegistry.GetTypeFromDict(entry.dataType);
            object data = null;
            //����ṹ����ʲô
            try
            {
                data = !string.IsNullOrEmpty(entry.jsonData) ? JsonUtility.FromJson(entry.jsonData, type) : System.Activator.CreateInstance(type);//��̬��������ʵ��������ȷ���������ͣ������� ����ֻ���ڱ༭��ʹ�ã����Բ���Ӱ����Ϸ����
            }
            catch
            {
                data = System.Activator.CreateInstance(type);
            }

            data = DrawStructFields(data, type);
            entry.jsonData = JsonUtility.ToJson(data);
        }
    }
    private object DrawStructFields(object data, System.Type type)//�����е㿴������
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

    private void DrawFileListItem(string filePath, bool isSelected, int index)//���б��е�ÿһ���浵������һ����ť�Դ�
    {
        GUIStyle fileStyle = new GUIStyle(GUI.skin.button);
        fileStyle.alignment = TextAnchor.MiddleLeft;//�趨��ť������

        if (isSelected)//�����ѡ�У��趨��ť�ı�����ɫ
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(0.2f, 0.4f, 0.6f));
            tex.Apply();
            fileStyle.normal.background = tex;
        }
        //string fileName = Path.GetFileNameWithoutExtension(filePath);//�趨��ť����������
        if (GUILayout.Button(filePath, fileStyle))//���ڰ�ť���߼������趨��ѡ��
        {
            selectedFileIndex = index;
        }
    }
    #endregion


    #region �����߼�
    private void TabSelect()
    {
        int clickedTab = GUILayout.Toolbar(selectedTab, new[] { "Create New", "Edit Existing" });//ѡ���

        // ����û�����˲�ͬѡ��Ҵ��ڱ༭ģʽ
        if (clickedTab != selectedTab && isEditMode)
        {
            // ����ȷ�϶Ի���ͬ��������
            bool confirm = EditorUtility.DisplayDialog(
                "����",
                "��ǰ���ڱ༭ģʽ���Ƿ�����༭��",
                "ȷ��",
                "ȡ��"
            );

            if (confirm)
            {
                // ȷ���л�������༭ģʽ״̬
                currentEntries.Clear();
                selectedSaveFile = null;
                selectedEntryIndex = -1;
                isEditMode = false;
                selectedTab = clickedTab; // ��ʽ�л�ѡ�
            }
            else
            {
                // ȡ���л�������ԭѡ�
                clickedTab = selectedTab; // ��ֹʵ���л�
            }
        }

        // ��������ѡ�е�ѡ�
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
                EditorUtility.DisplayDialog("Error", "δѡ���κδ浵�ļ�����ѡ��", "OK");
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
                    GUI.FocusControl(null);// ��仰û��⵽
                }
            }
        }
    }



    private void CreateTab_Generate()
    {
        if (GUILayout.Button("Generate New Save", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(newFileName))//��׳�Լ��1
            {
                EditorUtility.DisplayDialog("Error", "�ļ�������Ϊ��", "OK");
                return;
            }

            string fullPath;
            //string fullPath = Path.Combine(SAVEFOLDERPATHSTR, newFileName + ".json");
            //if (!Directory.Exists(SAVEFOLDERPATHSTR))//��׳�Լ��2
            if (DefaultSaveDataMode)
            {
                if (!Directory.Exists(SAVEFOLDERPATHLOCALSTR))
                {
                    EditorUtility.DisplayDialog("Error", "�����ڸ�·�������޸��ļ���·��", "OK");
                    return;
                }
                else
                {
                    fullPath = Path.Combine(SAVEFOLDERPATHLOCALSTR, newFileName + ".json");
                    if (File.Exists(fullPath))//��׳�Լ��3
                    {
                        EditorUtility.DisplayDialog("Error", "����ͬ���浵�ļ������޸��ļ���", "OK");
                        return;
                    }
                    else
                    {
                        File.WriteAllText(fullPath, "{}");
                        //Debug.Log(fullPath);

                        selectedFileIndex = existingSaves.FindIndex(p => p.EndsWith(newFileName + ".json"));
                        EditorUtility.DisplayDialog("Success", "�浵�ļ������ɹ�", "OK");

                    }
                }
                

            }
            else
            {
                if (!Directory.Exists(SAVEFOLDERPATHSTR))
                {
                    EditorUtility.DisplayDialog("Error", "�����ڸ�·�������޸��ļ���·��", "OK");
                    return;
                }
                else
                {
                    fullPath = Path.Combine(SAVEFOLDERPATHSTR, newFileName + ".json");
                    if (File.Exists(fullPath))//��׳�Լ��3
                    {
                        EditorUtility.DisplayDialog("Error", "����ͬ���浵�ļ������޸��ļ���", "OK");
                        return;
                    }
                    else
                    {
                        File.WriteAllText(fullPath, "{}");
                        //Debug.Log(fullPath);

                        selectedFileIndex = existingSaves.FindIndex(p => p.EndsWith(newFileName + ".json"));
                        EditorUtility.DisplayDialog("Success", "�浵�ļ������ɹ�", "OK");

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
            //    EditorUtility.DisplayDialog("Error", "��ѡ��Ҫɾ���Ŀɴ洢����", "OK");
            //}

            if (selectedEntryIndex != -1)
            {
                // �������б����Ƴ�
                int filteredIndex = filteredIndices.IndexOf(selectedEntryIndex);
                if (filteredIndex != -1)
                {
                    filteredIndices.RemoveAt(filteredIndex);
                    // �����б����Ƴ�
                    currentEntries.RemoveAt(selectedEntryIndex);

                    // �����������Ƴ������д��ڸ�������ֵ��1
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
                    EditorUtility.DisplayDialog("Error", "��ѡ��Ҫɾ���Ŀɴ洢����", "OK");
                }


            }
        }
    }

    private void EditTab_Add()
    {
        /* 
         * Step1.����һ���Ҽ��˵�ʵ��
         * Step2.�������еĿ�ѡ���empty����������Ӧ�Ĳ˵�ѡ������
         * Step3.��ѡ��ĳѡ�����һ����ѡ����½��ɴ洢�������ݣ�����չʾ��ɸѡ������Ϊ��ѡ���Ӧ���͡�
         * Step4.չʾ���Ҽ��˵�
         */
        if (GUILayout.Button("Add Entry (+)") && isEditMode)
        {
            GenericMenu typeMenu = new GenericMenu();
            foreach (DataTypes type in Enum.GetValues(typeof(DataTypes)))
            {
                if (type == DataTypes.Empty) continue;//��仰��;����

                typeMenu.AddItem(
                    new GUIContent(type.ToString()),//ѡ����
                    false,//�Ƿ�Ĭ��
                    (userData) =>//�ص�lamda���ʽ
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
                    type//���ݸ��ص������Ĳ���
                );
            }
            typeMenu.ShowAsContext();
        }
    }

    private void EditTab_EditMode()
    {
        if (GUILayout.Button(isEditMode ? "Exit Edit Mode" : "Enter Edit Mode", GUILayout.Width(120)))
        {
            //isEditModeȡ��
            if (isEditMode)
            {
                // �˳��༭ģʽ
                currentEntries.Clear();
                selectedSaveFile = null;//���Բ��䣿
                selectedEntryIndex = -1;
                isEditMode = false;
            }
            else
            {
                // ����༭ģʽ
                if (selectedSaveFile == null)
                {
                    EditorUtility.DisplayDialog("Error", "δѡ���κδ浵����", "OK");
                    return;
                }
                try
                {
                    var jsonDict = JsonUtility.FromJson<DictionaryWrapper>(selectedSaveFile.text);//ʹ�ð�װ��Ҫ��Ϊ�˷�ֹ�ֵ������л��ͷ����л�ʱ���������
                    currentEntries = jsonDict.items.Select(item => new SaveDataEntry//����������Զ��������ܣ�ͬʱlamda���ʽ������Ҳ�Ǳ����
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

                    EditorUtility.DisplayDialog("Error", "�浵�ļ���ʽ����", "OK");
                    return;
                }
                isEditMode = true;

            }
        }
    }
    private void EditerTab_CancelFilter()
    {
        if (GUILayout.Button("��", GUILayout.Width(20)))
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
            EditorUtility.DisplayDialog("Error", $"���� {errorIndices.Count} �����ݴ���", "OK");
        }
        else
        {
            // ��������
            var wrapper = new DictionaryWrapper
            {
                items = currentEntries.Select(e => new SaveData//�������ʲô
                {
                    key = e.id,
                    type = e.dataType,
                    value = e.jsonData
                }).ToList()
            };
            Debug.Log(currentFilePath + "����ɹ�");
            File.WriteAllText(currentFilePath, JsonUtility.ToJson(wrapper));
            //AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "����ɹ�", "OK");
        }
        searchString = tempSearch;
        selectedFilterType = tempFilter;
        ApplyFilter();
        Repaint();

        }
        // ������֤
    

    private List<int> ValidateAllEntries()
    {
        List<int> errorIndices = new ();
        HashSet<string> idSet = new ();
        for (int i = 0; i < currentEntries.Count; i++)//�������������ݼ�⣬ͬʱ���������ݽ��б�ǡ������Ϻ����ԭ������
        {
            var entry = currentEntries[i];
            entry.hasError = false;

            // ��֤����
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



    #region С����
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

            // ����ƥ����
            bool typeMatch = (selectedFilterType == DataTypes.Empty)
                ? (entry.dataType == DataTypes.Empty)
                : ((selectedFilterType & entry.dataType) != 0);

            // ����ƥ����
            bool searchMatch = entry.id.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;

            if (typeMatch && searchMatch)
            {
                filteredIndices.Add(i);
            }
        }



        //filteredEntries = currentEntries
        //.Where(entry => 
        //{
        //    //����return (entry.dataType == selectedFilterType);��ԭ������Ϊentry.dataType��ԶΪ��һ״̬����selectedFilterType�����Ƕ��״̬����
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
            Debug.LogError($"�ļ�������: {filePath}");
            return null;
        }

        try
        {
            // ��ȡ�ļ�����
            string fileContent = File.ReadAllText(filePath);

            // ����TextAsset�����ڴ��д��ڣ���д����Ŀ��
            return new TextAsset(fileContent)
            {
                name = Path.GetFileNameWithoutExtension(filePath)
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"����ʧ��: {e.Message}");
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
