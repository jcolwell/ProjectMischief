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

    public Text numPaintingsLeftText;
    public Text timerText;
    public Text[] toolCount = new Text[(int)ToolTypes.eToolMAX];
    //private
        // reticles and visual cues
    GameObject[] paintingVisualCues;
    GameObject spawned2DRecticle;
    Vector3[] paintingWorldPos;
    Vector3 recticle3DPos = new Vector3();

    int numPaintingsLeft = 0;

        // time related varibles
    double timeElapsed;
    double deltaTime = 0;
    double lastFramesTime;

	PersistentSceneData data;
    Canvas canvas;
    Camera cam;

    // public
    public LevelUIControl()
        : base(UITypes.level, 0)
    { }

    public void BringUpMap()
    {

    }

    public void CloseMap()
    {
        //UIManager.instance.pause
    }

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
                if(numPaintingsLeftText != null)
                {
                    numPaintingsLeftText.text = numPaintingsLeft.ToString() + "/" + ArtManager.instance.GetNumPaintings();
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

    public void TogglePauseButtonActive()
    {
        pauseButton.SetActive( !pauseButton.activeSelf );
    }

    public void UpdateToolCount()
    {
        toolCount[(int)ToolTypes.eJammer].text = "Jammers\n" + data.GetNumTools(ToolTypes.eJammer).ToString();
        toolCount[(int)ToolTypes.eMirror].text = "Mirrors\n" + data.GetNumTools(ToolTypes.eMirror).ToString();
        toolCount[(int)ToolTypes.eSmokeBomb].text = "Smoke Bombs\n" + data.GetNumTools(ToolTypes.eSmokeBomb).ToString();
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

		data = PersistentSceneData.GetPersistentData ();

        // itailize varibles
        timeElapsed = 0.0f;

        lastFramesTime = Time.realtimeSinceStartup;

        // set up the visual cues and recticle
        uint numPaintings = ArtManager.instance.GetNumPaintings();
        paintingVisualCues = new GameObject[numPaintings];
        numPaintingsLeft = (int)numPaintings;

        if( numPaintingsLeftText != null )
        {
            numPaintingsLeftText.text = numPaintingsLeft.ToString() + "/" + ArtManager.instance.GetNumPaintings();
        }

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

        if (recticle2D != null)
        {
            spawned2DRecticle = Instantiate(recticle2D);
            spawned2DRecticle.transform.SetParent(visualCuesParent.transform);
            spawned2DRecticle.SetActive(false);
        }

        if(UIManager.instance.loadTutorialMsg)
        {
            tutorialMsg.SetActive( true );
            Text temp = tutorialMsg.GetComponentInChildren<Text>();
            temp.text = UIManager.instance.tutorialMsg;
        }

        cam = Camera.main;

        UpdateToolCount();
    }

    // Private
    void Update()
    {
        CalculateDeltaTime();

        if(tutorialMsg.activeSelf && UIManager.instance.loadTutorialMsg)
        {
            Time.timeScale = 0.0f;
            UIManager.gameIsPaused = true;
        }

        if( !UIManager.gameIsPaused )
        {
            timeElapsed += deltaTime;
        }

        const int kSec = 60; // num of seconds per minute;
        timerText.text = "Time " + string.Format( "{0}:{1:00}", (int)(timeElapsed / kSec), (int)(timeElapsed % kSec) );

        RenderMode prevMode = canvas.renderMode;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; // HACK (COLE)

        UpdateRecticle();
        UpdateVisualCue();

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

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }
}