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
        Chase,
        LookAround,
        Wander,
        Sleeping
    }

    private State currentState;

    private NavMeshAgent agent;

    private int wayTarget;
    
    private Vector3 playerPosition;
    
    private Vector3 homePosition;
    private Quaternion homeRotation;


    private AnimController anime;
    private AnimController playerAnime;

    private bool isPlayerVisible = false;
    private bool isInvestigating = false;

    private float regularMoveSpeed = 0.0f;
    private float alertMoveSpeed = 0.0f;

    // wander and look around stat varibles
    private float elapsedSearchTime = 0.0f;
    private float degPerSec = 0.0f;
    private bool wasChasingPlayer = false;
    private float curDeg = 0.0f;
    private int timesLooked = 0;

    private int maxTimesLooked = 2;
    private LayerMask cullingMask;
    private float wanderTimeElpased = 0.0f;

    //==================================================

    //==================================================
    // Public Variables
    //==================================================
    //public ParticleSystem smokeBombEffect;
    public GameObject[] waypoints;
    public float distanceFromWaypoint = 1.0f;
    public float moveSpeedMultiplier = 1.5f;
    public float turnSpeed = 30;
    public float searchForPlayerDuration = 60.0f;
    public float wanderDuration = 5.0f;
    public float lookAroundTime = 10.0f;
    public float degTolook = 85.0f;
    public float wanderTurnSpeed = 30.0f;
    public float wanderViewDist = 1.0f;
    public float wanderStoppingDistance = 0.75f;
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

        VisionCone viewCone = GetComponent<VisionCone>();
        cullingMask = viewCone.cullingMask;
	}

    //==================================================
	
    void Update () 
    {
	    switch( currentState )
        {
            case State.Idle:
                currentState = Idle();
                break;
            case State.Alert:
                currentState = Alert();
                break;
            case State.Chase:
                currentState = Chase();
                break;
            case State.LookAround:
                currentState = LookAround();
                break;
            case State.Wander:
                currentState = Wander();
                break;

            case State.Sleeping:
                currentState = Sleeping();
                break;
        }
        Debug.DrawRay(transform.position, transform.forward * wanderViewDist, Color.blue, 1 / 30, true);
        Debug.DrawRay(transform.position, (-transform.forward) * wanderViewDist, Color.red, 1 / 30, true);
        Debug.DrawRay(transform.position, transform.right * wanderViewDist, Color.white, 1 / 30, true);
        Debug.DrawRay(transform.position, (-transform.right) * wanderViewDist, Color.black, 1 / 30, true);

    }

    //==================================================

    void OnTriggerEnter( Collider col )
    {
        if( col.CompareTag("Player"))
        {
            PlayerLife player = col.transform.parent.GetComponent<PlayerLife>();
            player.CaughtPlayer(HazardTypes.eGaurd, this.transform);//, smokeBombEffect );
            playerAnime.ChangeState( AnimController.State.Idle );
            
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
        if( anime.GetState() != AnimController.State.Walk )
        {
            anime.ChangeState( AnimController.State.Walk );
        }

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
            if(anime.GetState() != AnimController.State.Walk)
            {
                anime.ChangeState( AnimController.State.Walk );
            }

            if( playerAnime.GetState() != AnimController.State.Walk )
            {
                playerAnime.ChangeState( AnimController.State.Walk );
            }
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
        agent.SetDestination( playerPosition );
        
        if( isPlayerVisible )
        {
            returnState = State.Chase;
        }

        if( playerAnime.GetState() == AnimController.State.Walk )
        {
            playerAnime.ChangeState( AnimController.State.Run );
        }

        if( !agent.pathPending  && agent.remainingDistance < agent.stoppingDistance )
        {
            anime.ChangeState( AnimController.State.Walk );
            playerAnime.ChangeState( AnimController.State.Walk );

            isInvestigating = false;

            //returnState = State.FollowUp;
            elapsedSearchTime = 0.0f;
            degPerSec = (degTolook * (maxTimesLooked + 1)) / lookAroundTime;
            curDeg = 0.0f;
            returnState = wasChasingPlayer ? State.Wander : State.LookAround;
        }

        return returnState;
    }

    //==================================================
    // Alert: Player has been seen! Go look for them!

    //==================================================
    // Chase: You see the player! Catch them!
    
    private State Chase()
    {
        //agent.destination = playerPosition;
        agent.SetDestination( playerPosition );

        Quaternion lookRot = Quaternion.LookRotation(playerPosition - transform.position);
        float t = turnSpeed / Quaternion.Angle(transform.rotation, lookRot) * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, t);

        //KIMS FIRST HACK
        if( anime.GetState() != AnimController.State.Run )
        {
            anime.ChangeState( AnimController.State.Run );
        }

        if( playerAnime.GetState() == AnimController.State.Walk )
        {
            playerAnime.ChangeState( AnimController.State.Run );
        }


        if( isPlayerVisible && agent.remainingDistance > 0 )
        {
            return State.Chase;
        }

        isInvestigating = true;
        wasChasingPlayer = true;
        return State.Alert;
    }

    //==================================================

    private State LookAround()
    {
        State returnState = State.LookAround;
        agent.velocity = Vector3.zero;
        if (isPlayerVisible)
        {
            return State.Chase;
        }
        if (isInvestigating)
        {
            return State.Alert;
        }

        elapsedSearchTime += Time.deltaTime;
        curDeg += degPerSec * Time.deltaTime;

        if(curDeg > degTolook || curDeg < (-degTolook) * 2.0f)
        {
            curDeg = 0.0f;
            degPerSec = -degPerSec;
            ++timesLooked;
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + (degPerSec * Time.deltaTime),
            transform.eulerAngles.z);

        if (timesLooked >= maxTimesLooked)
        {
            returnState = wasChasingPlayer ? State.Wander : State.Idle;
            timesLooked = 0;
            curDeg = 0.0f;
        }

        if(returnState == State.Idle)
        {
            SendMessageUpwards("ReportInteruterNeutralized");
        }

        return returnState;
    }

    //==================================================

    private State Wander()
    {
        if (isPlayerVisible)
        {
            wanderTimeElpased = 0.0f;
            agent.velocity = Vector3.zero;
            curDeg = 0.0f;
            return State.Chase;
        }
        if (isInvestigating)
        {
            wanderTimeElpased = 0.0f;
            agent.velocity = Vector3.zero;
            curDeg = 0.0f;
            return State.Alert;
        }

        elapsedSearchTime += Time.deltaTime;
        wanderTimeElpased += Time.deltaTime;

        if (elapsedSearchTime >= searchForPlayerDuration)
        {
            wasChasingPlayer = false;
            wanderTimeElpased = 0.0f;
            agent.velocity = Vector3.zero;
            curDeg = 0.0f;
            SendMessageUpwards("ReportInteruterNeutralized");
            return State.Idle;
        }

         if (wanderTimeElpased > wanderDuration)
         {
             wanderTimeElpased = 0.0f;
             agent.velocity = Vector3.zero;
             curDeg = 0.0f;
             return State.LookAround;
         }

         if(agent.remainingDistance < wanderStoppingDistance)
        {
            DecideNewDir();
        }

        return State.Wander;
    }

    //==================================================

    private State Sleeping()
    {
        return State.Idle;
    }

    //==================================================

    private void DecideNewDir()
    {
        // raycast to check if agent needs to turn or not
        RaycastHit hit = new RaycastHit();

        bool rightBlocked = Physics.Raycast(transform.position, transform.right, out hit, wanderViewDist, cullingMask);
        bool leftBlocked = Physics.Raycast(transform.position, -transform.right, out hit, wanderViewDist, cullingMask);

        if (!Physics.Raycast(transform.position, transform.forward, out hit, wanderViewDist, cullingMask))
        {
            agent.SetDestination(transform.position + (transform.forward * wanderViewDist));
            return;
        }
        else if(flipCoin())
        {
            if(!rightBlocked)
            {
                agent.SetDestination(transform.position + (transform.right * wanderViewDist));
                return;
            }
            else if(!leftBlocked)
            {
                agent.SetDestination(transform.position + (-transform.right * wanderViewDist));
                return;
            }
        }
        else
        {
            if (!leftBlocked)
            {
                agent.SetDestination(transform.position + (-transform.right * wanderViewDist));
                return;
            }
            else if (!rightBlocked)
            {
                agent.SetDestination(transform.position + (transform.right * wanderViewDist));
                return;
            }
        }
        agent.SetDestination(transform.position + (-transform.forward * wanderViewDist));
    }

    //==================================================

    private bool flipCoin()
    {
        return (Random.Range(0, 2) == 1) ? true : false;
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
            case State.LookAround:
            case State.Wander:
                SendMessage( "FollowUpStatus" );
                break;
            default:
                SendMessage( "IdleStatus" );
                break;
        }
    }
}
//======================================================
