using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PersistentSceneData : MonoBehaviour 
{
    // Private
    string saveFile = "/Data.mmf";
    Data data;

    // Static
    static public PersistentSceneData GetPersistentData()
    {
        GameObject SceneDataObj = GameObject.Find("SceneData");
        PersistentSceneData returnData;

        if(SceneDataObj == null)
        {
            SceneDataObj = new GameObject();
            SceneDataObj.AddComponent<PersistentSceneData>();
            SceneDataObj.name = "SceneData";
            returnData = SceneDataObj.GetComponent<PersistentSceneData>();
            returnData.Load();
        }
        else
        {
            returnData = SceneDataObj.GetComponent<PersistentSceneData>();
        }

        return returnData;
    }

    // Public
    public void Save() 
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + saveFile, FileMode.Open);

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + saveFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + saveFile, FileMode.Open);
            data = (Data)bf.Deserialize(file);
            file.Close();
        }
    }

    public List<Stats> GetPlayerEquipment()
    {
        return data.playerEquipment;
    }

    public void SetPlayerEquipment(ref List<Stats> playerEquipment)
    {
        data.playerEquipment = playerEquipment;
    }

    // Private
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
}

[Serializable]
class Data
{
    Data()
    {}

    public List<Stats> playerEquipment;
    public List<Stats> storeEquipment;
}