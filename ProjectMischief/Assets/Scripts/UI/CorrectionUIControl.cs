﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CorrectionUIControl : UIControl 
{
    // Public
    public GameObject correctionMenu;
    [HideInInspector]
    public uint artContextID;
    public Text currentPainting;
    public Text currentYear;
    public Text currentArtist;

    public Text paintingNumberText;
    public string paintingNumberExtraText = "Painting #";
    public Image art;

    public Color buttonFadeOutColor;
    public GameObject correctTitleButton;
    public GameObject correctYearButton;
    public GameObject correctArtistButton;

    //public
    CorrectionUIControl()
        : base(UITypes.Correction)
    { }

    public void SetCurrentFields()
    {
        ArtContext curContext = ArtManager.instance.GetPainting( artContextID );
        art.sprite = curContext.art;
        currentPainting.text = curContext.currentChoices[0];
        currentYear.text = curContext.currentChoices[1];
        currentArtist.text = curContext.currentChoices[2];
        if(paintingNumberText != null)
        {
            paintingNumberText.text = paintingNumberExtraText + (artContextID + 1);
        }
    }

        // Functions for Button
    public void Verify()
    {
        ArtContext curContext = ArtManager.instance.GetPainting( artContextID );
        curContext.currentChoices[0] = currentPainting.text;
        curContext.currentChoices[1] = currentYear.text;
        curContext.currentChoices[2] = currentArtist.text;
        UIManager.instance.SetPaintingIteractedWith(true, artContextID);
    }

    //Private
    void Awake()
    {
        correctionMenu.SetActive(false);

        Image buttonImage = correctTitleButton.GetComponent<Image>();
        Button buttonScript = correctTitleButton.GetComponent<Button>();
        buttonScript.enabled = ArtManager.instance.enableTitleCategory;
        if (ArtManager.instance.enableTitleCategory)
        {
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = buttonFadeOutColor;
        }

        buttonImage = correctYearButton.GetComponent<Image>();
        buttonScript = correctYearButton.GetComponent<Button>();
        buttonScript.enabled = ArtManager.instance.enableYearCategory;
        if (ArtManager.instance.enableYearCategory)
        {
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = buttonFadeOutColor;
        }

        buttonImage = correctArtistButton.GetComponent<Image>();
        buttonScript = correctArtistButton.GetComponent<Button>();
        buttonScript.enabled = ArtManager.instance.enableArtistCategory;
        if (ArtManager.instance.enableArtistCategory)
        {
            buttonImage.color = Color.white;
        }
        else
        {
            buttonImage.color = buttonFadeOutColor;
        }


        SetCurrentFields();
    }

    // protected
    protected override void DurringCloseUI()
    {
        GameObject player = GameObject.Find("Actor");
        if(player == null)
        {
            player = GameObject.Find("Actor(Clone)");
            if (player == null)
            {
                return;
            }
        }
        Moving playerMover = player.GetComponent<Moving>();
        playerMover.ResetClick();

    }
}
