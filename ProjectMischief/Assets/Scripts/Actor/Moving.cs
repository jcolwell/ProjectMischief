//======================================================
// File: GuardAI.cs
// Description:    This Script will drive Guard AI
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
//======================================================


//======================================================
// Class Moving
//======================================================
public class Moving : MonoBehaviour 
{
    //======================================================
    // Public
    //======================================================

    public string floorTag;
    public string PictureTag;
    public Quaternion lookRotation;
    public LayerMask cullingMask;
    public float walkingSpeed;
    public float runningSpeed;
    public bool use2DReticle = false;
    public GameObject movementReticle;
    public AudioClip walking;
    public AudioClip running;

    //======================================================

    //======================================================
    // Private
    //======================================================
    bool leftClickFlag = true;
    float speed;
    float soundDelay;

    AudioSource sound;
    Vector3 Target;
    RaycastHit hit;
    NavMeshAgent agent;
    AnimController animation;

    //======================================================

    void Start()
    {
        animation = GetComponent<AnimController>();
        Target = transform.position;
        agent = GetComponent<NavMeshAgent>();
        speed = walkingSpeed;
        sound = GetComponent<AudioSource>();
        sound.clip = walking;
        soundDelay = 0.01f;
    }

    //======================================================

	void Update ()
    {
        if (!agent.enabled)
        {
            return;
        }

#if UNITY_ANDROID

        if( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began )
        {
            Ray ray = Camera.main.ScreenPointToRay( Input.GetTouch(0).position);
            if( Physics.Raycast( ray, out hit, 100, cullingMask ) && Time.timeScale != 0.0f )
            {
                Movement();
            }
        }
#else
        if ( Input.GetKey( KeyCode.Mouse0 ) && leftClickFlag )
        {
            leftClickFlag = false;
        }

        if( !Input.GetKey( KeyCode.Mouse0 ) && !leftClickFlag )
        {
            leftClickFlag = true;
            Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

            if( Physics.Raycast( ray, out hit, 100, cullingMask ) && Time.timeScale != 0.0f )
            {
                Movement();
            }
        }
#endif

        UpdateStealth(speed);
	}

    //======================================================

    //Controls the movement
    void Movement()
    {
        if( hit.transform.tag == floorTag )
        {
            if( animation.GetState() != AnimController.State.Walk )
            {
                if( animation.GetGuardState() == AnimController.State.Run )
                {
                    animation.ChangeState( AnimController.State.Run );
                    sound.clip = running;
                    soundDelay = 0.001f * runningSpeed;
                    SetSpeed( runningSpeed );
                }

                else
                {
                    animation.ChangeState( AnimController.State.Walk );
                    sound.clip = walking;
                    soundDelay = 0.01f * walkingSpeed;
                    SetSpeed( walkingSpeed );
                }
            }

            if( movementReticle != null && !use2DReticle )
            {
                Instantiate( movementReticle, hit.point, Quaternion.identity );
            }

            else if( use2DReticle )
            {
                UIManager.instance.Spawn2DReticle( Camera.main, hit.point );
            }

            float X = hit.point.x;
            float Z = hit.point.z;
            Target = new Vector3( X, gameObject.transform.position.y, Z );
            agent.SetDestination( Target );
            agent.CalculatePath(Target, agent.path);
        }

        if( hit.transform.tag == PictureTag )
        {
            ArtPiece art = hit.collider.gameObject.GetComponent<ArtPiece>();
            GetComponentInChildren<Sensor>().CheckIfInRange( hit.collider );

            if( art.playerIsInRange == true )
            {
                art.LoadMenu();
                PlayerCheckPoint playerCheckPoint = gameObject.GetComponent<PlayerCheckPoint>();

                if( playerCheckPoint != null )
                {
                    playerCheckPoint.SetCheckPoint( gameObject.transform.position );
                }
            }
        }
    }

    //======================================================

    //Main update for moving
    void UpdateStealth(float speed)
    {
        agent.speed = speed;

        if( !sound.isPlaying )
        {
            //sound.loop = true;
            sound.PlayDelayed( soundDelay );
        }

        float dist = agent.remainingDistance;

        if (dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && dist == 0)
        {
            sound.Pause();
            animation.ChangeState( AnimController.State.Idle);
            return;
        }
    }

    //======================================================

    //For Equipment Stats
    public void SetSpeed(float s)
    {
        speed = s;
    }

    //======================================================

    //Set new Target
    public void setTarget(Vector3 t)
    {
        Target = t;
        agent.ResetPath();
    }

    //======================================================

    //Reset the Path
    public void Reset()
    {
        agent.ResetPath();
        agent.velocity = new Vector3();
    }

    //======================================================

    //Screw around with the Navmesh
    public void ToggleNavMeshAgent()
    {
        agent.enabled = !agent.enabled;
    }
}

