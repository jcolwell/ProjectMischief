using UnityEngine;
using System.Collections;

public class PauseMenuUIControl : UIControl 
{
    // Private
    PauseMenuUIControl() : base(UITypes.pauseMenu)
    { }

    void Awake()
    {
        pauseTimeWhenLoaded = (Time.timeScale > 0.0f ) ? true: false;
        UIManager.gameIsPaused = true;
    }

    void Start()
    {
        GameObject canvasObject = transform.FindDeepChild("Canvas").gameObject;
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.planeDistance = 0.9f;
    }

    // Public
        // Functions for Buttons
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

        // Overideded functions
    protected override void DurringCloseUI()
    {
        UIManager.gameIsPaused = false;
        UIManager.instance.TogglePauseButtonActive();
    }
}
