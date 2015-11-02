﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GradingUIControl : MonoBehaviour 
{
    GameObject nextButton;
    GameObject backButton;
    Text IncorrectChoicesText;
    Image art;

    int currentContextID;
    int maxContextID;

	// Use this for initialization
	void Start () 
    {
        UIOverLord.instance.RegisterUI(gameObject, UITypes.grading);

        currentContextID = 0;
        maxContextID = ArtManager.instance.GetNumPaintings() - 1;

        // Get relvent objects
        GameObject temp = GameObject.Find("GradeText");
        Text  grade = temp.GetComponent<Text>();

        temp = GameObject.Find("TimeElapsedText");
        Text timeElapsed = temp.GetComponent<Text>();

        temp = GameObject.Find("IncorrectChoicesText");
        IncorrectChoicesText = temp.GetComponent<Text>();

        temp = GameObject.Find("CorrectCorrectionsText");
        Text CorrectCorrectionsText = temp.GetComponent<Text>();

        nextButton = GameObject.Find("NextButton");
        backButton = GameObject.Find("BackButton");

        temp = GameObject.Find("ArtPiece");
        art = temp.GetComponent<Image>();

        // fill up the text that will not change
        char letterGrade = ArtManager.instance.GetLetterGrade();
        grade.text = letterGrade.ToString();
        CorrectCorrectionsText.text = "You made " + ArtManager.instance.GetCorrectChanges().ToString() + " correct corrections";

        float time = UIOverLord.instance.GetTimeElapsed();
        const int kSec = 60; // num of seconds per minute;
        timeElapsed.text = "Time Elapsed\n" + string.Format("{0}:{1:00}", (int)(time / kSec), (int)(time % kSec));

        // set the text for the text that could change
        UpdateUI();
	}

    void OnDestroy()
    {
        UIOverLord.instance.UnRegisterUI(UITypes.level);
    }

    public void ToMenu()
    {
        Application.LoadLevel("FrontEnd");
    }

    public void NextArt()
    {
        if (currentContextID < maxContextID)
        {
            ++currentContextID;
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
        art.sprite = curContext.art;

        bool noIncorrectChoices = true;
        string [] fields = new string[(int)ArtFields.eMax];
        fields[(int)ArtFields.ePainting] = "Painting's Name ";
        fields[(int)ArtFields.eYear] = "Painting's year ";
        fields[(int)ArtFields.eArtist] = "artist's Name ";

        IncorrectChoicesText.text = "";

        for (uint i = 0; i < (int)ArtFields.eMax; ++i )
        {
            if(curContext.currentChoices[i] != curContext.correctChoices[i])
            {
                noIncorrectChoices = false;
                IncorrectChoicesText.text = IncorrectChoicesText.text + "You got the " + fields[i] + 
                    "wrong. The correct " + fields[i] + "was " + curContext.correctChoices[i] + ".\n";
            }
        }

        if (noIncorrectChoices)
        {
            IncorrectChoicesText.text = "You made no wrong corrections";
        }

        nextButton.SetActive(currentContextID != maxContextID);
        backButton.SetActive(currentContextID != 0);
    }

}