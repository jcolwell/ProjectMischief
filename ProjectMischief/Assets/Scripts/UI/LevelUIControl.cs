using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUIControl : UIControl 
{
    //public
    public GameObject recticle2D;
    public Sprite paintingVisualCueIntracted;
    public Sprite paintingVisualCueNotIntracted;
    public GameObject paintingVisualCuePrefab;

        // misc
    public GameObject timer;
    public GameObject menu;
    public GameObject pauseButton;
    public GameObject visualCuesParent;
    public GameObject tutorialMsg;
    
    public GameObject paintingCounter;
    public GameObject paintingCounterToken;
    public Color paintingColorDimColor = new Color( 1.0f, 1.0f, 1.0f, 0.5f );

    public RenderTexture rendTexture;

    public GameObject playerCaughtPopUp;

        // tool used popUp image stuff
    public Text coinsNotificationText;
    public GameObject smokeBombUsed;
    public GameObject mirrorUsed;
    public GameObject zapperUsed;
    public GameObject inputBlockerAndFilter;
    public double toolUsedPopUpDuration = 1.0f;
    public double coinNotifcationDuration = 1.0f;
    public double playerCaughtPopUpDuration = 5.0f;
    double toolUsedPopUpTimePassed = 0.0f;
    double coinNotifactionTimePassed = 0.0f;
    double playerCaughtPopUpTimePassed = 0.0f;
    bool toolPopUpActive = false;
    bool playerPopUpActive = false;

    //public Text numPaintingsLeftText;
    public Text timerText;
    public Text[] toolCount = new Text[(int)ToolTypes.eToolMAX];
    public GameObject[] toolCounterImages = new GameObject[(int)ToolTypes.eToolMAX];
    //private
    ImageToken curPainting3D = null;

        // reticles and visual cues
    GameObject[] paintingVisualCues;
    GameObject[] paintingTokens;
    GameObject spawned2DRecticle;
    Vector3[] paintingWorldPos;
    Vector3 recticle3DPos = new Vector3();

    Vector3 coinWorldPos = new Vector3();

    int numPaintingsLeft = 0;

    PersistentSceneData data;
        // time related varibles
    double timeElapsed;
    double deltaTime = 0;
    double lastFramesTime;

    Canvas canvas;
    Camera cam;

    // public
    public LevelUIControl()
        : base(UITypes.level, 0)
    { }

    public double GetTimeElapsed()
    {
        return timeElapsed;
    }

    public void TurnTimerOff()
    {
        UIManager.gameIsPaused = true;
        timer.SetActive(false);
    }

    public void TurnTimerOn()
    {
        UIManager.gameIsPaused = false;
        timer.SetActive(true);
    }

    public void Spawn2DReticle(Camera cam, Vector3 pos)
    {
        if (recticle2D != null)
        {
            spawned2DRecticle.SetActive(true);
            spawned2DRecticle.GetComponent<MovementReticle>().Reset();

            RenderMode prevMode = canvas.renderMode;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // HACK (COLE)

            RectTransform tempTransform = spawned2DRecticle.GetComponent<RectTransform>();
            tempTransform.transform.position = RectTransformUtility.WorldToScreenPoint(cam, pos);
            recticle3DPos = pos;

            canvas.renderMode = prevMode; // HACK (COLE)
        }
    }

    public void SetPaintingIteractedWith( bool interactivedWith, uint index )
    {
        if(index >= paintingVisualCues.Length)
        {
            return;
        }

        Image visualCueImage = paintingVisualCues[index].GetComponent<Image>();

        if(interactivedWith)
        {
            if(visualCueImage.sprite != paintingVisualCueIntracted)
            {
                --numPaintingsLeft;
                ImageToken curPainting = paintingTokens[index].GetComponent<ImageToken>();
                curPainting.painting.color = paintingColorDimColor;
                curPainting.tokenImage.color = paintingColorDimColor;
                curPainting.tokenImage.sprite = paintingVisualCueIntracted;

                curPainting3D.imageToken3D.sprite = paintingVisualCueIntracted;
                if(numPaintingsLeft == 0)
                { 
                    EndOfLevel.allPaintingsComplete = true;
                }
            }
            visualCueImage.sprite = paintingVisualCueIntracted;
        }
        else 
        {
            visualCueImage.sprite = paintingVisualCueNotIntracted;
        }
    }

    public void SetVisualCueActive(bool active)
    {
        visualCuesParent.SetActive( active );
    }

    public void SetMenuActive( bool active )
    {
        if( menu != null )
        {
            menu.SetActive( active );
        }
    }

    public void SetCurPainting( ref ImageToken token )
    {
        curPainting3D = token;
    }

    public void TogglePauseButtonActive()
    {
        pauseButton.SetActive( !pauseButton.activeSelf );
    }

    public void UpdateToolCount()
    {
        int numTools = 0;
        for(int i = 0; i < (int)ToolTypes.eToolMAX; ++i)
        {
            numTools = data.GetNumTools((ToolTypes)i);
            toolCount[i].text = numTools.ToString();
            toolCounterImages[i].SetActive(numTools > 0);
        }
    }

    public void UsedTool(ToolTypes toolUsed)
    {
        UIManager.instance.PauseGameTime();
        UIManager.instance.PauseTimeScale();
        switch (toolUsed)
        {
            case ToolTypes.eJammer:
                zapperUsed.SetActive(true);
                break;
            case ToolTypes.eMirror:
                mirrorUsed.SetActive(true);
                break;
            case ToolTypes.eSmokeBomb:
                smokeBombUsed.SetActive(true);
                break;
        }
        inputBlockerAndFilter.SetActive(true);
        toolUsedPopUpTimePassed = toolUsedPopUpDuration;
        toolPopUpActive = true;
    }

    public void EarnedCoin(Vector3 coinPos, int coinValue)
    {
        coinsNotificationText.gameObject.SetActive(true);
        coinsNotificationText.text = "+" + coinValue;
        coinsNotificationText.rectTransform.position = RectTransformUtility.WorldToScreenPoint(cam, coinPos);
        coinWorldPos = coinPos;
        coinNotifactionTimePassed = coinNotifcationDuration;
    }

    public void PopUpTutorialMSG(string msg)
    {
        tutorialMsg.SetActive(true);
        Text temp = tutorialMsg.GetComponentInChildren<Text>();
        temp.text = msg;
    }

    public void ActivatePlayerCaughtPopUp()
    {
        UIManager.instance.PauseGameTime();
        UIManager.instance.PauseTimeScale();
        playerCaughtPopUpTimePassed = playerCaughtPopUpDuration;
        playerPopUpActive = true;
        playerCaughtPopUp.SetActive(true);
    }

        // Functions for buttons
    public void LoadPauseMenu()
    {
        UIManager.instance.LoadPauseMenu();
        TogglePauseButtonActive();
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1.0f;
        UIManager.gameIsPaused = false;
    }

    //Prottected
    protected override void DurringOnEnable()
    {
        // Grab relvent objects
        GameObject canvasObject = transform.FindDeepChild("Canvas").gameObject;
        canvas = canvasObject.GetComponent<Canvas>();
        data = PersistentSceneData.GetPersistentData();

        // itailize varibles
        timeElapsed = 0.0f;

        lastFramesTime = Time.realtimeSinceStartup;

        zapperUsed.SetActive( false );
        mirrorUsed.SetActive( false );
        smokeBombUsed.SetActive( false );
        inputBlockerAndFilter.SetActive( false );

        // set up the visual cues and recticle
        uint numPaintings = ArtManager.instance.GetNumPaintings();
        paintingVisualCues = new GameObject[numPaintings];
        paintingTokens = new GameObject[numPaintings];
        numPaintingsLeft = (int)numPaintings;

        // setting up Painting visual
        
        //if( numPaintingsLeftText != null )
        //{
        //    numPaintingsLeftText.text = numPaintingsLeft.ToString() + "/" + ArtManager.instance.GetNumPaintings();
        //}
        float width = paintingCounterToken.GetComponent<RectTransform>().rect.width;
        float offSet = (numPaintingsLeft % 2 == 0) ? width * 0.5f : width; 
        float startingXPos = paintingCounter.transform.position.x - ( Mathf.Floor(numPaintingsLeft * 0.5f) * offSet);

        for( uint i = 0; i < paintingTokens.Length; ++i )
        {
            GameObject curToken = Instantiate( paintingCounterToken );
            paintingTokens[i] = curToken;
            curToken.transform.SetParent( paintingCounter.transform );
            curToken.transform.position = new Vector3(  startingXPos + (width * i), paintingCounter.transform.position.y );

            ImageToken curPainting = curToken.GetComponent<ImageToken>();
            curPainting.painting.sprite = ArtManager.instance.GetPainting(i).art;
        }

        // set visual cues up
        for( uint i = 0; i < paintingVisualCues.Length; ++i )
        {
            GameObject visualCue = Instantiate( paintingVisualCuePrefab );
            paintingVisualCues[i] = visualCue;
            visualCue.transform.SetParent( visualCuesParent.transform );
            Image visualCueImage = visualCue.GetComponent<Image>();
            visualCueImage.sprite = paintingVisualCueNotIntracted;
        }

        paintingWorldPos = new Vector3[numPaintings];

        for (int i = 0; i < numPaintings; ++i )
        {
            paintingWorldPos[i] = ArtManager.instance.GetPaintingPos((uint)i);
        }

        // set recticle up
        if (recticle2D != null)
        {
            spawned2DRecticle = Instantiate(recticle2D);
            spawned2DRecticle.transform.SetParent(visualCuesParent.transform);
            spawned2DRecticle.SetActive(false);
        }

        // set up tutorila message
        if(UIManager.instance.loadTutorialMsg)
        {
            PopUpTutorialMSG(UIManager.instance.tutorialMsg);
        }

        // misc
        cam = Camera.main;
        coinsNotificationText.gameObject.SetActive( false );
        playerCaughtPopUp.SetActive(false);

        UpdateToolCount();
    }

    // Private
    void Update()
    {
        CalculateDeltaTime();

        if(tutorialMsg.activeSelf)
        {
            Time.timeScale = 0.0f;
            UIManager.gameIsPaused = true;
        }

        if( !UIManager.gameIsPaused )
        {
            timeElapsed += deltaTime;
        }

        toolUsedPopUpTimePassed -= deltaTime;
        coinNotifactionTimePassed -= deltaTime;
        playerCaughtPopUpTimePassed -= deltaTime;

        if( toolPopUpActive && toolUsedPopUpTimePassed <= 0.0f )
        {
            zapperUsed.SetActive( false );
            mirrorUsed.SetActive( false );
            smokeBombUsed.SetActive( false );
            toolPopUpActive = false;
            inputBlockerAndFilter.SetActive( false );

            UIManager.instance.UnPauseGameTime();
            UIManager.instance.UnPauseTimeScale();
        } 

        const int kSec = 60; // num of seconds per minute;
        timerText.text = "Time " + string.Format( "{0}:{1:00}", (int)(timeElapsed / kSec), (int)(timeElapsed % kSec) );

        RenderMode prevMode = canvas.renderMode;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; // HACK (COLE)

        UpdateRecticle();
        UpdateVisualCue();
        UpdateCoinNotifiaction();
        UpdatePlayerCaughtPopUp();

        canvas.renderMode = prevMode; // HACK (COLE)
    }

    void UpdateVisualCue()
    {
        for( uint i = 0; i < paintingVisualCues.Length; ++i )
        {
            RectTransform tempTransform = paintingVisualCues[i].GetComponent<RectTransform>();
            tempTransform.transform.position = RectTransformUtility.WorldToScreenPoint( cam, paintingWorldPos[i] );
        }
    }

    void UpdateRecticle()
    {
        if(spawned2DRecticle != null)
        {
            RectTransform tempTransform = spawned2DRecticle.GetComponent<RectTransform>();
            tempTransform.transform.position = RectTransformUtility.WorldToScreenPoint(cam, recticle3DPos); 
        }
    }

    void UpdateCoinNotifiaction()
    {
        if( coinNotifactionTimePassed <= 0.0f )
        {
            coinsNotificationText.gameObject.SetActive( false );
        }
        else
        {
            coinsNotificationText.transform.position = RectTransformUtility.WorldToScreenPoint( cam, coinWorldPos );
        }
    }

    void UpdatePlayerCaughtPopUp()
    {
        if(playerPopUpActive && playerCaughtPopUpTimePassed <= 0.0)
        {
            playerPopUpActive = false;
            playerCaughtPopUp.SetActive(false);
            UIManager.instance.UnPauseGameTime();
            UIManager.instance.UnPauseTimeScale();
        }
    }

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }
}