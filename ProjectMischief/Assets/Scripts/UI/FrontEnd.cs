using UnityEngine;
using System.Collections;

public class FrontEnd : UIControl
{
    public string levelNameFile = "LevelNames";
    string[] LevelNames = null;

    FrontEnd () : base (UITypes.frontEnd, -1)
    { }

    void Start()
    {
        TextAsset text = Resources.Load<TextAsset>( levelNameFile );
        char[] delim = new char[] { '\r', '\n' };
        LevelNames = text.text.Split( delim, System.StringSplitOptions.RemoveEmptyEntries );
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
}
