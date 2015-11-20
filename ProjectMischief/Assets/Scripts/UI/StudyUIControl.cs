using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StudyUIControl : MonoBehaviour {

    Image art;
    Text artName;
    Text artInfo;
    GameObject nextButton;
    GameObject backButton;
    GameObject startButton;

    int currentContextID;
    int maxContextID;
    int highestViewedContextID;

    bool viewedAll = false;

	// Use this for initialization
	void Start () 
    {
        UIOverLord.instance.RegisterUI(gameObject, UITypes.study);

        viewedAll = false;
        highestViewedContextID = 0;

        currentContextID = 0;
        maxContextID = ArtManager.instance.GetNumPaintings() - 1;

        nextButton = GameObject.Find("NextButton");
        backButton = GameObject.Find("BackButton");
        startButton = GameObject.Find("StartButton");

        GameObject temp = GameObject.Find("ArtInfo");
        artInfo = temp.GetComponent<Text>();

        temp = GameObject.Find("ArtName");
        artName = temp.GetComponent<Text>();

        temp = GameObject.Find("ArtPiece");
        art = temp.GetComponent<Image>();

        UpdateUI();
        UIOverLord.gameIsPaused = true;

        UIOverLord.instance.SetVisualCueActive( false );
	}

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

    void UpdateUI()
    {
        ArtContext curContext = ArtManager.instance.GetPainting(currentContextID);
        viewedAll = (highestViewedContextID == maxContextID);

        art.sprite = curContext.art;
        artName.text = curContext.correctChoices[(int)ArtFields.ePainting];
        artInfo.text = "Created in " + curContext.correctChoices[(int)ArtFields.eYear] + "\n" + "Created by " +
            curContext.correctChoices[(int)ArtFields.eArtist];

        nextButton.SetActive(currentContextID != maxContextID);
        backButton.SetActive(currentContextID != 0);
        startButton.SetActive(currentContextID == maxContextID || viewedAll);
    }

    void OnDestroy()
    {
        UIOverLord.gameIsPaused = false;
        UIOverLord.instance.SetVisualCueActive( true );
        UIOverLord.instance.UnRegisterUI(UITypes.study);
    }
}
