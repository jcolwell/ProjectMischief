using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;

public class GradingUIControl : UIControl 
{
    // public
    public GameObject levelLoader;

    public GameObject nextButton;
    public GameObject backButton;
    public GameObject unlockedButton;
    public GameObject leaderBoard;
    public Text unlockedArtist;
    public Text unlockedFunFact;

    public GameObject gradeSubMenu;
    public GameObject answerSubMenu;
    public GameObject levelSubMenu;

    public GameObject paintingUnlockButton;
    public float paintingUnlockTimeBeforeReActivation;

        //All Grading Text
    public Text correctPaintingNameText;
    public Text incorrectPaintingNameText;
    public Text correctPaintingYearText;
    public Text incorrectPaintingYearText;
    public Text correctPaintingArtistText;
    public Text incorrectPaintingArtistText;
    public Text coinsEarnedText;
    public Text coinsEarnedTitleText;
    public Text gradeTitleText;
    public Text gradeText;
    public Text timeText;
    public Text timeTitleText;
    // answer detail text (text shwing how they got the grade)
    public Text correctAnswerTitleText;
    public Text correctAnswerDetailText;
    public Text alertDeductionTitleText;
    public Text alertDeductionDetailText;
    public Text captureDeductionTitleText;
    public Text captureDeductionDetailText;
    public Text totalTitleText;
    public Text totalDetailText;

    public Image art;
    public Image unlockedArt;
    public AudioClip song;
    public InputField inputField;
    public Text output;

    //Newpaper shit
    public Text headline;
    public Text newsInfo;

    public string currencyEarnedNotificationText = "You Earned ";
    public string currencyName = " Coins";

    const int levelIncremntToGetRewards = 5;

    #region LevelUpSubMenuObjects
    public Image levelBarBack;
    public Image levelBarFront;
    public Text levelInfoText;
    [Range(0.0f, 100.0f)]
    public float levelBarSpeed = 1.0f; // speed is measuered in percentage per second

    public GameObject levelUpPopUp;

    public GameObject rewardInfoObject;

    public Text smokeBombRewardText;
    public Text mirrorRewardText;
    public Text zapperRewardText;
    public Text hintRewardText;
    #endregion

    // presitiege level stuff
    public RewardInfo [] levelRewards;

    //private
    uint currentContextID;
    int currentUnlockedPainting = 0;
    uint maxContextID;
    int coinsEarned;
    int coinsEarnedIn;
    int textIn;
    List<ArtContext> PaintingQueue = new List<ArtContext>();
    InputField.SubmitEvent se;
    PersistentSceneData data;
    string playerName;

    Text[] allText;
    int leaderBoardSpot = -1;
    float timeElapsed;
    float paintingtimeElapsed;
    float timeBeforeReActivation = 0.5f;
    bool hasPlacedInLeaderBoard = false;
    bool isleaderBoardActive;
    bool hasSeenLevelProgress = false;
    bool hasShownRewards = false;
    bool hasShownLevelUp = false;

    BackgroundMusicManager manager;

        //Perstiege level stuff
    bool hasLeveledUp = false;
    PrestigeLevelData oldPrestigeLevelData = new PrestigeLevelData();
    PrestigeLevelData newPrestigeLevelData = new PrestigeLevelData();
    float levelUpbarMaxPercent;
    int expMod = 30;
    //Enums
    enum text
    {
        timeTitle = 0,
        time,
        coinsTitle,
        coins,
        cAnswerTitle,
        cAnswerDetail,
        alertTitle,
        alertDetail,
        captureTitle,
        captureDetail,
        totalTitle,
        totalDetail,
        gradeTitle,
        grade,
        cName,
        iName,
        cArtist,
        iArtist,
        cYear,
        iYear,
        textMAX
    }

    struct NewspaperArticle
    {
        public string heading;
        public string info;
        public char grade;
    }

    struct GradesArticle
    {
        public string Type;
        public string info;
        public float grade;
    }

    // public
    public GradingUIControl()
        : base(UITypes.grading)
    { }

        // Functions for buttons
    public void ToMenu()
    {
        UIManager.instance.CloseAllUI();
        LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();
        loader.LoadLevel("FrontEnd");
    }

