using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FrontEnd : UIControl
{
    public string levelNameFile = "LevelNames";
    public GameObject leaderBoardMenu;
    public Text leaderBoardNameText;
    public string leaderBoardNameHeader = "Name";
    public Text leaderBoardLevelText;
    public string leaderBoardLevelHeader = "Level";
    public Text leaderBoardGradeText;
    public string leaderBoardGradeHeader = "Grade";
    public Text leaderBoardTimeText;
    public string leaderBoardTimeHeader = "Time";

    public GameObject levelSelect;
    public GameObject introObject;
    public GameObject levelLoader;

    // for the art comprendium
    public Image art;
    public Text artName;
    public Text artInfo1;
    public Text artInfo2;
    public Text artInfo3;
    public GameObject nextButton;
    public GameObject prevButton;
    public string tutLevel;
    int currentContextID = 0;
    int maxContextID;

    PersistentSceneData sceneData;

    FrontEnd () : base (UITypes.frontEnd, -1)
    { }

    void Start()
    {
        UIManager.instance.ToggleLevelSelect(false);

        sceneData = PersistentSceneData.GetPersistentData();

        leaderBoardMenu.SetActive(false);
        TextAsset text = Resources.Load<TextAsset>( levelNameFile );
        char[] delim = new char[] { '\r', '\n' };
        string[] LevelNames = text.text.Split(delim, System.StringSplitOptions.RemoveEmptyEntries);

        // LeaderBoard Stuff
        leaderBoardNameText.text = leaderBoardNameHeader;
        leaderBoardLevelText.text = leaderBoardLevelHeader;
        leaderBoardGradeText.text = leaderBoardGradeHeader;
        leaderBoardTimeText.text = leaderBoardTimeHeader;
        const int kSec = 60; // num of seconds per minute;

        LeaderBoardInfo[] leaderBoard = sceneData.GetLeaderBoard();
        for (int i = 0; i < leaderBoard.Length; ++i)
        {
            if (leaderBoard[i] == null || leaderBoard[i].grade == LeaderBoardInfo.noGrade)
            {
                break;
            }
            LeaderBoardInfo curInfo = leaderBoard[i];
            leaderBoardNameText.text = leaderBoardNameText.text + "\n" + curInfo.name;
            leaderBoardLevelText.text = leaderBoardLevelText.text + "\n" + LevelNames[curInfo.level];
            leaderBoardGradeText.text =leaderBoardGradeText.text + "\n" + curInfo.grade;
            leaderBoardTimeText.text = leaderBoardTimeText.text + "\n" + string.Format("{0}:{1:00}",
                (int)(curInfo.time / kSec), (int)(curInfo.time % kSec));
        }

        UpdateUI();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void OpenArtGallery()
    {
        //SceneManager.LoadScene("ArtGallery", LoadSceneMode.Single);
        UIManager.instance.CloseAllUI();

        LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();
        loader.LoadLevel("ArtGallery");
    }

    public void OpenNewspaper()
    {
        //SceneManager.LoadScene("ArtGallery", LoadSceneMode.Single);

        UIManager.instance.LoadNewspaperUI();
    }

    public void LoadShop()
    {
        UIManager.instance.LoadStoreUI();
    }

	public void LoadLevelSelect()
	{
        //levelSelect.SetActive(true);
		UIManager.instance.LoadLevelSelect();
	}

    public void BringUpSettings()
    {
        UIManager.instance.LoadSettings();
    }

    public void BringUpLeaderBoard()
    {
        leaderBoardMenu.SetActive(true);
    }

    public void NextArt()
    {
        if (currentContextID < maxContextID)
        {
            ++currentContextID;
        }
        UpdateUI();
    }

    public void PrevArt()
    {
        if (currentContextID > 0)
        {
            --currentContextID;
        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        maxContextID = sceneData.GetEncounteredArtCount() - 1;
        currentContextID = (maxContextID < currentContextID) ? 0 : currentContextID;

        ArtFileInfo artFileInfo = sceneData.GetArtInfo(currentContextID);
        art.sprite = Resources.Load<Sprite>(artFileInfo.artFileName);
        artName.text = artFileInfo.name;
        artInfo1.text = "Created by " + artFileInfo.artist;
        artInfo2.text = "Created in " + artFileInfo.year;
        artInfo3.text = artFileInfo.description;

        nextButton.SetActive(currentContextID != maxContextID);
        prevButton.SetActive(currentContextID != 0);
    }

    public void PlayButton()
    {
        if(sceneData.CheckIfPlayerHasPlayedTut())
        {
            LoadLevelSelect();
        }
        else
        {
            sceneData.SetHasPlayedTut(true);
            introObject.SetActive(true);
            IntroControl.TurnOnIntro();
            IntroControl.SetIntroToLoadLevelWhenDone(tutLevel);
        }
    }

    public void ActivateIntroObject()
    {
        introObject.SetActive(true);
    }
}
