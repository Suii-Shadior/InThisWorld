using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SceneConnectionManager", menuName = "Scene Management/Scene Connection Manager")]
public class SceneConnectionManager : ScriptableObject
{
    [System.Serializable]
    public class SceneConnection
    {
        public string mainScene;
        public List<string> neighborScenes = new List<string>();
    }

    public List<SceneConnection> connections = new List<SceneConnection>();//ʹ��List��Ϊԭʼ���ݵ�������Է�����Inspector���ֶ����

    private Dictionary<string, List<string>> connectionDict;//ʹ��Dict��Ϊ����ʱ����ʵ�ʲ��ҵ�

    public void BuildDictionary()
    //����ʱ�����ڴ��ڹ�����ֻҪ���иö�������ã����б����������ڴ��г������ڣ������ֵ䣬����������Hierarchy�仯���仯
    //�˳����з��ر༭ģʽʱ�ڴ�ȫ��ע�����ֵ�һ�������
    //�ܽ᣺������LevelController�Ͼ�����
    {
        connectionDict = new Dictionary<string, List<string>>();
        foreach (var connection in connections)
        {
            if (!connectionDict.ContainsKey(connection.mainScene))
            {
                connectionDict[connection.mainScene] = new List<string>();
            }
            connectionDict[connection.mainScene].AddRange(connection.neighborScenes);
        }
    }

    public List<string> GetNeighbors(string sceneName)
    {
        if (connectionDict == null) BuildDictionary();
        return connectionDict.TryGetValue(sceneName, out var neighbors) ? neighbors : new List<string>();
    }
}
