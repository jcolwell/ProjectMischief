using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CorrectionUIControl : MonoBehaviour 
{
    [HideInInspector]
    public int artContextID;
    [HideInInspector]
    public Text currentPainting;
    [HideInInspector]
    public Text currentYear;
    [HideInInspector]
    public Text currentArtist;
    [HideInInspector]
    public Image art;

    public void Verify()
    {
        ArtContext curContext = ArtManager.instance.GetPainting( artContextID );
        curContext.currentChoices[0] = currentPainting.text;
        curContext.currentChoices[1] = currentYear.text;
        curContext.currentChoices[2] = currentArtist.text;
    }

    void Awake()
    {
        Time.timeScale = 0.0f;
        currentPainting = GameObject.Find( "PaintingChoice" ).GetComponent<Text>();
        currentYear =     GameObject.Find( "YearChoice" ).GetComponent<Text>();
        currentArtist =   GameObject.Find( "ArtistChoice" ).GetComponent<Text>();
        art =             GameObject.Find( "ArtPiece" ).GetComponent<Image>();
    }
}
