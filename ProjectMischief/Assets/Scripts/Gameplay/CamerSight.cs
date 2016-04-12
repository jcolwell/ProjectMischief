//======================================================
// File: CamerSight.cs
// Description:    This Script will control camera sight
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
//======================================================


//======================================================
// Class CamerSight
//======================================================
public class CamerSight : MonoBehaviour 
{
    //======================================================
    // Public
    //======================================================
    public bool isActive = true;
    public string CameraBodyName = "H_Camera_01_Body_GEO2";
    public string CameraViewConeName = "Camera";
    public int PauseTime = 5;
    public GameObject Spawn;
    //======================================================

    //======================================================
    // Private
    //======================================================
    GameObject player;
    Transform cameraBody;
    Transform cameraViewCone;
    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    bool isCaught = false;
    bool dispatchCalled = false;
    //======================================================
        
    void Start()
    {
        cameraBody = transform.FindChild( CameraBodyName ).gameObject.transform;

        cameraViewCone = cameraBody.transform.FindChild( CameraViewConeName ).gameObject.transform;
        
        player = GameObject.Find( "Actor" );

        if( player == null)
        {
            player = GameObject.Find( "Actor(Clone)" );
        }

        //spark = gameObject.GetComponentInChildren<ParticleSystem>();
        //spark.Pause();
    }

    void Update()
    {
        if(isCaught && !dispatchCalled)
        {
            PlayerLife playerLife = player.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer(HazardTypes.eCamera, Spawn.transform);//, spark );
            isCaught = false;
            dispatchCalled = true;
        }

        if( !isActive && timeElapsed >= timeBeforeReActivation )
        {
            cameraViewCone.gameObject.SetActive( true );
            isActive = true;
        }

        timeElapsed += Time.deltaTime;
    }

    public void PlayerVisible()
    {
        isCaught = true;
    }

    public void PlayerNotVisible()
    {
        isCaught = false;
        dispatchCalled = false;
    }

    public bool GetActive()
    {
        return isActive;
    }

    public void DeActivate( float t )
    {
        timeBeforeReActivation = t;
        timeElapsed = 0.0f;
        cameraViewCone.gameObject.SetActive( false );
        isActive = false;
    }

    public void AskStatus()
    {
    }
}
