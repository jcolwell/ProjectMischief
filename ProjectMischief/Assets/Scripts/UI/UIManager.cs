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
    levelSelect,
    settings,
    pauseMenu,
    UIMAX
}

public class UIManager : MonoBehaviour
{
    static public bool gameIsPaused = false;
    static public UIManager instance = null;

    public bool isNotInALevel = false;

    // HACK (Cole)
    public GameObject fogOfWar = null;

    UIControl[] uiInstances = new UIControl[(int)UITypes.UIMAX];
    uint activeUI = 0;
    string nextLevelToLoad;

    // for when fixed aspect ratio is enabled aspect ratio
    public Vector2 aspectRatio = new Vector2( 16.0f, 10.0f );

    //Intialization stuff
    void Awake()
    {
        if( instance == null )
        {
            PersistentSceneData.GetPersistentData();
            gameIsPaused = false;
            if( !isNotInALevel )
            {
                Application.LoadLevelAdditive( "UILevel" );
                SetFogOfWar();
            }
            instance = this;
        }
    }

    void Start()
    {
        SettingsData settingData = PersistentSceneData.GetPersistentData().GetSettingsData();
        if( settingData.fixedAspectRatio ) // place for a setting check
        {
            AddLetterBox();
        }
    }

    void AddLetterBox()
    {
        // set the desired aspect ratio 
        float targetaspect = aspectRatio.x / aspectRatio.y;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = Camera.main;

        // if scaled height is less than current height, add letterbox
        if( scaleheight < 1.0f )
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) * 0.5f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) * 0.5f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    void RemoveLetterBox()
    {
        Camera camera = Camera.main;
        Rect rect = camera.rect;

        rect.x = 0.0f;
        rect.y = 0.0f;
        rect.width = 1.0f;
        rect.height = 1.0f;
        camera.rect = rect;
    }

    // Genral UI stuff
    public void CloseAllUI()
    {
        for( uint i = 0; i < uiInstances.Length; ++i )
        {
            if( uiInstances[i] != null )
            {
                uiInstances[i].GetComponent<UIControl>().CloseUI();
            }
        }
    }

    public void RegisterUI( GameObject ui, UITypes type )
    {
        UIControl temp = ui.GetComponent<UIControl>();
        if( uiInstances[(int)type] == null )
        {
            ++activeUI;
            uiInstances[(int)type] = temp;
            SetLevelMenuActive();
        }
        else
        {
            temp.CloseUI();
        }
    }

    public void UnRegisterUI( UITypes type )
    {
        --activeUI;
        uiInstances[(int)type] = null;
        SetLevelMenuActive();
    }

    public void ResetAllUICanvas()
    {
        SettingsData settingData = PersistentSceneData.GetPersistentData().GetSettingsData();

        if( settingData.fixedAspectRatio )// place for settings check
        {
            AddLetterBox();
        }
        else
        {
            RemoveLetterBox();
        }

        for( int i = 0; i < uiInstances.Length; ++i )
        {
            if( uiInstances[i] != null )
            {
                uiInstances[i].SetCanvas();
            }
        }
    }

    // TODO: (Cole) Should this be here or moved somewhere else??
    public void SetFogOfWar()
    {
        if( fogOfWar == null )
        {
            fogOfWar = GameObject.Find( "Fow" );
            if( fogOfWar == null )
            {
                fogOfWar = GameObject.FindGameObjectWithTag( "Fow50" );
            }
        }

        if( fogOfWar != null )
        {
            SettingsData settingData = PersistentSceneData.GetPersistentData().GetSettingsData();
            fogOfWar.SetActive( settingData.fogOfWarOn );
        }
    }

    // Level UI related tasks
    public float GetTimeElapsed()
    {
        LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
        return levelUI.GetTimeElapsed();
    }

    public void EndLevel( string nextLevel )
    {
        nextLevelToLoad = nextLevel;
        LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
        levelUI.TurnTimerOff();

        Application.LoadLevelAdditive( "UIGrading" );
    }

    public void Spawn2DReticle( Camera cam, Vector3 pos )
    {
        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.Spawn2DReticle( cam, pos );
            levelUI = null;
        }
    }

    public void SetPaintingPos( uint index, Vector3 pos )
    {
        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetPaintingPos( index, pos );
            levelUI = null;
        }
    }

    public void SetPaintingIteractedWith( bool interactivedWith, uint index )
    {
        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetPaintingIteractedWith( interactivedWith, index );
            levelUI = null;
        }
    }

    void SetLevelMenuActive()
    {
        bool active = false;

        if( activeUI == 1 )
        {
            active = true;
        }
        else if( activeUI == 0 )
        {
            return;
        }

        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetMenuActive( active );
            levelUI = null;
        }

    }

    public void SetVisualCueActive( bool active )
    {
        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.SetVisualCueActive( active );
            levelUI = null;
        }
    }

    public void UpdateToolCount()
    {
        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.UpdateToolCount();
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

    public void InitializeArtCorrectionUI( uint artContextID )
    {
        if( uiInstances[(int)UITypes.Correction] != null )
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

    // Level Select Related tasks

    public void LoadLevelSelect()
    {
        Application.LoadLevelAdditive( "UILevelSelect" );
    }

    public void TogglePauseButtonActive()
    {
        if( uiInstances[(int)UITypes.level] != null )
        {
            LevelUIControl levelUI = uiInstances[(int)UITypes.level].GetComponent<LevelUIControl>();
            levelUI.TogglePauseButtonActive();
            levelUI = null;
        }
    }

    // PauseMenu related Tasks

    public void LoadPauseMenu()
    {
        Application.LoadLevelAdditive( "UIPauseMenu" );
    }

    // Settings related Tasks

    public void LoadSettings()
    {
        Application.LoadLevelAdditive( "UISettings" );
    }

    // privates
    private UIManager()
    {

    }
}
