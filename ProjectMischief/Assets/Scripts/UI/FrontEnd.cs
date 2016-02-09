using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FrontEnd : UIControl
{
    public string levelNameFile = "LevelNames";
    public GameObject leaderBoardMenu;
    public Text leaderBoardText;
    public string leaderBoardSpacer = "\t\t";

    // for the art comprendium
    public Image art;
    public Text artName;
    public Text artInfo1;
    public Text artInfo2;
    public Text artInfo3;
    public GameObject nextButton;
    public GameObject prevButton;
    int currentContextID = 0;
    int maxContextID;

    PersistentSceneData sceneData;

    FrontEnd () : base (UITypes.frontEnd, -1)
    { }

    void Start()
    {
        sceneData = PersistentSceneData.GetPersistentData();

        leaderBoardMenu.SetActive(false);
        TextAsset text = Resources.Load<TextAsset>( levelNameFile );
        char[] delim = new char[] { '\r', '\n' };
        string[] LevelNames = text.text.Split(delim, System.StringSplitOptions.RemoveEmptyEntries);

        // LeaderBoard Stuff
        leaderBoardText.text = "";
        const int kSec = 60; // num of seconds per minute;

        LeaderBoardInfo[] leaderBoard = sceneData.GetLeaderBoard();
        for (int i = 0; i < leaderBoard.Length; ++i)
        {
            if (leaderBoard[i] == null)
            {
                return;
            }
            LeaderBoardInfo curInfo = leaderBoard[i];
            leaderBoardText.text = leaderBoardText.text + (i + 1) + ")" + leaderBoardSpacer + LevelNames[curInfo.level] +
                leaderBoardSpacer + curInfo.grade + leaderBoardSpacer + string.Format("{0}:{1:00}",
                (int)(curInfo.time / kSec), (int)(curInfo.time % kSec)) + "\n";
        }

        maxContextID = sceneData.GetEncounteredArtCount() - 1;
        UpdateUI();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void LoadShop()
    {
        Application.LoadLevelAdditive( "UIStore" );
    }

	public void LoadLevelSelect()
	{
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

    void UpdateUI()
    {
        ArtFileInfo artFileInfo = sceneData.GetArtInfo(currentContextID);
        art.sprite = Resources.Load<Sprite>(artFileInfo.artFileName);
        artName.text = artFileInfo.name;
        artInfo1.text = "Created by " + artFileInfo.artist;
        artInfo2.text = "Created in " + artFileInfo.year;
        artInfo3.text = artFileInfo.description;

        nextButton.SetActive(currentContextID != maxContextID);
        prevButton.SetActive(currentContextID != 0);
    }

}
