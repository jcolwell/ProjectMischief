using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroControl : MonoBehaviour 
{
    // varibles
    static bool showIntro = false;
    static bool firstLoad = true;
    public float frameDuration = 4.0f;
    public float panSpeed = 0.0f;
    public Sprite[] framesImages = new Sprite[6];
    public string[] framesCaptions = new string[6];
    public Text text;
    public Image image;
    RectTransform transform;
    bool skipIntro = false;
    int curFrame = 0;
    float timeElapsed = 0.0f;
    float frameSwitchTime;
    Vector2 firstPos;

    //functions
    static public void TurnOnIntro()
    {
        showIntro = true;
    }

    public void SkipIntro()
    {
        skipIntro = true;
    }

	void Start () 
    {
        if(!firstLoad)
        {
            Destroy(gameObject);
            return;
        }
        transform = image.GetComponent<RectTransform>();
        firstPos = transform.anchoredPosition;
        image.sprite = framesImages[curFrame];
        text.text = framesCaptions[curFrame];
        frameSwitchTime = frameDuration;
	}
	
	void Update () 
    {
        if (showIntro)
        {
            showIntro = skipIntro ? false : showIntro;

            timeElapsed += Time.unscaledDeltaTime;
            transform.anchoredPosition = new Vector2(transform.anchoredPosition.x + (panSpeed * Time.unscaledDeltaTime), transform.anchoredPosition.y);

            bool switchFrame = timeElapsed > frameSwitchTime;
            if (switchFrame && (curFrame + 1) == framesImages.Length)
            {
                showIntro = false;
            }
            else if (switchFrame)
            {
                transform.anchoredPosition = firstPos;
                image.sprite = framesImages[++curFrame];
                text.text = framesCaptions[curFrame];
                frameSwitchTime = frameDuration * (curFrame + 1);
            }
            if (!showIntro)
            {
                Destroy(gameObject);
            }
        }
	}

    void OnDestroy()
    {
        firstLoad = false;
    }
}
