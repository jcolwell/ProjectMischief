using UnityEngine;
using System.Collections;

public class FrontEnd : UIControl
{
    FrontEnd () : base (UITypes.frontEnd, 0)
    { }

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
