using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CorrectionUIControl : UIControl 
{
    // Public
    [HideInInspector]
    public uint artContextID;
    [HideInInspector]
    public Text currentPainting;
    [HideInInspector]
    public Text currentYear;
    [HideInInspector]
    public Text currentArtist;
    [HideInInspector]
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
		currentPainting = transform.FindDeepChild( "PaintingChoice" ).GetComponent<Text>();
		currentYear = transform.FindDeepChild( "YearChoice" ).GetComponent<Text>();
		currentArtist = transform.FindDeepChild( "ArtistChoice" ).GetComponent<Text>();
		art = transform.FindDeepChild( "ArtPiece" ).GetComponent<Image>();
        SetCurrentFields();
    }

}
