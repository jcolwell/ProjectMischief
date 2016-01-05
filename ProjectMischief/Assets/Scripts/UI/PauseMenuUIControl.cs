using UnityEngine;
using System.Collections;

public class PauseMenuUIControl : UIControl 
{
    PauseMenuUIControl() : base(UITypes.pauseMenu)
    { }

    public void GoToMainMenu()
    {
        Application.LoadLevel( "FrontEnd" );
    }

    public void BringUpSettings()
    {
        UIManager.instance.LoadSettings();
    }

    public void RestartLevel()
    {
        UIManager.instance.CloseAllUI();
        Application.LoadLevel( Application.loadedLevel );
    }

    protected override void DurringCloseUI()
    {
        UIManager.instance.TogglePauseButtonActive();
    }
}
