using UnityEngine;
using System.Collections;

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

public class PlayerLife : MonoBehaviour 
{
    PersistentSceneData data;
	//public int[] numOfTools = new int[(int)ToolTypes.eToolMAX];
    public GameObject[] tools = new GameObject[(int)ToolTypes.eToolMAX];
    //int jammerNumber = 0;


    void Awake()
    {
        data = PersistentSceneData.GetPersistentData();
        data.IncreaseNumTools( ToolTypes.eJammer );
        data.IncreaseNumTools( ToolTypes.eMirror );
        data.IncreaseNumTools( ToolTypes.eSmokeBomb );
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
        int num = data.GetNumTools( ToolTypes.eJammer );

        if( num > 0 )
        {
            laser lazer = hazard.gameObject.GetComponent<laser>();
            DeleteAfterInterval interval = tools[(int)ToolTypes.eMirror].GetComponent<DeleteAfterInterval>();
            lazer.DeActivate( interval.lifeTime );
            //part.Play();
            data.DecreaseNumTools( ToolTypes.eMirror );
        }

        else
        {
            PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();
            playerCheckPoint.GoToCheckPoint();
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
            PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();
            playerCheckPoint.GoToCheckPoint();
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