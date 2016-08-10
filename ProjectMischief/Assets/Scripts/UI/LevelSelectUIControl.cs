using UnityEngine;
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

    [MultilineAttribute]
    public string levelGradeWithGradeString = "\nHighest Grade\n";
    [MultilineAttribute]
    public string levelGradeWithNoGradeString = "\nHighest Grade\n-";

    // private
	string[] LevelNames;

    uint firstLevel;
    uint numLevels;

    Text[] levelButtonTexts;
    uint curLevel = 0;
    uint lastLevelUnlocked;
    Image buttonImage;
    PersistentSceneData data;
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
        data = PersistentSceneData.GetPersistentData();
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

        lastLevelUnlocked = data.GetLastLevelUnlocked();

        UpdateUI();
    }

	    // funtion to update ui
	void UpdateUI()
	{
		prevButton.SetActive( curLevel != 0 );
        uint neededLevelsForNextButton =  curLevel + (uint)levelButtons.Length;
        nextButton.SetActive(neededLevelsForNextButton < numLevels);


        for (int i = 0; i < levelButtons.Length; ++i)
		{
            if (curLevel + i < numLevels)
			{

                buttonImage = levelButtons[i].GetComponent<Image>();
                levelButtons[i].SetActive(true);
				

                Color buttonimagecolor =  new Color(0, 0, 0, 1);
                Color buttonimagecolorfade =  new Color(0, 0, 0, 0.5f);
                Button button = levelButtons[i].GetComponent<Button>();

                if (curLevel + i <= lastLevelUnlocked)
                {
                    buttonImage.color = buttonimagecolor;
                    button.interactable = true;
                    levelButtonTexts[i].text = LevelNames[curLevel + i] + levelGradeWithGradeString 
                        + data.GetGradeFromLevel((uint)(curLevel + i));
                }

                else
                {
                    buttonImage.color = buttonimagecolorfade;
                    button.interactable = false;
                    levelButtonTexts[i].text = LevelNames[curLevel + i] +levelGradeWithNoGradeString;
                }
			}
			else
			{
				levelButtons[i].SetActive(false);
			}
		}
	}

}

