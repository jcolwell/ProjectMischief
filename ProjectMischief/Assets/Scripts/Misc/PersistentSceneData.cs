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

			if(returnData.data.firstPlay)
			{
				returnData.LoadEquipment();

				// TOBUILD: commented out line below for testing purposes, unCommet line while makeing Build
                returnData.data.firstPlay = false;
			}
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
        else
        {
            data = new Data();
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

    public List<Stats> GetStoreEquipment()
    {
        return data.storeEquipment;
    }

    public void SetStoreEquipment( ref List<Stats> storeEquipment )
    {
        data.storeEquipment = storeEquipment;
    }

    // Private
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

	void LoadEquipment()
	{
        if(data.playerEquipment == null)
        {
            data.playerEquipment = new List<Stats>();
        }

        if( data.storeEquipment == null )
        {
            data.storeEquipment = new List<Stats>();
        }

		data.storeEquipment.Clear();
		data.playerEquipment.Clear();

		data.storeEquipment = new List<Stats>();
		TextAsset text = Resources.Load<TextAsset>( "EquipmentSheet" );
		char[] delim = new char[] { ',' };
		char[] delim2 = new char[] { '\n' };
		string[] equipmentList = text.text.Split( delim2, System.StringSplitOptions.RemoveEmptyEntries );

		const uint numCategories = 3;
		uint numStats = ((uint)equipmentList.Length / numCategories);
		// start at 1 to avoid the title headings within the csv
		for( uint i = 1; i < numStats; ++i )
		{
			Stats curStat = new Stats();

			string[] line = equipmentList[i].Split( delim );
			curStat.name = line[i*numCategories];
			string typeString = line[(i*numCategories) + 1].ToLower();
			if(typeString.Equals("headgear") )
			{
				curStat.type = EquipmentTypes.headGear;
			}
			else if(typeString.Equals("footwear"))
			{
				curStat.type = EquipmentTypes.footWear;
			}
			curStat.stat = System.Convert.ToSingle( line[(i*numCategories) + 2]);

			curStat.isEquipt = false;
			data.storeEquipment.Add(curStat);
		}
	}
    
    void OnDestroy()
    {
        Save();
    }
}

[Serializable]
public class Data
{
    public Data()
    {}

	public bool firstPlay = true;
    public List<Stats> playerEquipment;
    public List<Stats> storeEquipment;
}