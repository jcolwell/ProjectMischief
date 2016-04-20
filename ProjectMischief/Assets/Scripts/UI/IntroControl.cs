using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroControl : MonoBehaviour 
{
    enum FrameState
    {
        firstFadeIn,
        display,
        crossFade,
        lastFadeOut
    }

    // varibles
    static bool showIntro = false;
    static bool loadLevelWhenDone = false;
    static int screenTimeOut;
    static string levelToLoad;

    public GameObject levelLoader;

    public float frameDuration = 4.0f;
    public float crossFadeTime = 0.5f;
    public float panSpeed = 0.0f;
    public Sprite[] framesImages = new Sprite[6];
    [MultilineAttribute]
    public string[] framesCaptions = new string[6];
    public Text captionText;
    public Text skipText;
    public Image mainImage;
    public Image fadingInImage;
    public Image backGround;

    RectTransform mainImageTransform;
    RectTransform fadingInImageTransform;
    bool skipIntro = false;
    int curFrame = 0;
    float timeElapsed = 0.0f;
    float deltaAlpha;
    Vector2 firstPos;

    FrameState state = FrameState.firstFadeIn;
    

    //functions
    static public void TurnOnIntro()
    {
        showIntro = true;
        screenTimeOut = Screen.sleepTimeout;
        Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
    }

    static public void SetIntroToLoadLevelWhenDone(string _levelToLoad)
    {
        levelToLoad = _levelToLoad;
        loadLevelWhenDone = true;
    }

    static public void SetIntroToNotLoadLevelWhenDone()
    {
        loadLevelWhenDone = false;
    }

    public void SkipIntro()
    {
        skipIntro = true;
    }

	void Start () 
    {
        mainImageTransform = mainImage.GetComponent<RectTransform>();
        fadingInImageTransform = fadingInImage.GetComponent<RectTransform>();
        firstPos = mainImageTransform.anchoredPosition;

        InitializeIntro();
    }
	
    void InitializeIntro()
    {
        curFrame = 0;
        timeElapsed = 0.0f;
        mainImageTransform.anchoredPosition = firstPos;


        mainImage.sprite = framesImages[curFrame];
        mainImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        fadingInImage.color = mainImage.color;
        backGround.color = Color.black;
        captionText.text = framesCaptions[curFrame];
        deltaAlpha = 1 / crossFadeTime;


        state = FrameState.firstFadeIn;
        skipIntro = false;
        
        captionText.text = framesCaptions[curFrame];
    }

    void Update () 
    {
        if (showIntro)
        {
            showIntro = skipIntro ? false : showIntro;

            timeElapsed += Time.unscaledDeltaTime;
            mainImageTransform.anchoredPosition = new Vector2(mainImageTransform.anchoredPosition.x 
                + (panSpeed * Time.unscaledDeltaTime), mainImageTransform.anchoredPosition.y);

            switch(state)
            {
                case FrameState.firstFadeIn:
                    UpdateFirstFadeIn();
                    break;
                case FrameState.display:
                    UpdateDisplay();
                    break;
                case FrameState.crossFade:
                    UpdateCrossFade();
                    break;
                case FrameState.lastFadeOut:
                    UpdateLastFadeOut();
                    break;
            }

            if (!showIntro)
            {
                if(loadLevelWhenDone)
                {
                    UIManager.instance.CloseAllUI();

                    LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();
                    loader.LoadLevel(levelToLoad);
                }
                Screen.sleepTimeout = screenTimeOut;
                InitializeIntro(); // InitializeIntro for next use
                gameObject.SetActive(false);
            }
        }
	}

    void UpdateFirstFadeIn()
    {
        float deltaAlphaScaled = deltaAlpha * Time.unscaledDeltaTime;

        Color color = mainImage.color;
        color.a += deltaAlphaScaled;
        mainImage.color = color;

        color = skipText.color;
        color.a += deltaAlphaScaled;
        skipText.color = color;

        color = captionText.color;
        color.a += deltaAlphaScaled;
        captionText.color = color;

        if(timeElapsed > crossFadeTime)
        {
            timeElapsed = 0.0f;
            state = FrameState.display;
        }
    }

    void UpdateDisplay()
    {
        if (timeElapsed > frameDuration)
        {
            timeElapsed = 0.0f;
            if (curFrame == framesImages.Length - 1)
            {
                state = FrameState.lastFadeOut;
            }
            else
            {
                state = FrameState.crossFade;
                fadingInImage.sprite = framesImages[++curFrame];
            }
        }
    }

    void UpdateCrossFade()
    {
        fadingInImageTransform.anchoredPosition = new Vector2(fadingInImageTransform.anchoredPosition.x
               + (panSpeed * Time.unscaledDeltaTime), fadingInImageTransform.anchoredPosition.y);

        float deltaAlphaScaled = deltaAlpha * Time.unscaledDeltaTime;

        Color color = mainImage.color;
        color.a -= deltaAlphaScaled;
        mainImage.color = color;

        color = fadingInImage.color;
        color.a += deltaAlphaScaled;
        fadingInImage.color = color;

        if(timeElapsed > crossFadeTime)
        {
            timeElapsed = 0.0f;
            state = FrameState.display;

            mainImageTransform.anchoredPosition = fadingInImageTransform.anchoredPosition;
            fadingInImageTransform.anchoredPosition = firstPos;

            mainImage.sprite = fadingInImage.sprite;
            fadingInImage.color = mainImage.color;
            mainImage.color = Color.white;
            captionText.text = framesCaptions[curFrame];
        }
    }

    void UpdateLastFadeOut()
    {
        float deltaAlphaScaled = deltaAlpha * Time.unscaledDeltaTime;
        
        Color color = mainImage.color;
        color.a -= deltaAlphaScaled;
        mainImage.color = color;

        color = backGround.color;
        color.a -= deltaAlphaScaled;
        backGround.color = color;

        color = skipText.color;
        color.a -= deltaAlphaScaled;
        skipText.color = color;

        color = captionText.color;
        color.a -= deltaAlphaScaled;
        captionText.color = color;

        if (timeElapsed > crossFadeTime)
        {
            showIntro = false;
        }
    }

    void OnDestroy()
    {
        Screen.sleepTimeout = screenTimeOut;
    }
}
