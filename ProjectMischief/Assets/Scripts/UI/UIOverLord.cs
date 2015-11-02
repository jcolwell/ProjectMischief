using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UITypes
{
    Correction,
    level,
    study,
    grading,
    UIMAX
}

public class UIOverLord : MonoBehaviour 
{
    static public bool gameIsPaused = false;
    static public UIOverLord instance = null;
     UIManger [] uiInstances = new UIManger[(int)UITypes.UIMAX]; 
    
    void Start()
    {
        if (instance == null)
        {
            gameIsPaused = false;
            Application.LoadLevelAdditive("UILevel");
            instance = this;
        }
    }

    public void RegisterUI(GameObject ui, UITypes type)
    {
        UIManger temp = ui.GetComponent<UIManger>();
        uiInstances[(int)type] = temp;
    }

    public void UnRegisterUI(UITypes type)
    {
        uiInstances[(int)type] = null;
    }

    public float GetTimeElapsed()
    {
        LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
        return levelUI.GetTimeElapsed();
    }

    public void EndLevel()
    {
        LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
        levelUI.TurnTimerOff();

        Application.LoadLevelAdditive("UIGrading");
    }

    

}
