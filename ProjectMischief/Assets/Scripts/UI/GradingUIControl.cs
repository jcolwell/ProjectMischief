using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GradingUIControl : UIControl 
{
    // public
    public GameObject levelLoader;

    public GameObject nextButton;
    public GameObject backButton;
    public Text IncorrectChoicesText;
    public Image art;
    public AudioClip song;

    public string zeroWrongCorrectionsText = "You made no wrong corrections";
    public string correctCorrectionText = " correct corrections";

    //private
    uint currentContextID;
    uint maxContextID;

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
        loader.LoadLevel(Application.loadedLevel);
    }

    public void LoadNextLevel()
    {
        string nextlevel = UIManager.instance.GetNextLevelToLoad();
        if( nextlevel == null )
        {
            UIManager.instance.CloseAllUI();
            LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();
            loader.LoadLevel("FrontEnd");
        }
        else
        {
            UIManager.instance.CloseAllUI();
            LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();
            loader.LoadLevel(nextlevel);
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
        Text timeElapsed = temp.GetComponent<Text>();

		temp = transform.FindDeepChild("CorrectCorrectionsText").gameObject;
        Text CorrectCorrectionsText = temp.GetComponent<Text>();

        // fill up the text that will not change
        char letterGrade = ArtManager.instance.GetLetterGrade();
        int correctChoices = ArtManager.instance.GetCorrectChoices();
        grade.text = letterGrade.ToString();
        CorrectCorrectionsText.text = "You made " + ArtManager.instance.GetCorrectChanges().ToString() + correctCorrectionText;

		// mark level as completed
		data.SetLevelCompleted ((uint)Application.loadedLevel, letterGrade); 
		data.SetPlayerCurrency(PersistentSceneData.GetPersistentData().GetPlayerCurrency() + correctChoices);

        double time = UIManager.instance.GetTimeElapsed();
        const int kSec = 60; // num of seconds per minute;
        timeElapsed.text = "Time Elapsed\n" + string.Format("{0}:{1:00}", (int)(time / kSec), (int)(time % kSec));

        data.CheckLeaderBoard(Application.loadedLevel, letterGrade, time);

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

        IncorrectChoicesText.text = "";

        for (uint i = 0; i < (int)ArtFields.eMax; ++i )
        {
            if(curContext.currentChoices[i] != curContext.correctChoices[i])
            {
                noIncorrectChoices = false;
                IncorrectChoicesText.text = IncorrectChoicesText.text + "You got the " + fields[i] + 
                    "wrong. The correct " + fields[i] + "was " + curContext.correctChoices[i] + ".\n";
            }
        }

        if (noIncorrectChoices)
        {
            IncorrectChoicesText.text = zeroWrongCorrectionsText;
        }

        nextButton.SetActive(currentContextID != maxContextID);
        backButton.SetActive(currentContextID != 0);
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
