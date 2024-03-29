﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen :MonoBehaviour
{
    public float lifeTimeAfterLevelLoaded = 0.5f;
    public float crossfadeTime = 1.0f;
    public Image loadingScreenImage;
    public Image nextLoadingScreenImage;
    public Image backGround;

    public GameObject eventSystem;

    bool isCrossFading = false;
    bool isDoneCrossfading = false;

    bool levelLoaded = false;
    public float timeElpased = 0.0f;
    float deltaTime = 0.0f;
    float lastFramesTime = 0.0f;
    float tIncrement;

    Color curLoadingScreenColor;
    Color nextLoadingScreenColor;
    Color backGroundColor;
    void Start()
    {
        DontDestroyOnLoad(this);
        GameObject canvasObject = transform.FindDeepChild("Canvas").gameObject;
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.sortingOrder = 6;

        curLoadingScreenColor = loadingScreenImage.color;
        nextLoadingScreenColor = nextLoadingScreenImage.color;
        backGroundColor = backGround.color;
    }

    public void LevelLoaded()
    {
        levelLoaded = true;
        timeElpased = 0.0f;
        lastFramesTime = Time.realtimeSinceStartup;
        tIncrement = 1 / crossfadeTime;
    }

    void Update()
    {
        CalculateDeltaTime();
        timeElpased += deltaTime;

        if( eventSystem == null )
        {
            eventSystem = GameObject.Find( "EventSystem" );
            if(eventSystem != null)
            {
                eventSystem.SetActive( false );
            }
        }
        

        if(isCrossFading)
        {
            if(timeElpased > crossfadeTime)
            {
                isCrossFading = false;
                isDoneCrossfading = true;
                timeElpased = lifeTimeAfterLevelLoaded * 0.5f;
            }
            curLoadingScreenColor.a -= tIncrement * deltaTime;
            nextLoadingScreenColor.a += tIncrement * deltaTime;
            loadingScreenImage.color = curLoadingScreenColor;
            nextLoadingScreenImage.color = nextLoadingScreenColor;
        }
        else if (levelLoaded && timeElpased >= (lifeTimeAfterLevelLoaded))
        {
            nextLoadingScreenColor.a -= tIncrement * deltaTime;
            nextLoadingScreenImage.color = nextLoadingScreenColor;
            backGroundColor.a -= tIncrement * deltaTime;
            backGround.color = backGroundColor;
            if( timeElpased >= (lifeTimeAfterLevelLoaded) + crossfadeTime )
            {
                if( eventSystem != null )
                {
                    eventSystem.SetActive( true );
                }
                Destroy( gameObject );
            }
        }
        else if( (levelLoaded && timeElpased >= lifeTimeAfterLevelLoaded * 0.5f) && (!isCrossFading && !isDoneCrossfading) )
        {
            isCrossFading = true;
            timeElpased = 0.0f;
        }
    }

    void CalculateDeltaTime()
    {
        float curTime = Time.realtimeSinceStartup;
        deltaTime = curTime - lastFramesTime;
        lastFramesTime = curTime;
    }
}
