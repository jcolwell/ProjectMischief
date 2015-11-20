using UnityEngine;
using System.Collections;

public class CamerSight : MonoBehaviour 
{
    FOV2DEyes eyes;
    FOV2DVisionCone visionCone;

    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    Transform CamObject;
    GameObject cam;
    GameObject player;

    public bool active = true;
    bool isCaught = false;

    void Start()
    {
        eyes = GetComponentInChildren<FOV2DEyes>();
        visionCone = GetComponentInChildren<FOV2DVisionCone>();
        CamObject = gameObject.GetComponent<Transform>();
        cam = GameObject.Find( "CameraSight" );
        player = GameObject.Find( "Actor(Clone)" );
    }

    void Update()
    {
        if(isCaught)
        {
            PlayerLife playerLife = player.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eCamera, CamObject );
            isCaught = false;
        }

        if( !active && timeElapsed >= timeBeforeReActivation )
        {
            cam.gameObject.SetActive( true );
            active = true;
        }

        timeElapsed += Time.deltaTime;
    }

    public void Caught()
    {
        isCaught = true;
    }

    public bool GetActive()
    {
        return active;
    }

    public void DeActivate( float t )
    {
        timeBeforeReActivation = t;
        timeElapsed = 0.0f;
        cam.gameObject.SetActive( false );
        active = false;
    }
}
