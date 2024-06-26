﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

public static class SaveSystem
{
    public static string savePath => Application.persistentDataPath + "/save" + Player.instance.gameDataId + ".xml";

    public static void SaveData() {
        SaveData(Player.instance.gameData);
    }

    public static void SaveData(GameData data, int id = -1) {
        id = (id == -1) ? Player.instance.gameDataId : id;
        string path = "save" + id.ToString();
        SaveXML(data, path);
    }

    public static GameData LoadData(int id = 0) {
        GameData data = null;
        string path = "save" + id.ToString();
        data = LoadXML<GameData>(path);
        if (data == null) {
            data = GameData.GetDefaultData(2000, 0);
            SaveData(data, id);
            
            Debug.Log("Save file not found in " + path);
            Debug.Log("Using default data.");
        }
        return data;
    }

    public static void SaveXML(object item, string path) {
        string XmlPath = Application.persistentDataPath + "/" + path + ".xml";
        using (StreamWriter writer = new StreamWriter(XmlPath)) {
            XmlSerializer serializer = new XmlSerializer(item.GetType());
            serializer.Serialize(writer.BaseStream, item);
            writer.Close();
        };
    }

    public static T LoadXML<T>(string path) {
        string XmlPath = Application.persistentDataPath + "/" + path + ".xml";
        if (!File.Exists(XmlPath))
            return default(T);
        
        using (StreamReader reader = new StreamReader(XmlPath)) {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T deserialized = (T)serializer.Deserialize(reader.BaseStream);
            reader.Close();
            return deserialized;
        };
    }
    
}
