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

    int  artContextID = 0; // The ID used to comunicate with artManger
    bool openingMenu = false;
    int  currentTick = 0; // counts how many times Update() has been called since LoadMenu() has been called

    public int GetArtContextID()
    {
        return artContextID;
    }

    public void SetArtContextID( int id )
    {
        artContextID = id;
    }

    public void LoadMenu()
    {
        Application.LoadLevelAdditive( "UITest" );
        openingMenu = true;
        currentTick = 0;
    }

    void Start()
    {
        Renderer rend = gameObject.GetComponent<Renderer>();
        if(rend == null)
        {
            rend = gameObject.AddComponent<MeshRenderer>();
        }
        ArtContext curContext = ArtManager.instance.GetPainting(artContextID);
        rend.material.mainTexture = curContext.art.texture;

        UIManager.instance.SetPaintingPos((uint)artContextID, gameObject.transform.position);
    }
                                      
    void Update()
    {
        //currentTick is checked to make sure that the uimanager has been loaded
        if( openingMenu == true && currentTick > 0 )
        {
            openingMenu = false;
            GameObject uiMangerGameObject = GameObject.Find( "UIMangerCorrection" );
            CorrectionUIControl uiControl = uiMangerGameObject.GetComponent<CorrectionUIControl>();
            if (uiControl != null)
            {
                uiControl.artContextID = artContextID;
                uiControl.SetCurrentFields();
            }
        }
        ++currentTick;
    }

}
