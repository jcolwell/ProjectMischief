using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUIControl : MonoBehaviour 
{
    public GameObject recticle2D;
    public Sprite paintingVisualCueIntracted;
    public Sprite paintingVisualCueNotIntracted;
    public GameObject paintingVisualCuePrefab;

    GameObject[] paintingVisualCues;
    Vector3[] paintingWorldPos;

    float timeElapsed;

    float deltaTime = 0;
    float lastFramesTime;

    GameObject timer;
    GameObject menu;
    GameObject visualCuesParent;

    Text timerText;

    void OnEnable() 
    {
        UIOverLord.instance.RegisterUI(gameObject, UITypes.level);

        // Grab relvent objects
        menu = GameObject.Find( "MenuLevel" );
        visualCuesParent = GameObject.Find( "VisualCues" );
        timer = GameObject.Find("Timer");
        GameObject temp = GameObject.Find( "TimerText" );
        // TODO: add asserts
        timerText = temp.GetComponent<Text>();

        // itailize varibles
        timeElapsed = 0.0f;

        lastFramesTime = Time.realtimeSinceStartup;

        int numPaintings = ArtManager.instance.GetNumPaintings();
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
	
	void Update () 
    {
        CalculateDeltaTime();

        if( !UIOverLord.gameIsPaused)
        {
            timeElapsed += deltaTime;
        }

        const int kSec = 60; // num of seconds per minute;
        string minSec = string.Format( "{0}:{1:00}", (int)(timeElapsed / kSec), (int)(timeElapsed % kSec) );
        timerText.text = "Time " + minSec;

        // if any other ui is open make this ui invisble
        int uiOpen = UIManger.GetNumOfUIOpen();
        menu.SetActive( uiOpen == 1 );


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

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }

    void OnDestroy()
    {
        UIOverLord.instance.UnRegisterUI(UITypes.level);
    }

    public float GetTimeElapsed()
    {
        return timeElapsed;
    }

    public void TurnTimerOff()
    {
        UIOverLord.gameIsPaused = true;
        timer.SetActive(false);
    }

    public void TurnTimerOn()
    {
        UIOverLord.gameIsPaused = false;
        timer.SetActive(true);
    }

    public void ToMenu()
    {
        Application.LoadLevel( "FrontEnd" );
    }

    public void Spawn2DReticle(Camera cam, Vector3 pos)
    {
        if (recticle2D != null)
        {
            GameObject temp = Instantiate(recticle2D);
            temp.transform.SetParent(menu.transform);
            RectTransform tempTransform = temp.GetComponent<RectTransform>();
            tempTransform.transform.position = RectTransformUtility.WorldToScreenPoint(cam, pos);
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
}

