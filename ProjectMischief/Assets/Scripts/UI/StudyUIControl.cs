﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StudyUIControl : UIControl 
{
    // private
    public Image art;
    public Text artName;
    public Text artInfo1;
    public Text artInfo2;
    public Text artInfo3;
    public GameObject nextButton;
    public GameObject prevButton;
    public GameObject startButton;
    public AudioClip song;
    public GameObject levelLoader;

    public GameObject map;
    GameObject cameraMap;

    uint currentContextID;
    uint maxContextID;
    uint highestViewedContextID;

    BackgroundMusicManager manager;

    bool viewedAll = false;

    //Analytics
    float timeInStudyUI = 0.0f;

    //public
    public StudyUIControl()
        : base(UITypes.study)
    { }

        // functions for button
    public void NextArt()
    {
        if(currentContextID < maxContextID)
        {
            ++currentContextID;
            highestViewedContextID = (highestViewedContextID < currentContextID) ? currentContextID : highestViewedContextID;
        }
        UpdateUI();
    }

    public void PrevArt()
    {
        if (currentContextID > 0)
        {
            --currentContextID;
        }
        UpdateUI();
    }

    public void BringUpMap()
    {
        if( map != null && cameraMap != null )
        {
            cameraMap.SetActive( true );
            map.SetActive( true );
        }
    }

    public void CloseMap()
    {
        if( map != null && cameraMap != null )
        {
            cameraMap.SetActive( false );
            map.SetActive( false );
        }
    }

    public void GoToMainMenu()
    {
        LevelLoader loader = Instantiate(levelLoader).GetComponent<LevelLoader>();
        loader.LoadLevel("FrontEnd");
    }

    public void Update()
    {
        if( !UIManager.instance.IsUIActive( UITypes.store) )
        {
            timeInStudyUI += Time.unscaledDeltaTime;
        }
    }

    //private
    void Start()
    {
        viewedAll = false;
        highestViewedContextID = 0;

        currentContextID = 0;
        maxContextID = ArtManager.instance.GetNumPaintings() - 1;

        UpdateUI();
        UIManager.instance.PauseGameTime();

        UIManager.instance.SetVisualCueActive( false );
    }

    void UpdateUI()
    {
        ArtContext curContext = ArtManager.instance.GetPainting(currentContextID);
        viewedAll = (highestViewedContextID == maxContextID);

        art.sprite = curContext.art;
        artName.text = curContext.correctChoices[(int)ArtFields.ePainting];
        artInfo1.text = "Created by " + curContext.correctChoices[(int)ArtFields.eArtist];
        artInfo2.text = "Created in " + curContext.correctChoices[(int)ArtFields.eYear];
        //artInfo3.text = curContext.description;

        nextButton.SetActive(currentContextID != maxContextID);
        prevButton.SetActive(currentContextID != 0);
        startButton.SetActive(currentContextID == maxContextID || viewedAll);
    }

    protected override void DurringDestroy()
    {
        UIManager.instance.UnPauseGameTime();
        UIManager.instance.SetVisualCueActive(true);
    }

        // function for button
    public void LoadShop()
    {
        UIManager.instance.LoadStoreUI();
    }

    protected override void DurringOnEnable()
    {
        manager = UIManager.instance.GetMusicManger();
        manager.ChangeSong( song );

        cameraMap = UIManager.instance.GetMapCamera();

        //Cache LoadingScreen Lifetime so we can cleanup time spent studying
        GameObject loadingScreenObj = GameObject.Find("LoadingScreen(Clone)");
        if (loadingScreenObj != null)
        {
            LoadingScreen ls = loadingScreenObj.GetComponent<LoadingScreen>();
            if (ls != null)
            {
                timeInStudyUI -= (ls.lifeTimeAfterLevelLoaded + ls.crossfadeTime);
            }
        }
    }

    protected override void DurringCloseUI()
    {
        manager = UIManager.instance.GetMusicManger();
        manager.Pause();

        //Analytics
        //Debug.Log("[StudyUI/Analytics] Spent " + timeInStudyUI.ToString() + " seconds in Study UI");
        Analytics.CustomEvent("TimeInStudyUI", new Dictionary<string, object>
        {
            //{"PlayerID", SystemInfo.deviceUniqueIdentifier.ToString() },
            {"ElapsedTime", timeInStudyUI },
            {"LevelNumber", SceneManager.GetActiveScene().name },
        });
    }


}
