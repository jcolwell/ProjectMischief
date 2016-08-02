using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;


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

    //All Grading Text
    public Text correctPaintingNameText;
    public Text incorrectPaintingNameText;
    public Text correctPaintingYearText;
    public Text incorrectPaintingYearText;
    public Text correctPaintingArtistText;
    public Text incorrectPaintingArtistText;
    public Text coinsEarnedText;
    public Text GradeText;
    public Text TimeText;

    public Image art;
    public Image unlockedArt;
    public AudioClip song;
    public InputField inputField;
    public Text output;

    public string currencyEarnedNotificationText = "You Earned ";
    public string currencyName = " Coins";


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
    float timeBeforeReActivation = 0.5f;
    bool hasPlacedInLeaderBoard = false;
    bool isleaderBoardActive;
    bool isCoinDoingThings = false;
    
    BackgroundMusicManager manager;

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

    // Private
    void EnterLeaderBoardInfo()
    {
        leaderBoard.SetActive(true);
        isleaderBoardActive = true;
    }

    void Update()
    {
        if ( timeElapsed >= timeBeforeReActivation)
        {
            if (!leaderBoard.activeSelf && !unlockedButton.activeSelf && textIn < allText.Length)
            {
                allText[textIn].enabled = true;
                if(textIn != 2)
                {
                    ++textIn;
                }
                else
                {
                    isCoinDoingThings = true;
                    int stuff = data.GetPlayerCurrency() + coinsEarnedIn;
                    coinsEarnedText.text = currencyEarnedNotificationText + stuff + currencyName;

                    if(coinsEarnedIn < coinsEarned)
                    {
                        coinsEarnedIn++;
                    }
                    else
                    {
                        ++textIn;
                    }
                }
            }
            timeElapsed = 0.0f;
        }

        timeElapsed += Time.unscaledDeltaTime;
    }
    
	void Start ()
    { 
        data = PersistentSceneData.GetPersistentData ();
        se = new InputField.SubmitEvent();

        allText = new Text[9];
        allText[0] = GradeText;
        allText[1] = TimeText;
        allText[2] = coinsEarnedText;
        allText[3] = correctPaintingNameText;
        allText[4] = incorrectPaintingNameText;
        allText[5] = correctPaintingArtistText;
        allText[6] = incorrectPaintingArtistText;
        allText[7] = correctPaintingYearText;
        allText[8] = incorrectPaintingYearText;

        for(int i = 0; i < allText.Length; ++i)
        {
            allText[i].enabled = false;
        }

        se.AddListener(SubmitInput);
        inputField.onEndEdit = se;

        currentContextID = 0;
        maxContextID = ArtManager.instance.GetNumPaintings() - 1;

        // Get relvent objects
		GameObject temp = transform.FindDeepChild("GradeText").gameObject;
        Text  grade = temp.GetComponent<Text>();

		temp = transform.FindDeepChild("TimeElapsedText").gameObject;
        Text timeElapsed1 = temp.GetComponent<Text>();

            // fill up the text that will not change
        char letterGrade = ArtManager.instance.GetLetterGrade();
        int correctChoices = ArtManager.instance.GetCorrectChoices();
        coinsEarned = UIManager.instance.GetCoinsEarned() + correctChoices;
        //print("Coins Earned " + coinsEarned);
        grade.text = letterGrade.ToString();

        // mark level as completed
        data.SetLevelCompleted((uint)SceneManager.GetActiveScene().buildIndex, letterGrade);
        data.SetPlayerCurrency( PersistentSceneData.GetPersistentData().GetPlayerCurrency() + coinsEarned );

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

            if (PersistentSceneData.GetPersistentData().AddEncounterdArt(currentArtFileInfo))
            {
                PaintingQueue.Add(currentArt);
            }
        }

        double time = UIManager.instance.GetTimeElapsed();
        const int kSec = 60; // num of seconds per minute;
        timeElapsed1.text = "Time Elapsed: " + string.Format("{0}:{1:00}", (int)(time / kSec), (int)(time % kSec));

        hasPlacedInLeaderBoard = data.CheckLeaderBoard(SceneManager.GetActiveScene().buildIndex, letterGrade, time, ref leaderBoardSpot);

        if (PaintingQueue == null || PaintingQueue.Count == 0)
        {
            unlockedButton.SetActive(false);

            if (hasPlacedInLeaderBoard)
            {
                EnterLeaderBoardInfo();
            }
        }


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

        // functions to update UI
    void UpdateUI()
    {
        ArtContext curContext = ArtManager.instance.GetPainting(currentContextID);
        art.sprite = curContext.art;

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

    public void Input()
    {

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
