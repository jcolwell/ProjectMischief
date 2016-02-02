using UnityEngine;
using System.Collections;

public class PauseMenuUIControl : UIControl 
{
    // Public
    public GameObject levelLoader;

    // Private
    PauseMenuUIControl() : base(UITypes.pauseMenu, 4)
    { }


    // Public
        // Functions for Buttons
    public void GoToMainMenu()
    {
        LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();
        loader.LoadLevel("FrontEnd");
    }

    public void BringUpSettings()
    {
        UIManager.instance.LoadSettings();
    }

    public void RestartLevel()
    {
        UIManager.instance.CloseAllUI();

        LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();
        loader.LoadLevel(Application.loadedLevel);
    }

        // Overideded functions
    protected override void DurringOnEnable()
    {
        UIManager.instance.PauseGameTime();
    }


    protected override void DurringCloseUI()
    {
        UIManager.instance.UnPauseGameTime();
        UIManager.instance.TogglePauseButtonActive();
    }
}