    public void RetryLevel()
    {
        UIManager.instance.CloseAllUI();
        LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();

        loader.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();

        if (UIManager.instance.GetLoadlevelWithString())
        {
            UIManager.instance.CloseAllUI();
            string nextlevel = UIManager.instance.GetNextLevelToLoad();
            if( nextlevel == null )
            {
                loader.LoadLevel("FrontEnd");
            }
            else
            {
                loader.LoadLevel(nextlevel);
            }
        }
        else 
        {
            int nextLevelIndex = UIManager.instance.GetNextLevelToLoadIndex();
            if(nextLevelIndex == -1)
            {
                LoadLevelSelect();
                return;
            }
            UIManager.instance.CloseAllUI();
            loader.LoadLevel(nextLevelIndex);
        }
    }

	public void LoadLevelSelect()
	{
		UIManager.instance.LoadLevelSelect();
	}

    public void NextArt()
    {
        if( currentContextID < maxContextID )
        {
            ++currentContextID;
        }
        UpdateUI();
    }

    public void PrevArt()
    {
        if( currentContextID > 0 )
        {
            --currentContextID;
        }
        UpdateUI();
    }

    public void NextUnlocked()
    {
        if (PaintingQueue.Count > 0)
        {
            unlockedArt.sprite = PaintingQueue[currentUnlockedPainting].art;
            unlockedArtist.text = PaintingQueue[currentUnlockedPainting].correctChoices[(int)ArtFields.ePainting];
            unlockedFunFact.text = PaintingQueue[currentUnlockedPainting].description;
            PaintingQueue.RemoveAt(currentUnlockedPainting);
        }
        else
        {
            unlockedButton.SetActive(false);

            // the spot to deal with leader board shit
            if(hasPlacedInLeaderBoard)
            {
                EnterLeaderBoardInfo();
            }
        }
    }

    public void SwitchToGradeSubMenu()
    {
        gradeSubMenu.SetActive(true);
        answerSubMenu.SetActive(false);
        levelSubMenu.SetActive(false);
    }

    public void SwitchToAnswerSubMenu()
    {
        if(!hasSeenLevelProgress)
        {
            gradeSubMenu.SetActive(false);
            answerSubMenu.SetActive(false);
            levelSubMenu.SetActive(true);

            hasSeenLevelProgress = true;
        }
        else
        {
            gradeSubMenu.SetActive(false);
            answerSubMenu.SetActive(true);
            levelSubMenu.SetActive(false);
        }
    }

    // Private
    void EnterLeaderBoardInfo()
    {
        leaderBoard.SetActive(true);
        isleaderBoardActive = true;
    }

