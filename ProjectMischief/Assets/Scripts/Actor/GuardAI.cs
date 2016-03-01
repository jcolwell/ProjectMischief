//======================================================
// File: GuardAI.cs
// Description:    This Script will drive Guard AI
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
using System.Resources;
using System.Runtime.Remoting.Messaging;

//======================================================


//======================================================
// Class GuardAI
//======================================================
public class GuardAI : MonoBehaviour
{
    //==================================================
    // Private Variables
    //==================================================
    public enum State
    {
        Idle = 0,
        Alert,
        FollowUp,
        Chase,
        Sleeping,
    }

    private State currentState;


    private NavMeshAgent agent;
    private VisionCone vision;

    private int wayTarget;
    
    private Vector3 playerPosition;
    
    private Vector3 homePosition;
    private Quaternion homeRotation;


    private AnimController anime;
    private AnimController playerAnime;

    private bool isPlayerVisible = false;
    private bool isInvestigating = false;
    private bool isTargetingWall = false;

    private float regularMoveSpeed = 0.0f;
    private float alertMoveSpeed = 0.0f;
    //==================================================

    //==================================================
    // Public Variables
    //==================================================
    public ParticleSystem smokeBombEffect;
    public GameObject[] waypoints;
    public float distanceFromWaypoint = 1.0f;
    public float moveSpeedMultiplier = 1.5f;
    //==================================================


    //==================================================

	void Start () 
    {

        GameObject player = GameObject.Find( "Actor" );

        if( player == null )
        {
            player = GameObject.Find( "Actor(Clone)" );
        }

        playerAnime = player.gameObject.GetComponent<AnimController>();

        homePosition = gameObject.transform.position;
        homeRotation = gameObject.transform.rotation;

        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<VisionCone>();
        anime = GetComponentInChildren<AnimController>();

        if( waypoints.Length > 0 )
        {
            wayTarget = 0;
            agent.SetDestination( waypoints[wayTarget].transform.position );
            currentState = State.Idle;
        }

        playerPosition = new Vector3();

        regularMoveSpeed = agent.speed;
        alertMoveSpeed = agent.speed * moveSpeedMultiplier;
	}

    //==================================================
	
    void Update () 
    {
        //Debug.Log(currentState.ToString());

	    switch( currentState )
        {
            case State.Idle:
                currentState = Idle();
                break;
            case State.Alert:
                currentState = Alert();
                break;
            case State.FollowUp:
                currentState = FollowUp();
                break;
            case State.Chase:
                currentState = Chase();
                break;
            case State.Sleeping:
                currentState = Sleeping();
                break;
        }
	}

    //==================================================

    void OnTriggerEnter( Collider col )
    {
        if( col.CompareTag("Player"))
        {
            PlayerLife player = col.transform.parent.GetComponent<PlayerLife>();
            //smokeBombEffect = col.gameObject.GetComponentInChildren<ParticleSystem>();
            player.CaughtPlayer( HazardTypes.eGaurd, this.transform, smokeBombEffect );
            
            isPlayerVisible = false;
            isInvestigating = false;
            currentState = State.Idle;
            SendMessageUpwards( "ReportInteruterNeutralized" );

            agent.Warp( homePosition );
            gameObject.transform.rotation = homeRotation;
        }
    }

    //==================================================
    // Idle: Not much is going on... Just follow your route

    private State Idle()
    {
        agent.speed = regularMoveSpeed;
        if( isPlayerVisible )
        {
            return State.Chase;
        }
        if( isInvestigating )
        {
            return State.Alert;
        }
        //Determine Distance to target
        if( waypoints.Length > 0 && !( agent.pathPending || agent.remainingDistance > distanceFromWaypoint ) )
        {
            anime.ChangeState( AnimController.State.Walk );
            playerAnime.SetGuardState( AnimController.State.Walk );

            playerAnime.ChangeState( AnimController.State.Walk );
            wayTarget = (wayTarget + 1) % waypoints.Length; 
          
            //Update destination
            Vector3 tarPos = waypoints[wayTarget].transform.position;
            Vector3 destination = new Vector3( tarPos.x, transform.position.y, tarPos.z );
            
            //Travel to destination
            agent.SetDestination( destination );
        }

        return State.Idle;
    }

    //==================================================
    // Alert: Player has been seen! Go look for them!

    private State Alert()
    {
        State returnState = State.Alert;
        agent.speed = alertMoveSpeed;
        //agent.destination = playerPosition;
        agent.SetDestination( playerPosition );
        
        if( isPlayerVisible )
        {
            returnState = State.Chase;
        }
        
        if( !agent.pathPending  && agent.remainingDistance < agent.stoppingDistance )
        {
            anime.ChangeState( AnimController.State.Walk );
            playerAnime.SetGuardState( AnimController.State.Walk );

            playerAnime.ChangeState( AnimController.State.Walk );
            isInvestigating = false;
            returnState = State.FollowUp;
        }

        return returnState;
    }

    //==================================================
    // Alert: Player has been seen! Go look for them!

    private State FollowUp()
    {
        if( isPlayerVisible )
        {
            return State.Chase;
        }

        //Path to the closest wall if not targetting a wall;
        if( !isTargetingWall )
        {
            //Find closest wall outside view cone (with heavy forward bias)
            RaycastHit hit;
            Physics.Raycast( this.transform.position, this.transform.forward, out hit );
            if( hit.distance > vision.dist_max )
            {
                playerPosition = hit.collider.transform.position;
                isTargetingWall = true;
            }
            else
            {
                // find a different wall;
                isTargetingWall = false;
                SendMessageUpwards( "ReportInteruterNeutralized" );
                return State.Idle;
            }
        }

        agent.destination = playerPosition;
        Debug.Log( agent.remainingDistance );
        if( !(agent.remainingDistance > agent.stoppingDistance) )
        {
            //PAN LEFT RIGHT
            SendMessageUpwards( "ReportInteruterNeutralized" );
            isTargetingWall = false;
            return State.Idle;
        }
        return State.FollowUp;
    }


    //==================================================
    // Chase: You see the player! Catch them!
    
    private State Chase()
    {
        //agent.destination = playerPosition;
        agent.SetDestination( playerPosition );
        
        //KIMS FIRST HACK
        if( anime.GetState() != AnimController.State.Run )
        {
            anime.ChangeState( AnimController.State.Run );
            playerAnime.SetGuardState( AnimController.State.Run );

            playerAnime.ChangeState( AnimController.State.Run );
        }

        if( isPlayerVisible && agent.remainingDistance > 0 )
        {
            return State.Chase;
        }

        isInvestigating = true;
        return State.Alert;
    }

    //==================================================
    
    private State Sleeping()
    {
        return State.Idle;
    }

    //==================================================
    // Public Messaging functions

    public void PlayerVisible( Vector3 position )
    {
        playerPosition = position;
        isPlayerVisible = true;
        SendMessageUpwards( "ReportIntruder" );
    }

    public void PlayerNotVisible()
    {
        isPlayerVisible = false;
    }

    public void Investigate( Vector3 position )
    {
        playerPosition = position;
        isInvestigating = true;
    }

    public void AskStatus()
    {
        switch ( currentState )
        {
            case State.Chase:
                SendMessage( "VisibleStatus" );
                break;
            case State.Alert:
                SendMessage( "AlertStatus" );
                break;
            case State.FollowUp:
                SendMessage( "FollowUpStatus" );
                break;
            default:
                SendMessage( "IdleStatus" );
                break;
        }
    }
}
//======================================================
