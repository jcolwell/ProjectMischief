using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSelectUIControl : UIControl
{
    // public
	public string levelNameFile = "LevelNames";

    // private
	uint firstLevel;
	uint numLevels;
	string[] LevelNames;

    uint curLevel = 0;
	uint numButtonsForLevels = 3;

	GameObject prevButton;
	GameObject nextButton;

	Text[] levelButtonTexts;
	GameObject[] levelButtons;

    // public
    public LevelSelectUIControl()
        : base(UITypes.levelSelect)
    { }

	    // Functions for buttons
	public void Prev()
	{
		if( (int)curLevel - (int)numButtonsForLevels >= 0 )
		{
			curLevel -= numButtonsForLevels;
		}
		UpdateUI();
	}
	
	public void Next()
	{
		if( curLevel + numButtonsForLevels < numLevels )
		{
			curLevel += numButtonsForLevels;
		}
		
		UpdateUI();
	}
	
	public void GoToLevel(int buttonIndex)
	{
		//TODO: confirm if we need to unlock levels
		UIManager.instance.CloseAllUI();
		Application.LoadLevel((int)curLevel + buttonIndex + (int)firstLevel);
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

        // find Buttons
        prevButton = transform.FindDeepChild( "PrevButton" ).gameObject;
        nextButton = transform.FindDeepChild( "NextButton" ).gameObject;

        levelButtonTexts = new Text[numButtonsForLevels];
        levelButtons = new GameObject[numButtonsForLevels];

        for( int i = 0; i < (int)numButtonsForLevels; ++i )
        {
            levelButtonTexts[i] = transform.FindDeepChild( "level" + (i + 1).ToString() + "Text" ).GetComponent<Text>();
            levelButtons[i] = transform.FindDeepChild( "level" + (i + 1).ToString() ).gameObject;
        }

        UpdateUI();
    }

	    // funtion to update ui
	void UpdateUI()
	{
		prevButton.SetActive( curLevel != 0 );
		nextButton.SetActive( curLevel + numButtonsForLevels < numLevels );


		for(int i = 0; i < (int) numButtonsForLevels; ++i)
		{
			if(curLevel + i < numLevels)
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