	void Start ()
    { 
        data = PersistentSceneData.GetPersistentData ();
        se = new InputField.SubmitEvent();

        correctAnswerDetailText.text = ArtManager.instance.GetGrade() + " / " + ArtManager.instance.GetGradeMax();
        alertDeductionDetailText.text = (ArtManager.instance.GetLazerPenalty() + 
            ArtManager.instance.GetCameraPenalty()).ToString();
        captureDeductionDetailText.text = ArtManager.instance.GetGuardPenalty().ToString();
        totalDetailText.text = ArtManager.instance.GetFinalGrade() + " / " + ArtManager.instance.GetGradeMax();

        allText = new Text[(int)text.textMAX];
        allText[(int)text.cAnswerTitle] = correctAnswerTitleText;
        allText[(int)text.cAnswerDetail] = correctAnswerDetailText;
        allText[(int)text.alertTitle] = alertDeductionTitleText;
        allText[(int)text.alertDetail] = alertDeductionDetailText;
        allText[(int)text.captureTitle] = captureDeductionTitleText;
        allText[(int)text.captureDetail] = captureDeductionDetailText;
        allText[(int)text.totalTitle] = totalTitleText;
        allText[(int)text.totalDetail] = totalDetailText;
        allText[(int)text.gradeTitle] = gradeTitleText;
        allText[(int)text.grade] = gradeText;
        allText[(int)text.timeTitle] = timeTitleText;
        allText[(int)text.time] = timeText;
        allText[(int)text.coinsTitle] = coinsEarnedTitleText;
        allText[(int)text.coins] = coinsEarnedText;
        allText[(int)text.cName] = correctPaintingNameText;
        allText[(int)text.iName] = incorrectPaintingNameText;
        allText[(int)text.cArtist] = correctPaintingArtistText;
        allText[(int)text.iArtist] = incorrectPaintingArtistText;
        allText[(int)text.cYear] = correctPaintingYearText;
        allText[(int)text.iYear] = incorrectPaintingYearText;

        for(int i = 0; i < allText.Length; ++i)
        {
            allText[i].enabled = false;
        }

        List<NewspaperArticle> newspaperArticles = new List<NewspaperArticle>();
        TextAsset paperText = Resources.Load<TextAsset>("Newspaper");
        newspaperArticles = LoadXMLFromString(paperText.ToString());

        List<GradesArticle> gradeArticles = new List<GradesArticle>();
        TextAsset xmlText = Resources.Load<TextAsset>("Grade");
        gradeArticles = XMLFromString(xmlText.ToString());

        se.AddListener(SubmitInput);
        inputField.onEndEdit = se;

        currentContextID = 0;
        maxContextID = ArtManager.instance.GetNumPaintings() - 1;

        // fill up the text that will not change
        char letterGrade = ArtManager.instance.GetLetterGrade();
        int correctChoices = ArtManager.instance.GetCorrectChoices();
        coinsEarned = UIManager.instance.GetCoinsEarned() + correctChoices;
        //print("Coins Earned " + coinsEarned);
        gradeText.text = letterGrade.ToString();

        float paintingTotalNum = (float)ArtManager.instance.GetNumPaintings();

        float paintingNumAverage = (AnswerTracker.instance.GetNumCorrectAnswers(ArtFields.ePainting) / paintingTotalNum) * 100.0f;
        float artistNumAverage = (AnswerTracker.instance.GetNumCorrectAnswers(ArtFields.eArtist) / paintingTotalNum) * 100.0f;
        float yearNumAverage = (AnswerTracker.instance.GetNumCorrectAnswers(ArtFields.eYear) / paintingTotalNum) * 100.0f;

        string paintingAverage = "/0";
        string artistAverage = "/0";
        string yearAverage = "/0";

        for (int i = 0; i < gradeArticles.Count; ++i)
        {
            float grade = gradeArticles[i].grade;

            if (gradeArticles[i].Type == "Painting")
            {
                if(paintingNumAverage >= grade)
                {
                    paintingAverage = gradeArticles[i].info;
                }
            }
            else if (gradeArticles[i].Type == "Artist")
            {
                if (artistNumAverage >= grade)
                {
                    artistAverage = gradeArticles[i].info;
                }
            }
            else if (gradeArticles[i].Type == "Year")
            {
                if (yearNumAverage >= grade)
                {
                    yearAverage = gradeArticles[i].info;
                }
            }
        }

        for (int i = 0; i < newspaperArticles.Count; ++i)
        {
            if(letterGrade.ToString() == newspaperArticles[i].grade.ToString())
            {
                headline.text = newspaperArticles[i].heading;
                newsInfo.text = newspaperArticles[i].info + paintingAverage + ", " + artistAverage + ", " + yearAverage + ".";
            }
        }

        #region PrestiegeLevelStuff
        //Handle perstiege level stuff
            //Grab prestigeLevelData before it get changed when exp is added and the level is checked 
        data.GetCopyOfPrestigeLevelData(ref oldPrestigeLevelData);

            // the following fuctions will change the values of the prestigeLevelData stored in persitainscenedata
            // note: correct chocies is currently being used to determine exp, might want to change that to coins earned
        data.AddExp(correctChoices * expMod);
        hasLeveledUp = data.CheckIfLeveledUp();

            // get new prestiege level data
        data.GetCopyOfPrestigeLevelData(ref newPrestigeLevelData);
        #endregion


        // mark level as completed
        data.SetLevelCompleted((uint)SceneManager.GetActiveScene().buildIndex, letterGrade);
        data.SetPlayerCurrency(data.GetPlayerCurrency() + coinsEarned );

        for (uint i = 0; i < maxContextID + 1; ++i)
        {
            ArtContext currentArt = ArtManager.instance.GetPainting(i);
            ArtFileInfo currentArtFileInfo = new ArtFileInfo();

            currentArtFileInfo.name = currentArt.correctChoices[(int)ArtFields.ePainting];
            currentArtFileInfo.artFileName = currentArt.artFileName;
            currentArtFileInfo.id = currentArt.artID;
            currentArtFileInfo.description = currentArt.description;
            currentArtFileInfo.artist = currentArt.correctChoices[(int)ArtFields.eArtist];
            currentArtFileInfo.year = currentArt.correctChoices[(int)ArtFields.eYear];

            if (data.AddEncounterdArt(currentArtFileInfo))
            {
                PaintingQueue.Add(currentArt);
            }
        }

        double time = UIManager.instance.GetTimeElapsed();
        const int kSec = 60; // num of seconds per minute;
        timeText.text = string.Format("{0}:{1:00}", (int)(time / kSec), (int)(time % kSec));

        hasPlacedInLeaderBoard = data.CheckLeaderBoard(SceneManager.GetActiveScene().buildIndex, letterGrade, time, ref leaderBoardSpot);

        if (PaintingQueue == null || PaintingQueue.Count == 0)
        {
            unlockedButton.SetActive(false);

            if (hasPlacedInLeaderBoard)
            {
                EnterLeaderBoardInfo();
            }
        }



        #region levelSubMenuInit

        rewardInfoObject.SetActive(false);

        smokeBombRewardText.gameObject.SetActive(false);
        mirrorRewardText.gameObject.SetActive(false);
        zapperRewardText.gameObject.SetActive(false);
        hintRewardText.gameObject.SetActive(false);

        levelInfoText.text = "Level " + oldPrestigeLevelData.level;
        levelBarFront.type = Image.Type.Filled;
        levelBarFront.fillMethod = Image.FillMethod.Horizontal;

        float div = 1.0f / (float)oldPrestigeLevelData.requiredExpToLevel;
        levelBarFront.fillAmount = (float)oldPrestigeLevelData.curExp * div;

        levelUpbarMaxPercent = (hasLeveledUp) ? 1.0f : (float)newPrestigeLevelData.curExp * div;
        #endregion

        //Analyitics
        Analytics.CustomEvent("FinishedLevel", new Dictionary<string, object>
        {
            {"PlayerID", SystemInfo.deviceUniqueIdentifier.ToString() },
            {"ElapsedTime", time },
            {"CoinsEarned", coinsEarned  },
            {"LevelNumber", SceneManager.GetActiveScene().name },
            {"LetterGrade", letterGrade.ToString() }
        });

        // set the text for the text that could change
        UpdateUI();
        
        coinsEarnedText.text = currencyEarnedNotificationText + data.GetPlayerCurrency() + currencyName;
    }

