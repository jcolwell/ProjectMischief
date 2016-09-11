using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelSelectUIControl : UIControl
{
    Color transparentColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    // public
	public string levelNameFile = "LevelNames";
    public GameObject levelLoader;

    public GameObject prevButton;
    public GameObject nextButton;

    public GameObject[] levelButtons;
    public Image[] levelButtonLockedImages;

    public Sprite unlockedSprite;

    [MultilineAttribute]
    public string levelGradeWithGradeString = "\nHighestsddf Grade\n";
    [MultilineAttribute]
    public string levelGradeWithNoGradeString = "\n-";
    public float fadeOutDuration = 0.5f;

    // private
	string[] LevelNames;

    uint firstLevel;
    uint numLevels;

    float timeElpased = 0.0f;

    List<int> unlockedImageToFadeOut = new List<int>();

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

    void Update()
    {
        if (unlockedImageToFadeOut.Count > 0)
        {
            float t = timeElpased / fadeOutDuration;
            for (int i = 0; i < unlockedImageToFadeOut.Count; ++i)
            {
                levelButtonLockedImages[unlockedImageToFadeOut[i]].color = Color.Lerp(Color.white, transparentColor, t);
            }

            if (timeElpased > fadeOutDuration)
            {
                unlockedImageToFadeOut.Clear();
            }
            timeElpased += Time.unscaledDeltaTime;
        }
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
                levelButtonLockedImages[i].color = Color.white;

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
                    if(data.IsLevelNowUnlocked((uint)(curLevel + i)))
                    {
                        levelButtonLockedImages[i].sprite = unlockedSprite;
                        unlockedImageToFadeOut.Add(i);
                        data.SetLevelUnlocked((uint)(curLevel + i));
                    }
                    else
                    {
                        levelButtonLockedImages[i].gameObject.SetActive(false);
                    }
                }

                else
                {
                    buttonImage.color = buttonimagecolorfade;
                    button.interactable = false;
                    levelButtonTexts[i].text = LevelNames[curLevel + i] +levelGradeWithNoGradeString;
                    levelButtonLockedImages[i].gameObject.SetActive(true);
                }
			}
			else
			{
				levelButtons[i].SetActive(false);
                levelButtonLockedImages[i].gameObject.SetActive(false);
            }
		}
	}

}

