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

    //private
    GameObject[] paintingVisualCues;
    GameObject spawned2DRecticle;
    Vector3[] paintingWorldPos;
    Vector3 recticle3DPos = new Vector3();

    float timeElapsed;

    float deltaTime = 0;
    float lastFramesTime;

    GameObject timer;
    GameObject menu;
    GameObject visualCuesParent;

    Text timerText;

	PersistentSceneData data;
	Text[] toolCount = new Text[(int)ToolTypes.eToolMAX];

<<<<<<< HEAD
=======
    Canvas canvas;

>>>>>>> parent of c4d2123... [UI] -Adde Animated loadingScreen [Optimization] - optimized levelUIControl, fogOfWar and VisionCone
    // public
    public LevelUIControl()
        : base(UITypes.level)
    { }

    public float GetTimeElapsed()
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
<<<<<<< HEAD
            spawned2DRecticle = Instantiate(recticle2D);
            spawned2DRecticle.transform.SetParent( menu.transform );
=======
            RenderMode prevMode = canvas.renderMode;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; // HACK (COLE)

            spawned2DRecticle = Instantiate(recticle2D);
            spawned2DRecticle.transform.SetParent(visualCuesParent.transform);
>>>>>>> parent of c4d2123... [UI] -Adde Animated loadingScreen [Optimization] - optimized levelUIControl, fogOfWar and VisionCone
            RectTransform tempTransform = spawned2DRecticle.GetComponent<RectTransform>();
            tempTransform.transform.position = RectTransformUtility.WorldToScreenPoint(cam, pos);
            recticle3DPos = pos;
        }
    }

    public void SetPaintingPos(uint index, Vector3 pos )
    {
        if(index < paintingWorldPos.Length)
        {
            paintingWorldPos[index] = pos;
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

<<<<<<< HEAD
=======
    public void TogglePauseButtonActive()
    {
        pauseButton.SetActive( !pauseButton.activeSelf );
    }

>>>>>>> parent of c4d2123... [UI] -Adde Animated loadingScreen [Optimization] - optimized levelUIControl, fogOfWar and VisionCone
        // Functions for buttons
    public void ToMenu()
    {
        UIManager.instance.CloseAllUI();
        Application.LoadLevel( "FrontEnd" );
    }

    //Prottected
    protected override void DurringOnEnable()
    {
        // Grab relvent objects
		menu = transform.FindDeepChild( "MenuLevel" ).gameObject;
		visualCuesParent = transform.FindDeepChild( "VisualCues" ).gameObject;
		timer = transform.FindDeepChild( "Timer" ).gameObject;
		GameObject temp = transform.FindDeepChild( "TimerText" ).gameObject;		
        // TODO: add asserts
        timerText = temp.GetComponent<Text>();

		temp = transform.FindDeepChild ("JammerCountText").gameObject;
		toolCount[(int)ToolTypes.eJammer] = temp.GetComponent<Text>();

		temp = transform.FindDeepChild ("SmokeBombCountText").gameObject;
		toolCount[(int)ToolTypes.eSmokeBomb] = temp.GetComponent<Text>();

		temp = transform.FindDeepChild ("MirrorCountText").gameObject;
		toolCount[(int)ToolTypes.eMirror] = temp.GetComponent<Text>();

		data = PersistentSceneData.GetPersistentData ();

        // itailize varibles
        timeElapsed = 0.0f;

        lastFramesTime = Time.realtimeSinceStartup;

        // set up the visual cues
        uint numPaintings = ArtManager.instance.GetNumPaintings();
        paintingVisualCues = new GameObject[numPaintings];

        for( uint i = 0; i < paintingVisualCues.Length; ++i )
        {
            GameObject visualCue = Instantiate( paintingVisualCuePrefab );
            paintingVisualCues[i] = visualCue;
            visualCue.transform.SetParent( visualCuesParent.transform );
            Image visualCueImage = visualCue.GetComponent<Image>();
            visualCueImage.sprite = paintingVisualCueNotIntracted;
        }

        paintingWorldPos = new Vector3[numPaintings];
    }

    // Private
    void Update()
    {
        CalculateDeltaTime();

        if( !UIManager.gameIsPaused )
        {
            timeElapsed += deltaTime;
        }

        const int kSec = 60; // num of seconds per minute;
        string minSec = string.Format( "{0}:{1:00}", (int)(timeElapsed / kSec), (int)(timeElapsed % kSec) );
        timerText.text = "Time " + minSec;
<<<<<<< HEAD
=======

		toolCount [(int)ToolTypes.eJammer].text = "Jammers\n" + data.GetNumTools(ToolTypes.eJammer).ToString();
		toolCount [(int)ToolTypes.eMirror].text = "Mirrors\n"+ data.GetNumTools(ToolTypes.eMirror).ToString();
		toolCount [(int)ToolTypes.eSmokeBomb].text = "Smoke Bombs\n"+ data.GetNumTools(ToolTypes.eSmokeBomb).ToString();
>>>>>>> parent of c4d2123... [UI] -Adde Animated loadingScreen [Optimization] - optimized levelUIControl, fogOfWar and VisionCone

		toolCount [(int)ToolTypes.eJammer].text = "Jammers\n" + data.GetNumTools(ToolTypes.eJammer).ToString();
		toolCount [(int)ToolTypes.eMirror].text = "Mirrors\n"+ data.GetNumTools(ToolTypes.eMirror).ToString();
		toolCount [(int)ToolTypes.eSmokeBomb].text = "Smoke Bombs\n"+ data.GetNumTools(ToolTypes.eSmokeBomb).ToString();

        UpdateRecticle();
        UpdateVisualCue();
    }

    void UpdateVisualCue()
    {
        Camera cam = Camera.main;

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
            Camera cam = Camera.main;
            RectTransform tempTransform = spawned2DRecticle.GetComponent<RectTransform>();
            tempTransform.transform.position = RectTransformUtility.WorldToScreenPoint( cam, recticle3DPos );
        }
    }

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }

}