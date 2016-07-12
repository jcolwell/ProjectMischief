using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArtPiece : MonoBehaviour 
{

    public bool randomID = true;
    public int  artID = 0;
    public bool forgery = false;
    public bool correctArtist = true;
    public bool correctYear = true;
    public bool correctName = true;

    bool hasBeenCorrected = false;

    [HideInInspector]
    public bool playerIsInRange = false;

    uint  artContextID = 0; // The ID used to comunicate with artManger
    bool openingMenu = false;
    int  currentTick = 0; // counts how many times Update() has been called since LoadMenu() has been called

    ImageToken token;

    public uint GetArtContextID()
    {
        return artContextID;
    }

    public void SetArtContextID( uint id )
    {
        artContextID = id;
    }
     
    public void LoadMenu()
    {
        UIManager.instance.SetCurrentArtPiece(GetComponent<ArtPiece>());
        UIManager.instance.LoadCorrectionUI();
        openingMenu = true;
        currentTick = 0;
    }

    public bool HasBeenCorrected()
    {
        return hasBeenCorrected;
    }

    public void SetPaintingToBeCorrected()
    {
        hasBeenCorrected = true;
    }

    void OnEnable()
    {
        Renderer rend = gameObject.GetComponent<Renderer>();
        if(rend == null)
        {
            rend = gameObject.AddComponent<MeshRenderer>();
        }
        ArtContext curContext = ArtManager.instance.GetPainting( artContextID );
        rend.material.mainTexture = curContext.art.texture;

        ArtManager.instance.SetPaintingPos(artContextID, gameObject.transform.position);
    }
                  
    void Start()
    {
        ArtContext curContext = ArtManager.instance.GetPainting( artContextID );
        token = GetComponentInChildren<ImageToken>();
        token.painting3D.sprite = curContext.art;
    }
                
    void Update()
    {
        //currentTick is checked to make sure that the uimanager has been loaded
        if( openingMenu == true && currentTick > 0 )
        {
            openingMenu = false;
            UIManager.instance.InitializeArtCorrectionUI(artContextID);
            UIManager.instance.SetCurPaintingToken( ref token );
        }
        ++currentTick;
    }

}
