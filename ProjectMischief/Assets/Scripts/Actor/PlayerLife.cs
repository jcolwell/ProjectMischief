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

	public int[] numOfTools = new int[(int)ToolTypes.eToolMAX];
    public GameObject[] tools = new GameObject[(int)ToolTypes.eToolMAX];


    // Use this for initialization
    void Start()
    {
        // load number of tools here.
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
        if( numOfTools[(int)ToolTypes.eMirror] > 0 )
        {
            laser lazer = hazard.gameObject.GetComponent<laser>();
            DeleteAfterInterval interval = tools[(int)ToolTypes.eMirror].GetComponent<DeleteAfterInterval>();
            lazer.DeActivate( interval.lifeTime );
            --numOfTools[(int)ToolTypes.eMirror];
        }
        else
        {
            PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();
            playerCheckPoint.GoToCheckPoint();
        }
    }

    void CaughtByCamera( Transform hazard )
    {
        if( numOfTools[(int)ToolTypes.eJammer] > 0 )
        {
            CamerSight cam = hazard.gameObject.GetComponent<CamerSight>();
            DeleteAfterInterval interval = tools[(int)ToolTypes.eJammer].GetComponent<DeleteAfterInterval>();
            cam.DeActivate( interval.lifeTime );
            --numOfTools[(int)ToolTypes.eJammer];
        }
        else
        {
            PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();
            playerCheckPoint.GoToCheckPoint();
        }
    }

    void CaughtByGuard( Transform hazard )
    {
        if( numOfTools[(int)ToolTypes.eSmokeBomb] > 0 )
        {
        }
        else
        {
            PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();
            playerCheckPoint.GoToCheckPoint();
        }
    }
}