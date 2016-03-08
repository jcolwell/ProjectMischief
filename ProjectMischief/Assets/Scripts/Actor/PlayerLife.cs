//======================================================
// File: PlayerLife.cs
// Description:    This Script will control how the tools and hazards effect the player
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
//======================================================

//======================================================
// Enums
//======================================================
public enum ToolTypes
{
    eMirror,
    eJammer,
    eSmokeBomb,
    eToolMAX
}

public enum HazardTypes
{
    eLazer,
    eCamera,
    eGaurd,
    eHazardMAX
}

//======================================================
// Class PlayerLife
//======================================================
public class PlayerLife : MonoBehaviour 
{
    //======================================================
    // Public
    //======================================================
    public GameObject[] tools = new GameObject[ ( int )ToolTypes.eToolMAX ];
    public string GaurdManagerName = "GuardManager";
    public AudioClip alarm;

    public float cameraCoolDown = 3;
    public float laserCoolDown = 3;
    //======================================================

    //======================================================
    // Private
    //======================================================
    PersistentSceneData data;
    GuardDispatchManager dispatchManager;
    AudioSource soundSource;
    //======================================================
    

    void Start()
    {
        GameObject manager = GameObject.Find( GaurdManagerName );
        dispatchManager = manager.GetComponent<GuardDispatchManager>();

        soundSource = gameObject.GetComponent<AudioSource>();
    }

    void Awake()
    {
        data = PersistentSceneData.GetPersistentData();
    }

    public void CaughtPlayer( HazardTypes hazardType, Transform hazard, ParticleSystem part)
    {
        switch( hazardType )
        {
        case HazardTypes.eLazer:
            CaughtByLazer( hazard, part );
            break;

        case HazardTypes.eCamera:
            CaughtByCamera( hazard, part );
            break;

        case HazardTypes.eGaurd:
            CaughtByGuard( hazard, part );
            break;  
        }
    }

    void CaughtByLazer( Transform hazard, ParticleSystem part )
    {
        int num = data.GetNumTools( ToolTypes.eMirror );

        if( num > 0 )
        {
            laser lazer = hazard.gameObject.GetComponent<laser>();
            lazer.DeActivate( laserCoolDown );
            data.DecreaseNumTools( ToolTypes.eMirror );
            UIManager.instance.UpdateToolCount();
            UIManager.instance.UsedTool( ToolTypes.eMirror );
        }
        else
        {
            if( !soundSource.isPlaying )
            {
                soundSource.PlayOneShot( alarm );
            }
            dispatchManager.DispatchGuard( transform.position );
        }
    }

    void CaughtByCamera( Transform hazard, ParticleSystem part )
    {
        int num = data.GetNumTools( ToolTypes.eJammer );

        if( num > 0 )
        {
            CamerSight cam = hazard.gameObject.GetComponent<CamerSight>();
            cam.DeActivate( cameraCoolDown );
            part.Play();
            data.DecreaseNumTools( ToolTypes.eJammer );
            UIManager.instance.UpdateToolCount();
            UIManager.instance.UsedTool( ToolTypes.eJammer );
        }
        else
        {
            if(!soundSource.isPlaying)
            {
                soundSource.PlayOneShot( alarm );
            }
            dispatchManager.DispatchGuard( transform.position );
        }
    }

    void CaughtByGuard( Transform hazard, ParticleSystem part )
    {
        int num = data.GetNumTools( ToolTypes.eSmokeBomb );

        if( num > 0 )
        {
            data.DecreaseNumTools( ToolTypes.eSmokeBomb );

            part = ParticleSystem.Instantiate( part );
            part.transform.position = new Vector3( this.transform.position.x, this.transform.position.y, this.transform.position.z );
            print( "Pos : " + this.transform.position.x + this.transform.position.y + this.transform.position.z );
            part.time = 0.01f;
            part.Play();

            UIManager.instance.UpdateToolCount();
            UIManager.instance.UsedTool( ToolTypes.eSmokeBomb );
        }
        else
        {
            UIManager.instance.ActivatePlayerCaughtPopUp();
            PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();
            playerCheckPoint.GoToCheckPoint();
        }
    }
}