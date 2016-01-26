using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CorrectionUIControl : UIControl 
{
    // Public
    [HideInInspector]
    public uint artContextID;
    public Text currentPainting;
    public Text currentYear;
    public Text currentArtist;
    public Image art;

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
        SetCurrentFields();
    }

}
