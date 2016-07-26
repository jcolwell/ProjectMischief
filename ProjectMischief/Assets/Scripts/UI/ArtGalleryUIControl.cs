using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArtGalleryUIControl : UIControl
{
    // for the art comprendium
    public Image art;
    public Text artName;
    public Text artInfo1;
    public Text artInfo2;
    public Text artInfo3;

    ArtGalleryUIControl() : base(UITypes.artGallery, -1)
    { }

    void Start()
    {
        ArtContext artContext = ArtManager.instance.GetPainting(UIManager.instance.GetCurrentArt().GetArtContextID());
        art.sprite = Resources.Load<Sprite>(artContext.artFileName);
        artName.text = artContext.correctChoices[(int)ArtFields.ePainting];
        artInfo1.text = "Created by " + artContext.correctChoices[(int)ArtFields.eArtist];
        artInfo2.text = "Created in " + artContext.correctChoices[(int)ArtFields.eYear];
        artInfo3.text = artContext.description;
    }
}
