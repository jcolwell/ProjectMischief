﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GradingUIControl : UIControl 
{
    //private
    GameObject nextButton;
    GameObject backButton;
    Text IncorrectChoicesText;
    Image art;

    uint currentContextID;
    uint maxContextID;

    // public
    public GradingUIControl()
        : base(UITypes.grading)
    { }

        // Functions for buttons
    public void ToMenu()
    {
        UIManager.instance.CloseAllUI();
        Application.LoadLevel( "FrontEnd" );
    }

    public void RetryLevel()
    {
        UIManager.instance.CloseAllUI();
        Application.LoadLevel( Application.loadedLevel );
    }

    public void LoadNextLevel()
    {
        string nextlevel = UIManager.instance.GetNextLevelToLoad();
        if( nextlevel == null )
        {
            UIManager.instance.CloseAllUI();
            Application.LoadLevel( "FrontEnd" );
        }
        else
        {
            UIManager.instance.CloseAllUI();
            Application.LoadLevel( nextlevel );
        }
    }

	public void LoadLevelSelect()
	{
		UIManager.instance.LoadLevelSelect();
	}

    public void NextArt()
    {
        if( currentContextID < maxContextID )
        {
            ++currentContextID;
        }
        UpdateUI();
    }

    public void PrevArt()
    {
        if( currentContextID > 0 )
        {
            --currentContextID;
        }
        UpdateUI();
    }

	// Private
	void Start () 
    {
		PersistentSceneData data = PersistentSceneData.GetPersistentData ();

        currentContextID = 0;
        maxContextID = ArtManager.instance.GetNumPaintings() - 1;

        // Get relvent objects
		GameObject temp = transform.FindDeepChild("GradeText").gameObject;
        Text  grade = temp.GetComponent<Text>();

		temp = transform.FindDeepChild("TimeElapsedText").gameObject;
        Text timeElapsed = temp.GetComponent<Text>();

		temp = transform.FindDeepChild("IncorrectChoicesText").gameObject;
        IncorrectChoicesText = temp.GetComponent<Text>();

		temp = transform.FindDeepChild("CorrectCorrectionsText").gameObject;
        Text CorrectCorrectionsText = temp.GetComponent<Text>();

		nextButton = transform.FindDeepChild("NextButton").gameObject;
		backButton = transform.FindDeepChild("BackButton").gameObject;

		temp = transform.FindDeepChild("ArtPiece").gameObject;
        art = temp.GetComponent<Image>();

        // fill up the text that will not change
        char letterGrade = ArtManager.instance.GetLetterGrade();
        grade.text = letterGrade.ToString();
        CorrectCorrectionsText.text = "You made " + ArtManager.instance.GetCorrectChanges().ToString() + " correct corrections";

		// mark level as completed
		data.SetLevelCompleted ((uint)Application.loadedLevel, letterGrade); 
		data.SetPlayerCurrency(PersistentSceneData.GetPersistentData().GetPlayerCurrency() + 20);

        float time = UIManager.instance.GetTimeElapsed();
        const int kSec = 60; // num of seconds per minute;
        timeElapsed.text = "Time Elapsed\n" + string.Format("{0}:{1:00}", (int)(time / kSec), (int)(time % kSec));

        // set the text for the text that could change
        UpdateUI();
	}

        // functions to update UI
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
