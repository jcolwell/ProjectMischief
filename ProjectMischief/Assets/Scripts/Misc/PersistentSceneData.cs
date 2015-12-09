using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class PersistentSceneData : MonoBehaviour 
{
    // Private
    string saveFile = "/Data.mmf";
    public Data data;

	static uint firstLevel = 1;
	static uint numLevels = 1;

    // Static
    // Accessor
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
                returnData.InitializeData();
			}
        }
        else
        {
            returnData = SceneDataObj.GetComponent<PersistentSceneData>();
        }

        return returnData;
    }

    // Public

    // file saving and loading
    public void Save() 
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + saveFile);

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + saveFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + saveFile, FileMode.Open);
            Debug.Log(Application.persistentDataPath);

            try
            {
                data = (Data)bf.Deserialize( file );
            }
            catch (SerializationException exception)
            {
                Debug.Log("Issue with deSerializetion. The problem is "  + exception.Message + 
                    ". Creating new save data. If this message persits get Cole to fix it" );
                
                data = new Data();
                data.firstPlay = true;
            }

            file.Close();
        }
        else
        {
            data = new Data();
        }
    }

    public void ResetData()
    {
        data = new Data();
        InitializeData();
        Save();
    }

    // Getters and setters
    public List<Stats> GetPlayerEquipment()
    {
        return data.playerEquipment;
    }

    public List<Stats> GetStoreEquipment()
    {
        return data.storeEquipment;
    }

    public Stats GetCurEquipment(EquipmentTypes type)
    {
        return data.currentEquipment[(int)type];
    }

    public int GetPlayerCurrency()
    {
        return data.playerCurrency;
    }

    public uint GetNumHints()
    {
        return data.numHints;
    }

   public uint GetNumLevels()
	{
		return numLevels;
	}

    public uint GetFirstLevelUnityIndex()
	{
		return firstLevel;
	}

    public int GetNumTools(ToolTypes tool)
    {
        if ((int)tool >= (int)ToolTypes.eToolMAX)
        {
            return 0;
        }
        return data.numTools[(int)tool];
    }

    public int GetMaxToolnum()
    {
        return (int)data.currentEquipment[(int)EquipmentTypes.attire].stat;
    }

	public void IncreaseHints()
	{
		++data.numHints;
	}

	public void DecreaseHints()
	{
		if(data.numHints == 0)
		{
			return;
		}
		
		--data.numHints;
	}

    public void IncreaseNumTools(ToolTypes tool)
    {
        if ((int)tool >= (int)ToolTypes.eToolMAX)
        {
            return;
        }
        ++data.numTools[(int)tool];
    }

    public void DecreaseNumTools(ToolTypes tool)
    {
        if ((int)tool >= (int)ToolTypes.eToolMAX || data.numTools[(int)tool] == 0)
        {
            return;
        }
        --data.numTools[(int)tool];
    }

    public void SetPlayerCurrency(int playerCurrency)
    {
        data.playerCurrency = playerCurrency;
    }
    
    public void SetCurEquipment(ref Stats equip)
    {
        data.currentEquipment[(int)equip.type] = equip;
    }

	public void SetLevelCompleted(uint unityLevelIndex, char grade)
	{
		uint index = unityLevelIndex - firstLevel;
		if( index < numLevels)
		{
			data.levelGrades[index] = grade;
			data.LevelsCompleted[(int)index] = true;
		}
	}

    // Private
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        Save();
    }

        // to load equipment from the csv
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

        data.currentEquipment = null;
        data.currentEquipment = new Stats[(int)EquipmentTypes.MAX];

        data.storeEquipment.Clear();
		data.playerEquipment.Clear();

		data.storeEquipment = new List<Stats>();
		TextAsset text = Resources.Load<TextAsset>( "EquipmentSheet" );
		char[] delim = new char[] { ',' };
		char[] delim2 = new char[] { '\n' };
		string[] equipmentList = text.text.Split( delim2, System.StringSplitOptions.RemoveEmptyEntries );

		uint numStats = ((uint)equipmentList.Length);
		// start at 1 to avoid the title headings within the csv
		for( uint i = 1; i < numStats; ++i )
		{
			Stats curStat = new Stats();
 
			string[] line = equipmentList[i].Split( delim );
			curStat.name = line[0];
			string typeString = line[1].ToLower();
			if(typeString.Equals("headgear") )
			{
				curStat.type = EquipmentTypes.headGear;
			}
			else if(typeString.Equals("footwear"))
			{
				curStat.type = EquipmentTypes.footWear;
			}
            else if( typeString.Equals( "attire" ) )
            {
                curStat.type = EquipmentTypes.attire;
            }
            curStat.stat = System.Convert.ToSingle( line[2] );
            curStat.cost = System.Convert.ToInt32(line[3]);

			data.storeEquipment.Add(curStat);

            curStat = null;
            line = null;
		}
	}
    
        // insitalize the data
    void InitializeData()
    {
        LoadEquipment();
        data.playerCurrency = 0;

        Stats defaultAttire = new Stats();
        defaultAttire.type = EquipmentTypes.attire;
        defaultAttire.stat = 3.0f;
        data.currentEquipment[(int)EquipmentTypes.attire] = defaultAttire;

        data.levelGrades = new char[numLevels];
        data.LevelsCompleted = new BitArray((int)numLevels, false);

        data.numTools[(int)ToolTypes.eJammer] = 0;
        data.numTools[(int)ToolTypes.eMirror] = 0;
        data.numTools[(int)ToolTypes.eSmokeBomb] = 0;

        data.firstPlay = false;
    }
}

[Serializable]
public class Data
{
    public Data()
    {}

    public bool firstPlay = true;

    public int playerCurrency = 0;

    // Inventory information
    public int[] numTools = new int[(int)ToolTypes.eToolMAX];
    public uint numHints = 0;
	
    public List<Stats> playerEquipment;
    public List<Stats> storeEquipment;
    public Stats[] currentEquipment = new Stats[(int)EquipmentTypes.MAX];

    // Level information
    public char[] levelGrades;
    public BitArray LevelsCompleted;
}