using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FrontEnd : UIControl
{
    public string levelNameFile = "LevelNames";
    public GameObject leaderBoardMenu;
    public Text leaderBoardText;
    public string leaderBoardSpacer = "\t\t";

    FrontEnd () : base (UITypes.frontEnd, -1)
    { }

    void Start()
    {
        leaderBoardMenu.SetActive(false);
        TextAsset text = Resources.Load<TextAsset>( levelNameFile );
        char[] delim = new char[] { '\r', '\n' };
        string[] LevelNames = text.text.Split(delim, System.StringSplitOptions.RemoveEmptyEntries);

        // LeaderBoard Stuff
        leaderBoardText.text = "";
        const int kSec = 60; // num of seconds per minute;

        LeaderBoardInfo[] leaderBoard = PersistentSceneData.GetPersistentData().GetLeaderBoard();
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
}
