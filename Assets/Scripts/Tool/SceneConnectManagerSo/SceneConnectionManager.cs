using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

[CreateAssetMenu(fileName = "SceneConnectionManager", menuName = "Scene Management/Scene Connection Manager")]
public class SceneConnectionManager : ScriptableObject
{
    [System.Serializable]
    public class SceneConnection
    {
        public string mainScene;
        public List<string> neighborScenes = new List<string>();

        //public SceneAsset mainScene; 
        //public List<SceneAsset> neighborScenes = new List<SceneAsset>();
    }

    public List<SceneConnection> connections = new List<SceneConnection>();//使用List作为原始数据的输入可以方便在Inspector中手动添加

    private Dictionary<string, List<string>> connectionDict;//使用Dict作为运行时进行实际查找的

    public void BuildDictionary()
    //引用时才在内存内构建，只要持有该对象的引用，所有变量都会在内存中持续存在，包括字典，并不会随着Hierarchy变化而变化
    //退出运行返回编辑模式时内存全部注销，字典一定会清空
    //总结：挂载在LevelController上就行了
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
