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
    public float speed;
    public bool use2DReticle = false;
    public GameObject movementReticle;

    //======================================================

    //======================================================
    // Private
    //======================================================
    bool leftClickFlag = true;
    bool isWalking = false;

    Vector3 pos;
    Vector3 Target;
    RaycastHit hit;
    NavMeshAgent agent;
    AnimationController animation;

    //======================================================

    void Start()
    {
        animation = GetComponent<AnimationController>();
        pos = gameObject.transform.position;
        Target = transform.position;
        agent = GetComponent<NavMeshAgent>();
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
            isWalking = true;
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
        //animation.ChangeState( AnimationController.State.Walk );
        if( hit.transform.tag == floorTag )
        {
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
        //animation.ChangeState( AnimationController.State.Walk );
        //if(!isWalking)
        //{
        //    animation.ChangeState( AnimationController.State.Idle );
        //}
        //else
        //{
        //    animation.ChangeState( AnimationController.State.Walk );
        //}

        agent.SetDestination(Target);

        agent.speed = speed;

        //if( V3Equal(pos, Target) )
        //{
        //    print( "GOD FUCKING HATE ANIMATION" + isWalking );
        //    isWalking = false;
        //    return;
        //}
        //if(agent.remainingDistance <= 1.0f)
        //{
        //    print( "GOD FUCKING HATE ANIMATION" + isWalking );
        //    isWalking = false;
        //    return;
        //}
         float dist= agent.remainingDistance;
        if (dist!=Mathf.Infinity && agent.pathStatus==NavMeshPathStatus.PathComplete && dist == 0)
        {
            isWalking = false;
            
            //animation.ChangeState( AnimationController.State.Idle);
            return;
        }
    }

    //======================================================

    //For Equipment Stats
    public void SetSpeed(int s)
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

    public bool V3Equal( Vector3 a, Vector3 b )
    {
        return Vector3.SqrMagnitude( a - b ) < 0.0001f;
    }

}

