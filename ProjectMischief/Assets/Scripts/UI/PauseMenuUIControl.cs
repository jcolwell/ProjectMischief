using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenuUIControl : UIControl 
{
    // Public
    public GameObject levelLoader;

    // private

    GameObject cameraMap;

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
        loader.LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

        // Overideded functions
    protected override void DurringOnEnable()
    {
        UIManager.instance.PauseGameTime();
        cameraMap = UIManager.instance.GetMapCamera();   
        if( cameraMap != null )
        {
            cameraMap.SetActive( true );
        }
    }


    protected override void DurringCloseUI()
    {
        if(cameraMap != null )
        {
            cameraMap.SetActive( false );
        }

        UIManager.instance.UnPauseGameTime();
        UIManager.instance.TogglePauseButtonActive();
    }
}