    void Update()
    {
        if(paintingUnlockButton.activeSelf && paintingtimeElapsed >= paintingUnlockTimeBeforeReActivation)
        {
            paintingUnlockButton.SetActive(false);
        }

        if ( timeElapsed >= timeBeforeReActivation)
        {
            if (!leaderBoard.activeSelf && !unlockedButton.activeSelf && textIn < allText.Length)
            {

                if (gradeSubMenu.activeSelf)
                {
                    allText[textIn].enabled = true;
                    if (textIn < (int)text.grade)
                    {
                        ++textIn;
                    }
                }
                else if (answerSubMenu.activeSelf)
                {
                    if(textIn < (int)text.grade)
                    {
                        textIn = (int)text.grade + 1;
                    }
                    allText[textIn].enabled = true;
                    
                    ++textIn;
                }
                
            }
            timeElapsed = 0.0f;
        }

        timeElapsed += Time.unscaledDeltaTime;
        paintingtimeElapsed += Time.unscaledDeltaTime;

        if (levelSubMenu.activeSelf)
        {
            UpdateLevelUI();
        }
    }


        // functions to update UI
    void UpdateUI()
    {
        ArtContext curContext = ArtManager.instance.GetPainting(currentContextID);
        art.sprite = Resources.Load<Sprite>(ArtManager.artFilePath + curContext.artFileName);

        correctPaintingNameText.text = curContext.correctChoices[(int)ArtFields.ePainting];
        correctPaintingYearText.text = curContext.correctChoices[(int)ArtFields.eYear];
        correctPaintingArtistText.text = curContext.correctChoices[(int)ArtFields.eArtist];

        incorrectPaintingArtistText.text = "";
        incorrectPaintingNameText.text = "";
        incorrectPaintingYearText.text = "";

        if (curContext.currentChoices[(int)ArtFields.ePainting] != curContext.correctChoices[(int)ArtFields.ePainting])
        {
            incorrectPaintingNameText.text = curContext.currentChoices[(int)ArtFields.ePainting];
        }

        if (curContext.currentChoices[(int)ArtFields.eArtist] != curContext.correctChoices[(int)ArtFields.eArtist])
        {
            incorrectPaintingArtistText.text = curContext.currentChoices[(int)ArtFields.eArtist];
        }

        if (curContext.currentChoices[(int)ArtFields.eYear] != curContext.correctChoices[(int)ArtFields.eYear])
        {
            incorrectPaintingYearText.text = curContext.currentChoices[(int)ArtFields.eYear];
        }

        nextButton.SetActive(currentContextID != maxContextID);
        backButton.SetActive(currentContextID != 0);

    }

