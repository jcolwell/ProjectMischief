using UnityEngine;
using UnityEngine.SceneManagement;
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
    public bool isArtGallery = false;

    bool isUnlocked = true;

    bool hasBeenCorrected = false;
    Light light;

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
        if (!isArtGallery || isUnlocked)
        {         
            UIManager.instance.SetCurrentArtPiece(GetComponent<ArtPiece>());
            openingMenu = true;
            currentTick = 0;

            if (SceneManager.GetActiveScene().name == "ArtGallery")
            {
                UIManager.instance.LoadArtGalleryUI();
            }
            else
            {
                UIManager.instance.LoadCorrectionUI();
            }
        }
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

        light = gameObject.GetComponentInChildren<Light>();
        Renderer rend = gameObject.GetComponent<Renderer>();
        if(rend == null)
        {
            rend = gameObject.AddComponent<MeshRenderer>();
        }

        ArtContext curContext = ArtManager.instance.GetPainting( artContextID );

        isUnlocked = PersistentSceneData.GetPersistentData().IsEncounteredArt((uint)curContext.artID);
        ArtManager.instance.SetPaintingPos(artContextID, gameObject.transform.position);

        if (isArtGallery && !isUnlocked)
        {
            light.gameObject.SetActive(false);
            return;
        }

        rend.material.mainTexture = curContext.art.texture;
        rend.material.color = Color.white;
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
