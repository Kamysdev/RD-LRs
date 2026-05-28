using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoadSystem
{
    public static void SaveToFile(string fileName, List<SaveData> saveData)
    {
        DataList saveList = new()
        {
            list = saveData
        };

        string json = JsonUtility.ToJson(saveList, true);
        File.WriteAllText(fileName, json);
    }

    public static List<SaveData> LoadFromFile(string fileName)
    {
        string json = File.ReadAllText(fileName);
        DataList loadList = JsonUtility.FromJson<DataList>(json);
        return loadList != null ? loadList.list : new List<SaveData>();
    }
}
