using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    public GameObject exitNotificationPopUp;
    public float offsetDiv = 0.3f;
    public float offsetHeightDiff = 0.05f;
    public bool isArtGallery = false;

    // tool used popUp image stuff
    public Text coinsNotificationText;
    public GameObject smokeBombUsed;
    public GameObject mirrorUsed;
    public GameObject zapperUsed;
    public GameObject inputBlockerAndFilter;
    public GameObject exitArrow;
    public GameObject MoveButton;
    public RectTransform exitArrowRotator;
    public double toolUsedPopUpDuration = 1.0f;
    public double coinNotifcationDuration = 1.0f;
    public double playerCaughtPopUpDuration = 5.0f;
    public double exitNotificationPopUpDuration = 5.0f;
    double toolUsedPopUpTimePassed = 0.0f;
    double coinNotifactionTimePassed = 0.0f;
    double playerCaughtPopUpTimePassed = 0.0f;
    double exitNotificationPopUpTimePassed = 0.0f;
    bool toolPopUpActive = false;
    bool playerPopUpActive = false;
    bool exitPopUpActive = false;
    bool levelCompleted = false;
    bool isMoveAlreadyPlayed = false;

    //public Text numPaintingsLeftText;
    public Text gradeCounter;
    public string gradeCounterFlavorText = "Grade ";
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

    Vector3 exitWorldPos = new Vector3();

    int numPaintingsLeft = 0;

    PersistentSceneData data;
        // time related varibles
    float timeElapsed;
    float deltaTime = 0;
    float lastFramesTime;

    Canvas canvas;
    Camera cam;

    Vector2 playerPos;
    GameObject player;

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

        if(interactivedWith && !isArtGallery)
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

                    ActivateExitNotification();
                    
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
        playerPos = RectTransformUtility.WorldToScreenPoint(cam, player.transform.position);
        RectTransform rect = tutorialMsg.GetComponent<RectTransform>();
        Vector3 offset = new Vector3(rect.rect.width * offsetDiv, rect.rect.height * - (offsetDiv + offsetHeightDiff), 0);
        tutorialMsg.transform.position = (Vector3)playerPos - offset;
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

    public void SetExitPos(Vector3 pos)
    {
        exitWorldPos = pos;
    }

    //Prottected
    protected override void DurringOnEnable()
    {

        // Grab relvent objects
        GameObject canvasObject = transform.FindDeepChild("Canvas").gameObject;
        canvas = canvasObject.GetComponent<Canvas>();
        data = PersistentSceneData.GetPersistentData();

        if (!data.CheckIfPlayerhasPlayerMoved())
        {
            MoveButton.SetActive(true);
        }

        player = GameObject.Find("Actor");

        if (player == null)
        {
            player = GameObject.Find("Actor(Clone)");
        }


        if(SceneManager.GetActiveScene().name == "ArtGallery")
        {
            isArtGallery = true;
        }

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
        
        float width = paintingCounterToken.GetComponent<RectTransform>().rect.width;
        float offSet = (numPaintingsLeft % 2 == 0) ? width * 0.5f : width; 
        float startingXPos = paintingCounter.transform.position.x - ( Mathf.Floor(numPaintingsLeft * 0.5f) * offSet);

        if (!isArtGallery)
        {
            for (uint i = 0; i < paintingTokens.Length; ++i)
            {
                GameObject curToken = Instantiate(paintingCounterToken);
                paintingTokens[i] = curToken;
                curToken.transform.SetParent(paintingCounter.transform, false);
                curToken.transform.position = new Vector3(startingXPos + ((width * curToken.transform.lossyScale.x) * i), paintingCounter.transform.position.y);

                ImageToken curPainting = curToken.GetComponent<ImageToken>();
                curPainting.painting.sprite = ArtManager.instance.GetPainting(i).art;
            }
        }

        // set visual cues up
        for( uint i = 0; i < paintingVisualCues.Length; ++i )
        {
            GameObject visualCue = Instantiate( paintingVisualCuePrefab );
            paintingVisualCues[i] = visualCue;
            visualCue.transform.SetParent( visualCuesParent.transform, false );
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
            spawned2DRecticle.transform.SetParent(visualCuesParent.transform, false);
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
        UpdateGrade();
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
        exitNotificationPopUpTimePassed -= deltaTime;

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
        UpdateExitNotification();
        UpdateExitArrow();

        canvas.renderMode = prevMode; // HACK (COLE)
    }

    void ActivateExitNotification()
    {
        exitPopUpActive = true;
        exitNotificationPopUpTimePassed = exitNotificationPopUpDuration;
        exitNotificationPopUp.SetActive(true);
        UIManager.instance.PauseGameTime();
        UIManager.instance.PauseTimeScale();

        levelCompleted = true;
    }

    void UpdateExitArrow()
    {
        if (levelCompleted)
        {
            exitArrow.SetActive(!EndOfLevel.isEndOfLevelVisible || exitPopUpActive);
            exitArrowRotator.rotation = new Quaternion();

            Vector2 exitPos = RectTransformUtility.WorldToScreenPoint(cam, exitWorldPos);

            exitPos.x -= exitArrowRotator.position.x;
            exitPos.y -= exitArrowRotator.position.y;

            float angle = Vector2.Angle(Vector2.right, exitPos);
            Vector3 cross = Vector3.Cross(Vector2.right, exitPos);

            if (cross.z < 0)
            {
                angle = 360 - angle;
            }

            exitArrowRotator.Rotate(new Vector3(0.0f, 0.0f, angle));
        }
    }

    void UpdateExitNotification()
    {
        if(exitPopUpActive && exitNotificationPopUpTimePassed <= 0.0f)
        {
            exitPopUpActive = false;
            exitNotificationPopUp.SetActive(false);
            UIManager.instance.UnPauseGameTime();
            UIManager.instance.UnPauseTimeScale();
        }
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

    public void UpdateGrade()
    {
        gradeCounter.text = gradeCounterFlavorText + ArtManager.instance.GetFinalGrade().ToString() 
            + "/" + ArtManager.instance.GetGradeMax().ToString();
    }
    public void UpdateTapToMoveButton()
    {
        data.SethasPlayerMoved(true);
    }

}