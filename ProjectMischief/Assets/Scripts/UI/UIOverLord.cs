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
    string nextLevelToLoad;

    void Awake()
    {
        //Time.timeScale = 1.0f;
    }

    void OnEnable()
    {
        if (instance == null)
        {
            gameIsPaused = false;
            Application.LoadLevelAdditive("UILevel");
            instance = this;
        }
    }

    public void CloseAllUI()
    {
        for(uint i =0; i < uiInstances.Length; ++i)
        {
            if(uiInstances[i] != null)
            {
                uiInstances[i].GetComponent<UIManger>().CloseUI();
            }
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

    public void EndLevel(string nextLevel)
    {
        nextLevelToLoad = nextLevel;
        LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
        levelUI.TurnTimerOff();

        Application.LoadLevelAdditive("UIGrading");
    }

    public string GetNextLevelToLoad()
    {
        return nextLevelToLoad;
    }

    public void Spawn2DReticle(Camera cam, Vector3 pos)
    {
        if (uiInstances[(int)UITypes.level] != null)
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.Spawn2DReticle(cam, pos);
            levelUI = null;
        }
    }

    public void SetPaintingPos( uint index, Vector3 pos )
    {
        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetPaintingPos(index, pos );
            levelUI = null;
        }
    }

    public void SetPaintingIteractedWith( bool interactivedWith, uint index )
    {
        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetPaintingIteractedWith( interactivedWith , index);
            levelUI = null;
        }
    }

    public void SetVisualCueActive(bool active)
    {
        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetVisualCueActive(active);
            levelUI = null;
        }
    }
}
