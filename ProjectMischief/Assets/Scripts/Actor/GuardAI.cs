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
    private NavMeshAgent agent;
    private VisionCone vision;

    private int wayTarget;
    
    private Vector3 playerPosition;
    
    private Vector3 homePosition;
    private Quaternion homeRotation;

    private ParticleSystem smokeBombEffect;
    
    private bool isPlayerVisible = false;
    private bool isInvestigating = false;
    private bool isTargetingWall = false;
    //==================================================

    //==================================================
    // Public Variables
    //==================================================
    public GameObject[] waypoints;
    public float distanceFromWaypoint = 1.0f;
    public State currentState;    
    //==================================================


    //==================================================

	void Start () 
    {
        homePosition = gameObject.transform.position;
        homeRotation = gameObject.transform.rotation;

        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<VisionCone>();

        smokeBombEffect = GetComponentInChildren<ParticleSystem>();
        smokeBombEffect.Pause();

        wayTarget = 0;
        agent.SetDestination( waypoints[wayTarget].transform.position );
        currentState = State.Idle;

        playerPosition = new Vector3();
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
            PlayerLife player = col.GetComponent<PlayerLife>();
            player.CaughtPlayer( HazardTypes.eGaurd, this.transform, smokeBombEffect );
            
            isPlayerVisible = false;
            isInvestigating = false;
            currentState = State.Idle;

            agent.Warp( homePosition );
            gameObject.transform.rotation = homeRotation;
        }
    }

    //==================================================
    // Idle: Not much is going on... Just follow your route

    private State Idle()
    {

        if( isPlayerVisible )
        {
            return State.Chase;
        }
        if( isInvestigating )
        {
            return State.Alert;
        }
        //Determine Distance to target
        //Debug.Log( agent.remainingDistance );
        if( ! ( agent.pathPending || agent.remainingDistance > distanceFromWaypoint) )
        {
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
        agent.destination = playerPosition;
        if( isPlayerVisible )
        {
            return State.Chase;
        }
        if( !agent.pathPending  && agent.remainingDistance < agent.stoppingDistance )
        {
            isInvestigating = false;
            return State.FollowUp;
        }
        return State.Alert;
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
                return State.Idle;
            }
        }

        agent.destination = playerPosition;
        Debug.Log( agent.remainingDistance );
        if( !(agent.remainingDistance > agent.stoppingDistance) )
        {
            //PAN LEFT RIGHT
            isTargetingWall = false;
            return State.Idle;
        }
        return State.FollowUp;
    }


    //==================================================
    // Chase: You see the player! Catch them!
    
    private State Chase()
    {
        agent.destination = playerPosition;

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
