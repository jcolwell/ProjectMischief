using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UITypes
{
    Correction,
    level,
    study,
    grading,
    store,
    UIMAX
}

public class UIManager : MonoBehaviour 
{
    static public bool gameIsPaused = false;
    static public UIManager instance = null;

    public bool isNotInALevel = false;

    UIControl[] uiInstances = new UIControl[(int)UITypes.UIMAX];
    uint activeUI = 0;
    string nextLevelToLoad;

    //Intialization stuff
    void OnEnable()
    {
        if (instance == null)
        {
            gameIsPaused = false;
            if( !isNotInALevel )
            {
                Application.LoadLevelAdditive( "UILevel" );
            }
            instance = this;
        }
    }

    // Genral UI stuff
    public void CloseAllUI()
    {
        for(uint i =0; i < uiInstances.Length; ++i)
        {
            if(uiInstances[i] != null)
            {
                uiInstances[i].GetComponent<UIControl>().CloseUI();
            }
        }
    }

    public void RegisterUI(GameObject ui, UITypes type)
    {
        ++activeUI;
        UIControl temp = ui.GetComponent<UIControl>();
        uiInstances[(int)type] = temp;
        SetLevelMenuActive();
    }

    public void UnRegisterUI(UITypes type)
    {
        --activeUI;
        uiInstances[(int)type] = null;
        SetLevelMenuActive();
    }

    // Level UI related tasks
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

    public void Spawn2DReticle(Camera cam, Vector3 pos)
    {
        if (uiInstances[(int)UITypes.level] != null)
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.Spawn2DReticle(cam, pos);
            levelUI = null;
        }
    }

    public void SetPaintingPos(uint index, Vector3 pos)
    {
        if (uiInstances[(int)UITypes.level] != null)
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetPaintingPos(index, pos);
            levelUI = null;
        }
    }

    public void SetPaintingIteractedWith(bool interactivedWith, uint index)
    {
        if (uiInstances[(int)UITypes.level] != null)
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetPaintingIteractedWith(interactivedWith, index);
            levelUI = null;
        }
    }

    void SetLevelMenuActive()
    {
        bool active = false;

        if(activeUI == 1) 
        {
            active = true;
        }
        else if(activeUI == 0)
        {
            return;
        }

        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetMenuActive(active);
            levelUI = null;
        }

    }

    public void SetVisualCueActive(bool active)
    {
        if (uiInstances[(int)UITypes.level] != null)
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetVisualCueActive(active);
            levelUI = null;
        }
    }

    // Grading UI related tasks
    public string GetNextLevelToLoad()
    {
        return nextLevelToLoad;
    }

    // Correction UI related tasks
    public void LoadCorrectionUI()
    {
        Application.LoadLevelAdditive( "UITest" );
    }
    
    public void InitializeArtCorrectionUI(uint artContextID)
    {
        if (uiInstances[(int)UITypes.Correction] != null)
        {
            CorrectionUIControl correctionlUI = uiInstances[(int)UITypes.Correction].GetComponent<CorrectionUIControl>();
            correctionlUI.artContextID = artContextID;
            correctionlUI.SetCurrentFields();
        }
    }

    // StudyUI related tasks
    public void LoadStudyUI()
    {
        Application.LoadLevelAdditive( "UIStudy" );
    }

    // Store related tasks
    public void LoadStoreUI()
    {
        Application.LoadLevelAdditive( "UIStore" );
    }

    private UIManager()
    {

    }
}
