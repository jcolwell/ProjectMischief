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
    ParticleSystem spark;
    public int PauseTime = 5;
    public float maxAngleVelocity = 0;
    public float maxAngleAcceleration = 0;
    public float rotationDuration = 0;
    public string ActorName = "Actor";


    bool isTurn = true;

    public bool active = true;
    bool isCaught = false;

    void Start()
    {
        eyes = GetComponentInChildren<FOV2DEyes>();
        visionCone = GetComponentInChildren<FOV2DVisionCone>();
        CamObject = gameObject.GetComponent<Transform>();
        cam = transform.FindChild( "CameraSight" ).gameObject;
        player = GameObject.Find( ActorName );
        spark = gameObject.GetComponentInChildren<ParticleSystem>();
        spark.Pause();
    }

    void Update()
    {
        if(isCaught)
        {
            PlayerLife playerLife = player.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eCamera, CamObject, spark );
            isCaught = false;
        }

        if( !active && timeElapsed >= timeBeforeReActivation )
        {
            cam.gameObject.SetActive( true );
            active = true;
        }

        //Turn();
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

   // void Turn()
   // {
   //     if(isTurn)
   //     {
   //         if(cam.gameObject.transform.rotation.eulerAngles.y >= 240 )
   //         {
   //             StartCoroutine( pause( false ) );
   //         }
   //         
   //         else
   //         {
   //             Vector3 turn = new Vector3( 0, 100 * Time.deltaTime, 0 );
   //             cam.gameObject.transform.Rotate( turn );
   //         }
   //     }
   //
   //     else
   //     {
   //
   //         if( cam.gameObject.transform.rotation.eulerAngles.y <= 90 )
   //         {
   //             StartCoroutine( pause( true ) );
   //         }
   //
   //         else
   //         {
   //             Vector3 turn = new Vector3( 0, -100 * Time.deltaTime, 0 );
   //             cam.gameObject.transform.Rotate( turn );
   //         }
   //     }
   //
   // }
   //

    private float CalculateRotation()
    {
        // Time = 
        // NewAngle = CurrentAngle + AngularVelocity( time ) + (0.5)(AngularAcceleration)(time * time);
        
        return 0.0f;
    }

    IEnumerator pause( bool pause )
    {
        yield return new WaitForSeconds( 5 );
        isTurn = pause;
    }
}
