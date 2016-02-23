using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeImageOnClick : MonoBehaviour 
{
    public Sprite imageToChangeItTo;
    Sprite orginalImage;
    public double duration = 1.0f;
    double curTime = 0.0f;
    bool isOrginalSprite = true;

    Image imageComponent;

    UITimer timer = null;

    public void ButtonClicked()
    {
        curTime = duration;
        imageComponent.sprite = imageToChangeItTo;
        isOrginalSprite = false;
    }

    void Start()
    {
        imageComponent = GetComponent<Image>();
        orginalImage = imageComponent.sprite;
        timer = UITimer.GetInstance();
    }

	// Update is called once per frame
	void Update () 
    {
        curTime -= timer.GetDeltaTime();
        if(!isOrginalSprite && curTime <= 0.0f)
        {
            imageComponent.sprite = orginalImage;
            isOrginalSprite = true;
        }
	}
}

public class UITimer : MonoBehaviour 
{
    static UITimer instance = null;

    static public UITimer GetInstance()
    {
        if(instance == null)
        {
            GameObject timerObj = new GameObject();
            instance = timerObj.AddComponent<UITimer>();
            timerObj.name = "Timer";
            instance.lastFramesTime = Time.realtimeSinceStartup;
        }
        return instance;
    }

    // time related varibles
    public double deltaTime = 0;
    public double lastFramesTime;

    public double GetDeltaTime()
    {
        return deltaTime;
    }

    void Awake()
    {
        //instance = this;
        //lastFramesTime = Time.realtimeSinceStartup;
    }

    void Update () 
    {
        CalculateDeltaTime();
	}

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }
}
