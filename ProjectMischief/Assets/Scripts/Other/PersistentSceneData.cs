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
    const int leaderBoardSpots = 10;

	static uint firstLevel = 1;
	static uint numLevels = 5;

    [HideInInspector]
    public uint ticksBetweenFrames = 1;
    [HideInInspector]
    public bool tuneViewConeUpdate = false;
    

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
            //Debug.Log(Application.persistentDataPath);

            try
            {
                data = (Data)bf.Deserialize( file );
            }
            catch (SerializationException exception)
            {
                Debug.Log("[Persistent Scene Data] Issue with de-serializetion. The problem is "  + exception.Message + 
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

    public uint GetLastLevelUnlocked()
    {
        return data.lastLevelUnlocked;
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

    public bool CheckIfPlayerHasPlayedTut()
    {
        return data.hasPlayedTut;
    }

    public void SetHasPlayedTut(bool hasPlayed)
    {
        data.hasPlayedTut = hasPlayed;
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

    public void IncreaseNumTools( ToolTypes tool , int amountToIncrease)
    {
        if( (int)tool >= (int)ToolTypes.eToolMAX )
        {
            return;
        }
        data.numTools[(int)tool]+= amountToIncrease;
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
        if( equip.stat < data.currentEquipment[ ( int )equip.type ].stat )
        {
            equip.stat = data.currentEquipment[ ( int )equip.type ].stat;
        }
        data.currentEquipment[(int)equip.type] = equip;
    }

	public void SetLevelCompleted(uint unityLevelIndex, char grade)
	{
		uint index = unityLevelIndex - firstLevel;
        if(data.levelGrades == null || data.LevelsCompleted == null)
        {
            data.levelGrades = new char[numLevels];
            data.LevelsCompleted = new BitArray((int)numLevels, false);
        }

		if( index < numLevels)
		{
			data.levelGrades[index] = grade;
            if (!data.LevelsCompleted[(int)index])
            {
                ++data.lastLevelUnlocked;
                data.LevelsCompleted[(int)index] = true;
            }
		}
	}

    public bool CheckLeaderBoard(int unityScenID, char grade, double time, ref int leaderBoardSpot)
    {
        for(int i = 0; i < leaderBoardSpots; ++i)
        {
            if (data.leaderBoard[i] == null || data.leaderBoard[i].grade > grade || 
                (data.leaderBoard[i].grade == grade && data.leaderBoard[i].time > time))
            {
                unityScenID -= (int)firstLevel;
                ShiftLeaderBoardDown(i);
                data.leaderBoard[i] = new LeaderBoardInfo( unityScenID, time, grade, "Unknown");
                leaderBoardSpot = i;
                return true;
            }
        }
        return false;
    }

    public void SetLeaderBoardName(int spotIndex, string _name)
    {
        if(spotIndex > 0 && spotIndex < data.leaderBoard.Length)
        {
            data.leaderBoard[spotIndex].name = _name;
        }
    }

    public LeaderBoardInfo[] GetLeaderBoard()
    {
        return data.leaderBoard;
    }

    public SettingsData GetSettingsData()
    {
        if(data.settings == null)
        {
            data.settings = new SettingsData();
        }
        return data.settings;
    }

    public ArtFileInfo GetArtInfo(int index)
    {
        CheckArtInfoInitilized();
        if (index >= 0 && index < data.encounteredArt.Count)
        {
            return data.encounteredArt[index];
        }
        return null;
    }

    public int GetEncounteredArtCount()
    {
        CheckArtInfoInitilized();
        return data.encounteredArt.Count;
    }

    public bool AddEncounterdArt(ArtFileInfo artInfo)
    {
        CheckArtInfoInitilized();
        // element => element.artFileName == artInfo.artFileName is a lambda function
        if (!data.encounteredArt.Exists(element => element.artFileName == artInfo.artFileName))
        {
            data.encounteredArt.Add(artInfo);
            return true;
        }
        return false;
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
    
    void LoadFirstArt()
    {
        TextAsset text = Resources.Load<TextAsset>("FirstArt");
        char[] delim = new char[] { '\r', '\n' };
        string[] artInfoFromFile = text.text.Split(delim, System.StringSplitOptions.RemoveEmptyEntries);
        ArtFileInfo artInfo = new ArtFileInfo();
        artInfo.artFileName = artInfoFromFile[0];
        artInfo.name = artInfoFromFile[1];
        artInfo.year = artInfoFromFile[2];
        artInfo.artist = artInfoFromFile[3];
        artInfo.description = artInfoFromFile[4];
        data.encounteredArt.Add(artInfo);
    }

        // insitalize the data
    void InitializeData()
    {
        data.settings = new SettingsData();

        data.leaderBoard = new LeaderBoardInfo[leaderBoardSpots];

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

        data.encounteredArt = new List<ArtFileInfo>();
        LoadFirstArt();

        data.firstPlay = false;
    }

    void CheckArtInfoInitilized()
    {
        if (data.encounteredArt == null)
        {
            data.encounteredArt = new List<ArtFileInfo>();
            LoadFirstArt();
        }
    }
        // shift leaderbaords down
    void ShiftLeaderBoardDown(int index)
    {
        if(index >= 0 && index < data.leaderBoard.Length)
        {
            for(int i = data.leaderBoard.Length - 1; i > index; --i)
            {
                data.leaderBoard[i] = data.leaderBoard[i - 1];
            }
        }
    }

}

[Serializable]
public class Data
{
    public Data()
    {}

    public bool firstPlay = true;
    public bool hasPlayedTut = false;

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
    public uint lastLevelUnlocked = 0;

    // LeaderBoardInformation
    public LeaderBoardInfo [] leaderBoard;

    // Settings information
    public SettingsData settings;

    // art Info
    public List<ArtFileInfo> encounteredArt;
}

[Serializable]
public class SettingsData
{
    public float sfxSoundLevel = 100.0f;
    public float musicSoundLevel = 60.0f;
}

[Serializable]
public class LeaderBoardInfo
{
    public LeaderBoardInfo()
    { }

    public LeaderBoardInfo( int _level, double _time, char _grade, string _name) 
    {
        level = _level;
        time = _time;
        grade = _grade;
        name = _name;
    }

    public int    level;
    public double time;
    public char   grade;
    public string name;
}