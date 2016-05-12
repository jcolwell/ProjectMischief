//======================================================
// File: PlayerLife.cs
// Description:    This Script will control how the tools and hazards effect the player
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using UnityEngine.Analytics;
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
    public GameObject spark;
    public GameObject jammer;
    public GameObject smokebomb;

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

    public void CaughtPlayer( HazardTypes hazardType, Transform hazard)
    {
        switch( hazardType )
        {
        case HazardTypes.eLazer:
            CaughtByLazer( hazard );
            break;

        case HazardTypes.eCamera:
            CaughtByCamera( hazard );
            break;

        case HazardTypes.eGaurd:
            CaughtByGuard( hazard );
            break;  
        }
    }

    void CaughtByLazer( Transform hazard )
    {
        int num = data.GetNumTools( ToolTypes.eMirror );

        if( num > 0 )
        {
            laser lazer = hazard.gameObject.GetComponentInParent<laser>();
            lazer.DeActivate( laserCoolDown );

            ParticleSystem part = spark.GetComponent<ParticleSystem>();
            part = ParticleSystem.Instantiate(part);
            part.transform.position = hazard.position;
            part.Play();
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

    void CaughtByCamera( Transform hazard)
    {
        int num = data.GetNumTools( ToolTypes.eJammer );

        if( num > 0 )
        {
            CamerSight cam = hazard.gameObject.GetComponentInParent<CamerSight>();
            cam.DeActivate( cameraCoolDown );
            ParticleSystem part = jammer.GetComponent<ParticleSystem>();
            part = ParticleSystem.Instantiate(part);
            part.transform.position = hazard.position;
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

    void CaughtByGuard( Transform hazard )
    {
        int num = data.GetNumTools( ToolTypes.eSmokeBomb );

        if( num > 0 )
        {
            data.DecreaseNumTools( ToolTypes.eSmokeBomb );

            ParticleSystem part = smokebomb.GetComponent<ParticleSystem>();
            part = ParticleSystem.Instantiate(part);
            part.transform.position = new Vector3( this.transform.position.x, this.transform.position.y, this.transform.position.z );
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