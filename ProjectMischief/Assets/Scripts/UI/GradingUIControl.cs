using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using System.Collections.Generic;

//paintings[i].artFileName = paintingsFromFile[id].artFileName;

public class GradingUIControl : UIControl 
{
    // public
    public GameObject levelLoader;

    public GameObject nextButton;
    public GameObject backButton;
    public GameObject unlockedButton;
    public Text incorrectChoicesText;
    public Text unlockedArtist;
    public Text coinsEarnedText;
    public Image art;
    public Image unlockedArt;
    public AudioClip song;

    public string currencyEarnedNotificationText = "You Earned ";
    public string currencyName = " Coins";

    public string zeroWrongCorrectionsText = "You made no wrong corrections";
    public string correctCorrectionText = " correct corrections";

    //private
    uint currentContextID;
    int currentUnlockedPainting;
    uint maxContextID;
    int coinsEarned;
    List<ArtContext> PaintingQueue = new List<ArtContext>();

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

    void Update()
    {
        // Debug.Log( scrollRect.verticalNormalizedPosition );
    }
    
	// Private
	void Start ()
    { 
        PersistentSceneData data = PersistentSceneData.GetPersistentData ();

        currentContextID = 0;
        maxContextID = ArtManager.instance.GetNumPaintings() - 1;

        // Get relvent objects
		GameObject temp = transform.FindDeepChild("GradeText").gameObject;
        Text  grade = temp.GetComponent<Text>();

		temp = transform.FindDeepChild("TimeElapsedText").gameObject;
        Text timeElapsed1 = temp.GetComponent<Text>();

        if (PaintingQueue == null)
        {
            unlockedButton.SetActive(false);
        }

            // fill up the text that will not change
            char letterGrade = ArtManager.instance.GetLetterGrade();
        int correctChoices = ArtManager.instance.GetCorrectChoices();
        coinsEarned = UIManager.instance.GetCoinsEarned() + correctChoices;
        //print("Coins Earned " + coinsEarned);
        if (coinsEarnedText != null)
        {
            coinsEarnedText.text = currencyEarnedNotificationText + coinsEarned + currencyName;
        }
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

        data.CheckLeaderBoard(SceneManager.GetActiveScene().buildIndex, letterGrade, time);

        //Analyitics
        Analytics.CustomEvent("FinishedLevel", new Dictionary<string, object>
        {
            {"ElapsedTime", time },
            {"CoinsEarned", coinsEarned  },
            {"LetterGrade", letterGrade },
            {"NumSmokeBombs", data.GetNumTools(ToolTypes.eSmokeBomb) },
            {"NumMirrors", data.GetNumTools(ToolTypes.eMirror) },
            {"NumJammers", data.GetNumTools(ToolTypes.eJammer) },
        });

        // set the text for the text that could change
        UpdateUI();
	}

        // functions to update UI
    void UpdateUI()
    {
        ArtContext curContext = ArtManager.instance.GetPainting(currentContextID);
        art.sprite = curContext.art;

        bool noIncorrectChoices = true;
        string [] fields = new string[(int)ArtFields.eMax];
        fields[(int)ArtFields.ePainting] = "Painting's Name ";
        fields[(int)ArtFields.eYear] = "Painting's year ";
        fields[(int)ArtFields.eArtist] = "artist's Name ";

        incorrectChoicesText.text = "";

        for (uint i = 0; i < (int)ArtFields.eMax; ++i )
        {
            if(curContext.currentChoices[i] != curContext.correctChoices[i])
            {
                noIncorrectChoices = false;
                incorrectChoicesText.text = incorrectChoicesText.text + "You got the " + fields[i] + 
                    "wrong.\nThe correct " + fields[i] + "was " + curContext.correctChoices[i] + ".\n";
            }
        }

        if (noIncorrectChoices)
        {
            incorrectChoicesText.text = zeroWrongCorrectionsText;
        }

        nextButton.SetActive(currentContextID != maxContextID);
        backButton.SetActive(currentContextID != 0);

    }

    public void NextUnlocked()
    {
        if (PaintingQueue.Count > 0)
        {
            unlockedArt.sprite = PaintingQueue[currentUnlockedPainting].art;
            unlockedArtist.text = PaintingQueue[currentUnlockedPainting].correctChoices[(int)ArtFields.ePainting];
            PaintingQueue.RemoveAt(currentUnlockedPainting);
        }
        else
        {
            unlockedButton.SetActive(false);
        }
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
