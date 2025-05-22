
namespace StructForSaveData
{
    public interface ISaveable
    {

        string GetISaveID();
        void RegisterSaveable(ISaveable _saveable);
        void UnregisterSaveable(ISaveable _saveable);
        //Toucher_Platform GetSaveDataStructFromDictionary<Toucher_Platform>(ISaveable _saveable);
        void LoadSpecificData(SaveData _passingData);
        SaveData SaveDatalizeSpecificData();

        void SaveDataSync();//用于将脚本内的相应内容同SaveData中的内容统一
    }

    public interface ISave<T> : ISaveable where T : struct 
    { 
        T GetSpecificDataByISaveable(SaveData _passedData);


    }

}


