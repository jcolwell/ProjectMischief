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
    const float saveFileVersionNumber = 0.909102016f; 
    const int numOfPaintingsInGame = 27;
    string saveFile = "/Data.mmf";
    public Data data;
    const int leaderBoardSpots = 10;

	static uint firstLevel = 1;
	static uint numLevels = 7;

    [HideInInspector]
    public uint ticksBetweenFrames = 1;
    [HideInInspector]
    public bool tuneViewConeUpdate = false;

    const float expModifer = 1.2f; //the modifier that changes how much exp is required to level up

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
            else 
            {
                if (returnData.data.prestigeLevelData == null)
                {
                    returnData.data.prestigeLevelData = new PrestigeLevelData();
                }
                if(returnData.data.artUnlocked == null)
                {
                    returnData.data.artUnlocked = new BitArray(numOfPaintingsInGame, false);
                }
            }
        }
        else
        {
            returnData = SceneDataObj.GetComponent<PersistentSceneData>();
        }

        return returnData;
    }

    // Public

    #region FileHandling
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
            if(data.prestigeLevelData == null)
            {
                data.prestigeLevelData = new PrestigeLevelData();
            }
        }
        else
        {
            data = new Data();
        }

        if(data.versionNum != saveFileVersionNumber)
        {
            InitializeData();
            Save();
        }
    }

    public void ResetData()
    {
        data = new Data();
        InitializeData();
        Save();
    }
    #endregion

    #region EquipmentAndItems
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

    public uint GetNumHints()
    {
        return data.numHints;
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

    public void IncreaseHints(uint numToincreaseBy)
    {
        data.numHints+= numToincreaseBy;
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

    public void SetCurEquipment(ref Stats equip)
    {
        if( equip.stat < data.currentEquipment[ ( int )equip.type ].stat )
        {
            equip.stat = data.currentEquipment[ ( int )equip.type ].stat;
        }
        data.currentEquipment[(int)equip.type] = equip;
    }

    #endregion

    #region BooleanStuff
    public bool CheckIfPlayerHasPlayedTut()
    {
        return data.hasPlayedTut;
    }

    public void SetHasPlayedTut(bool hasPlayed)
    {
        data.hasPlayedTut = hasPlayed;
    }

    public bool CheckIfPlayerHasPlayedPainingTut()
    {
        return data.hasPlayedPaintingTut;
    }

    public void SetHasPlayedPainingTut(bool hasPlayed)
    {
        data.hasPlayedPaintingTut = hasPlayed;
    }

    public bool CheckIfPlayerhasPlayerMoved()
    {
        return data.hasPlayerMoved;
    }

    public void SethasPlayerMoved(bool hasPlayed)
    {
        data.hasPlayerMoved = hasPlayed;
    }

    #endregion

    #region LevelInfo
    public uint GetFirstLevelUnityIndex()
	{
		return firstLevel;
	}

    public uint GetLastLevelUnlocked()
    {
        return data.lastLevelUnlocked;
    }

    public uint GetNumLevels()
	{
		return numLevels;
	}

    public void SetLevelCompleted(uint unityLevelIndex, char grade)
	{
		uint index = unityLevelIndex - firstLevel;
        if(data.levelGrades == null || data.levelsCompleted == null)
        {
            data.lastLevelUnlocked = 0;
            data.levelGrades = new char[numLevels];
            data.levelsCompleted = new BitArray((int)numLevels, false);
        }

		if( index < numLevels)
		{
            char newGradeOffset = (grade == 'S') ? (char)19 : (char)0;
            char oldGradeOffset = (data.levelGrades[index] == 'S') ? (char)19 : (char)0;


            if (!data.levelsCompleted[(int)index])
            {
                data.lastLevelUnlocked = index + 1;
                data.levelsCompleted[(int)index] = true;
                data.levelGrades[index] = grade;
            }
            else if (data.levelGrades[index] == 0 || (data.levelGrades[index] - oldGradeOffset) > (grade - newGradeOffset)
                || data.levelGrades[index] == '-')
            {
			    data.levelGrades[index] = grade;
            }
		}
	}

    public char GetGradeFromLevelUnityIndex(uint unityLevelIndex)
    {
        uint index = unityLevelIndex - firstLevel;
        return GetGradeFromLevel(index);
    }

    public char GetGradeFromLevel(uint index)
    {
        if (index < numLevels)
        {
            if(data.levelGrades[index] == 0)
            {
                data.levelGrades[index] = '-';
            }
            return data.levelGrades[index];
        }
        else
        {
            return '-';
        }
    }

    public bool IsLevelNowUnlocked(uint index)
    {
        if(index == 0)
        {
            return false; // level 1 is always unlocked
        }
        return (data.levelsUnlocked[(int)index] == false) && (data.levelsCompleted[(int)(index- 1)] == true);
    }

    public void SetLevelUnlocked(uint index)
    {
        data.levelsUnlocked[(int)index] = true;
    }

    #endregion

    #region ArtInfo
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

    public bool IsEncounteredArt(uint artId)
    {
        return (artId < data.artUnlocked.Length && data.artUnlocked.Get((int)artId));
    }

    public bool AddEncounterdArt(ArtFileInfo artInfo)
    {
        CheckArtInfoInitilized();
        // element => element.artFileName == artInfo.artFileName is a lambda function
        if (!data.encounteredArt.Exists(element => element.artFileName == artInfo.artFileName))
        {
            data.encounteredArt.Add(artInfo);
            data.artUnlocked.Set(artInfo.id, true);
            return true;
        }
        return false;
    }
    #endregion

    #region LeaderBoardStuff

    public bool CheckLeaderBoard(int unityScenID, char grade, double time, ref int leaderBoardSpot)
    {
        char gradeOffset = (grade == 'S') ? (char)19 : (char)0;

        for (int i = 0; i < leaderBoardSpots; ++i)
        {

            char leaderBoardGradeOffset = (data.leaderBoard[i] != null && data.leaderBoard[i].grade == 'S') ?  (char)19 : (char)0;

            if (data.leaderBoard[i] == null || (data.leaderBoard[i].grade - leaderBoardGradeOffset) > (grade - gradeOffset) || 
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

    #endregion

    #region CurrencyInfo
    public int GetPlayerCurrency()
    {
        return data.playerCurrency;
    }

    public void SetPlayerCurrency(int playerCurrency)
    {
        data.playerCurrency = playerCurrency;
    }
    #endregion

    #region PrestigeLevelInfo

    // checks to see if the player has enough exp to level up, if they do it changes the level to the next
    // level and changes the required exp accordingly.
    public bool CheckIfLeveledUp()
    {
        bool result = data.prestigeLevelData.curExp >= data.prestigeLevelData.requiredExpToLevel;

        if(result)
        {
            ++data.prestigeLevelData.level;
            data.prestigeLevelData.curExp -= data.prestigeLevelData.requiredExpToLevel;
            data.prestigeLevelData.requiredExpToLevel =(int)(data.prestigeLevelData.requiredExpToLevel * expModifer);
        }

        return result;
    }

    public void AddExp(int exp)
    {
        data.prestigeLevelData.curExp += exp;
    }

    public int GetCurExp()
    {
        return data.prestigeLevelData.curExp;
    }

    public int GetRequiredExpToLevel()
    {
        return data.prestigeLevelData.requiredExpToLevel;
    }

    public int GetCurLevel()
    {
        return data.prestigeLevelData.level;
    }

    public float GetExpModifier()
    {
        return expModifer;
    }

    public void GetCopyOfPrestigeLevelData(ref PrestigeLevelData _data)
    {
        _data.curExp = data.prestigeLevelData.curExp;
        _data.level = data.prestigeLevelData.level;
        _data.requiredExpToLevel = data.prestigeLevelData.requiredExpToLevel;
    }

    #endregion

    public SettingsData GetSettingsData()
    {
        if(data.settings == null)
        {
            data.settings = new SettingsData();
        }
        return data.settings;
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
        data.versionNum = saveFileVersionNumber;
        data.settings = new SettingsData();

        data.leaderBoard = new LeaderBoardInfo[leaderBoardSpots];

        data.prestigeLevelData = new PrestigeLevelData();

        LoadEquipment();
        data.playerCurrency = 0;

        Stats defaultAttire = new Stats();
        defaultAttire.type = EquipmentTypes.attire;
        defaultAttire.stat = 3.0f;
        data.currentEquipment[(int)EquipmentTypes.attire] = defaultAttire;

        data.levelGrades = new char[numLevels];
        data.lastLevelUnlocked = 0;
        data.levelsCompleted = new BitArray((int)numLevels, false);
        data.levelsUnlocked = new BitArray((int)numLevels, false);
        // KIMS Second hack
        data.artUnlocked = new BitArray(numOfPaintingsInGame, false);

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

    public float versionNum = 0.0f;

    public bool firstPlay = true;
    public bool hasPlayedTut = false;
    public bool hasPlayedPaintingTut = false;
    public bool hasPlayerMoved = false;

    public int playerCurrency = 0;

    // Inventory information
    public int[] numTools = new int[(int)ToolTypes.eToolMAX];
    public uint numHints = 0;
	
    public List<Stats> playerEquipment;
    public List<Stats> storeEquipment;
    public Stats[] currentEquipment = new Stats[(int)EquipmentTypes.MAX];


    // Level information
    public char[] levelGrades;
    public BitArray levelsCompleted;
    public BitArray levelsUnlocked;
    public BitArray artUnlocked;
    public uint lastLevelUnlocked = 0;

    // LeaderBoardInformation
    public LeaderBoardInfo [] leaderBoard;

    // Settings information
    public SettingsData settings;

    // art Info
    public List<ArtFileInfo> encounteredArt;

    //leveling system
    public PrestigeLevelData prestigeLevelData;
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
    public const char noGrade = (char)125; // this number/char is just 
                                           //used to indicate that this leaderboard has no actual grade

    public LeaderBoardInfo()
    {
        name = "Empty";
        grade = noGrade;
    }

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

[Serializable]
public class PrestigeLevelData
{
    public int level = 1;
    public int requiredExpToLevel = 100;
    public int curExp = 0;
}