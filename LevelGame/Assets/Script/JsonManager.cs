using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class QuestData
{
    public QData[] datas = new QData[3];
}
[Serializable]
public struct QData
{
    public int mode;
    public int score;
    public int count;
    public int targetCount;
    public int reward;
    public int state;
}
public class JsonManager : SingleTon<JsonManager>
{
    public QuestData questData = new QuestData();
    string fileName;
    private void Awake()
    {
        fileName = Path.Combine(Application.persistentDataPath + "/QuestData.json");
        if (File.Exists(fileName))
        {
            LoadData();
        }
        else
        {
            PlayerPrefs.SetInt("Day", DateTime.Now.Day);
            QuestManager.Instance.RandomQuest();
            SaveData();
        }
    }
    public void SaveData()
    {
        for (int i = 0; i < QuestManager.Instance.quests.Length; i++)
        {
            questData.datas[i] = QuestManager.Instance.quests[i].questData;
        }
        if (File.Exists(fileName))
            File.Delete(fileName);

        string json = JsonUtility.ToJson(questData);

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        string encodedJson = Convert.ToBase64String(bytes);

        File.WriteAllText(fileName, encodedJson);
    }


    public void LoadData()
    {
        if (File.Exists(fileName))
        {
            string jsonFromFile = File.ReadAllText(fileName);

            byte[] bytes = Convert.FromBase64String(jsonFromFile);
            string decodedJson = System.Text.Encoding.UTF8.GetString(bytes);

            questData = JsonUtility.FromJson<QuestData>(decodedJson);

        }
    }
}
