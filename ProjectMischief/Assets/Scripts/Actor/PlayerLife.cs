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
    //======================================================

    //======================================================
    // Private
    //======================================================
    PersistentSceneData data;
    GuardDispatchManager dispatchManager;
    //======================================================
    

    void Start()
    {
        GameObject manager = GameObject.Find( GaurdManagerName );
        dispatchManager = manager.GetComponent<GuardDispatchManager>();
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
            DeleteAfterInterval interval = tools[(int)ToolTypes.eMirror].GetComponent<DeleteAfterInterval>();
            lazer.DeActivate( interval.lifeTime );
            data.DecreaseNumTools( ToolTypes.eMirror );
        }

        else
        {
            dispatchManager.DispatchGuard( transform.position );
        }
    }

    void CaughtByCamera( Transform hazard, ParticleSystem part )
    {
        int num = data.GetNumTools( ToolTypes.eJammer );

        if( num > 0 )
        {
            CamerSight cam = hazard.gameObject.GetComponent<CamerSight>();
            DeleteAfterInterval interval = tools[(int)ToolTypes.eJammer].GetComponent<DeleteAfterInterval>();
            cam.DeActivate( interval.lifeTime );
            part.Play();
            data.DecreaseNumTools( ToolTypes.eJammer );
        }

        else
        {
            dispatchManager.DispatchGuard( transform.position );
        }
    }

    void CaughtByGuard( Transform hazard, ParticleSystem part )
    {
        int num = data.GetNumTools( ToolTypes.eSmokeBomb );

        if( num > 0 )
        {
            data.DecreaseNumTools( ToolTypes.eSmokeBomb );
        }

        else
        {
            PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();
            playerCheckPoint.GoToCheckPoint();
        }
    }
}