    List<NewspaperArticle> LoadXMLFromString(string xmlFile)
    {
        XDocument doc = XDocument.Parse(xmlFile);

        IEnumerable<XElement> articles = from NewspaperList in doc.Root.Elements()
                                         select NewspaperList;

        List<NewspaperArticle> infos = new List<NewspaperArticle>();
        foreach (XElement article in articles)
        {
            NewspaperArticle info = new NewspaperArticle();
            info.heading = article.Element("Headline").Value;
            info.info = article.Element("TextInfo").Value;
            info.grade = char.Parse(article.Element("Grade").Value);

            infos.Add(info);
        }
        return infos;
    }

    List<GradesArticle> XMLFromString(string xmlFile)
    {
        XDocument doc = XDocument.Parse(xmlFile);

        IEnumerable<XElement> articles = from GradeList in doc.Root.Elements()
                                         select GradeList;

        List<GradesArticle> infos = new List<GradesArticle>();
        foreach (XElement article in articles)
        {
            GradesArticle info = new GradesArticle();
            info.Type = article.Element("Type").Value;
            info.info = article.Element("TextInfo").Value;
            info.grade = float.Parse(article.Element("Grade").Value);

            infos.Add(info);
        }
        return infos;
    }

    void UpdateLevelUI()
    {
        if(levelBarFront.fillAmount < levelUpbarMaxPercent)
        {
            // levelbarspeed is multiplied by 0.01f so that we take levelbarspeed which is in range from 0 - 100
            // to a range of 0 to 1
            levelBarFront.fillAmount += (levelBarSpeed * 0.01f) * Time.unscaledDeltaTime;
        }
        else if(hasLeveledUp)
        {
            levelInfoText.text = "Level " + newPrestigeLevelData.level;
            ShowRewards();
        }
    }

    void ShowRewards()
    {

        if (!hasShownRewards)
        {
            if(!hasShownLevelUp)
            {
                levelUpPopUp.SetActive(true);
                hasShownLevelUp = true;
            }
            else if (newPrestigeLevelData.level % levelIncremntToGetRewards == 0 && !levelUpPopUp.activeSelf)
            {
                rewardInfoObject.SetActive(true);

                int rewardIndex = Mathf.Min(levelRewards.Length - 1, newPrestigeLevelData.level / levelIncremntToGetRewards);
                smokeBombRewardText.text = "SmokeBombs X " + levelRewards[rewardIndex].numSmokeBombs;
                mirrorRewardText.text = "Pocket mirrors X " + levelRewards[rewardIndex].numMirrors;
                zapperRewardText.text = "Camera Zapper X " + levelRewards[rewardIndex].numZappers;
                hintRewardText.text = "Hints X " + levelRewards[rewardIndex].numHints;

                data.IncreaseHints((uint)levelRewards[rewardIndex].numHints);
                data.IncreaseNumTools(ToolTypes.eJammer, levelRewards[rewardIndex].numZappers);
                data.IncreaseNumTools(ToolTypes.eMirror, levelRewards[rewardIndex].numMirrors);
                data.IncreaseNumTools(ToolTypes.eSmokeBomb, levelRewards[rewardIndex].numSmokeBombs);
                hasShownRewards = true;
            }
        }
    }

    void SubmitInput(string arg0)
    {
        string currentText = output.text;
        //string newText = currentText + "\n" + arg0;
        string newText = arg0;
        output.text = newText;
        //inputField.text = newText;
        inputField.ActivateInputField();
        playerName = newText;
        data.SetLeaderBoardName(leaderBoardSpot, playerName);
        leaderBoard.SetActive(false);
        isleaderBoardActive = false;
    }


    protected override void DurringOnEnable()
    {
        manager = UIManager.instance.GetMusicManger();
        manager.ChangeSong( song );
    }

    protected override void DurringCloseUI()
    {
        manager = UIManager.instance.GetMusicManger();
        manager.Pause();
    }
}

// class to hold reward info that is accessible from the editor when used as an array
[System.Serializable]
public class RewardInfo
{
    public int numHints = 1;
    public int numSmokeBombs = 1;
    public int numMirrors = 1;
    public int numZappers = 1;
}