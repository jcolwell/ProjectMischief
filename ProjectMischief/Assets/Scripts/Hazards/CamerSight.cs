using UnityEngine;
using System.Collections;

public class CamerSight : MonoBehaviour 
{
    public bool active = true;
        
    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    Transform cameraBody;
    Transform cameraViewCone;
    ParticleSystem spark;
    public int PauseTime = 5;
    GameObject player;
    bool isTurn = true;
    bool isCaught = false;

    public string CameraBodyName = "H_Camera_01_Body_GEO2";
    public string CameraViewConeName = "Camera";

    void Start()
    {
        cameraBody = transform.FindChild( CameraBodyName ).gameObject.transform;

        cameraViewCone = cameraBody.transform.FindChild( CameraViewConeName ).gameObject.transform;
        
        player = GameObject.Find( "Actor" );
        if( null == player )
            player = GameObject.Find( "Actor(Clone)" );
        
        spark = gameObject.GetComponentInChildren<ParticleSystem>();
        spark.Pause();
    }

    void Update()
    {
        if(isCaught)
        {
            PlayerLife playerLife = player.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eCamera, this.transform, spark );
            isCaught = false;
        }

        if( !active && timeElapsed >= timeBeforeReActivation )
        {
            cameraViewCone.gameObject.SetActive( true );
            active = true;
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
    }

    public bool GetActive()
    {
        return active;
    }

    public void DeActivate( float t )
    {
        timeBeforeReActivation = t;
        timeElapsed = 0.0f;
        cameraViewCone.gameObject.SetActive( false );
        active = false;
    }

    IEnumerator pause( bool pause )
    {
        yield return new WaitForSeconds( 5 );
        isTurn = pause;
    }
}
