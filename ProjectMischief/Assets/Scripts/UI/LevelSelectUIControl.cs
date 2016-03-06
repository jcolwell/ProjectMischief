﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectUIControl : UIControl
{
    // public
	public string levelNameFile = "LevelNames";
    public GameObject levelLoader;

    public GameObject prevButton;
    public GameObject nextButton;

    public GameObject[] levelButtons;
    // private
	string[] LevelNames;

    uint firstLevel;
    uint numLevels;

    Text[] levelButtonTexts;
    uint curLevel = 0;
    uint lastLevelUnlocked;

    // public
    public LevelSelectUIControl()
        : base(UITypes.levelSelect, 2)
    { }

	    // Functions for buttons
	public void Prev()
	{
		if( (int)curLevel - (int)levelButtons.Length >= 0 )
		{
            curLevel -= (uint)levelButtons.Length;
		}
		UpdateUI();
	}
	
	public void Next()
	{
        if (curLevel + levelButtons.Length < numLevels)
		{
            curLevel += (uint)levelButtons.Length;
		}
		
		UpdateUI();
	}
	
	public void GoToLevel(int buttonIndex)
	{
		//TODO: confirm if we need to unlock levels
        UIManager.instance.CloseAllUI();

        LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();
        loader.LoadLevel((int)curLevel + buttonIndex + (int)firstLevel);
	}

	public void LoadStore()
	{
		UIManager.instance.LoadStoreUI();
	}

    // Private

    void Start()
    {
        PersistentSceneData data = PersistentSceneData.GetPersistentData();
        firstLevel = data.GetFirstLevelUnityIndex();
        numLevels = data.GetNumLevels();

        // load levelNames
        TextAsset text = Resources.Load<TextAsset>( levelNameFile );
        char[] delim = new char[] { '\r', '\n' };
        LevelNames = text.text.Split( delim, System.StringSplitOptions.RemoveEmptyEntries );

        levelButtonTexts = new Text[levelButtons.Length];
        for (int i = 0; i < levelButtons.Length; ++i)
        {
            levelButtonTexts[i] = levelButtons[i].GetComponentInChildren<Text>();
        }

        lastLevelUnlocked = PersistentSceneData.GetPersistentData().GetLastLevelUnlocked();

        UpdateUI();
    }

	    // funtion to update ui
	void UpdateUI()
	{
		prevButton.SetActive( curLevel != 0 );
        uint neededLevelsForNextButton =  curLevel + (uint)levelButtons.Length;
        nextButton.SetActive((neededLevelsForNextButton < numLevels) && (neededLevelsForNextButton <= lastLevelUnlocked));


        for (int i = 0; i < levelButtons.Length; ++i)
		{
            if (curLevel + i < numLevels && curLevel + i <= lastLevelUnlocked)
			{
				levelButtons[i].SetActive(true);
				levelButtonTexts[i].text = LevelNames[curLevel + i];
			}
			else
			{
				levelButtons[i].SetActive(false);
			}
		}
	}

}

