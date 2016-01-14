using UnityEngine;
using System.Collections;

public class PauseMenuUIControl : UIControl 
{
    // Public
    public GameObject levelLoader;

    // private
    bool pausedOnLoad = false;

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
        pauseTimeWhenLoaded = (Time.timeScale > 0.0f) ? true : false;

        pausedOnLoad = UIManager.gameIsPaused;
        UIManager.gameIsPaused = true;
    }


    protected override void DurringCloseUI()
    {
        UIManager.gameIsPaused = pausedOnLoad;
        UIManager.instance.TogglePauseButtonActive();
    }
}
