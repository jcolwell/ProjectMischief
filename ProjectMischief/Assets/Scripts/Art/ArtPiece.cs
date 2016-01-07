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

    [HideInInspector]
    public bool playerIsInRange = false;

    uint  artContextID = 0; // The ID used to comunicate with artManger
    bool openingMenu = false;
    int  currentTick = 0; // counts how many times Update() has been called since LoadMenu() has been called

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
        UIManager.instance.LoadCorrectionUI();
        openingMenu = true;
        currentTick = 0;
    }

    void Awake()
    {
        Renderer rend = gameObject.GetComponent<Renderer>();
        if(rend == null)
        {
            rend = gameObject.AddComponent<MeshRenderer>();
        }
        ArtContext curContext = ArtManager.instance.GetPainting(artContextID);
        rend.material.mainTexture = curContext.art.texture;

        ArtManager.instance.SetPaintingPos(artContextID, gameObject.transform.position);
    }
                                      
    void Update()
    {
        //currentTick is checked to make sure that the uimanager has been loaded
        if( openingMenu == true && currentTick > 0 )
        {
            openingMenu = false;
            UIManager.instance.InitializeArtCorrectionUI(artContextID);
        }
        ++currentTick;
    }

}
