//======================================================
// File: GuardAI.cs
// Discription:    This Script will drive Guard AI
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
        Sleeping,
    }
    private NavMeshAgent agent;
    private int wayTarget;
    private Vector3 playerPosition;
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
        agent = GetComponent<NavMeshAgent>();

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
            //Harm Player
            Debug.Log("HARM THE PLAYER!");
        }
    }

    //==================================================

    State Idle()
    {
        //Determine Distance to target
        if( agent.remainingDistance < distanceFromWaypoint )
        {
            //Debug.Log( "Next Target" );
            wayTarget = (wayTarget + 1) % waypoints.Length;
          

            //Update destination
            Vector3 tarPos = waypoints[wayTarget].transform.position;
            Vector3 destination = new Vector3( tarPos.x, transform.position.y, tarPos.z );
            
            //Travel to destination
            //agent.destination = destination;
            agent.SetDestination( destination );
        }
        return State.Idle;
    }

    //==================================================

    State Alert()
    {
        return State.Idle;
    }

    //==================================================

    State Chase()
    {
        agent.destination = playerPosition;

        if (agent.remainingDistance > 0)
            return State.Chase;
        
        return State.Idle;
    }

    //==================================================
    
    State Sleeping()
    {
        return State.Idle;
    }

    //==================================================

    public void PlayerVisible( Vector3 position )
    {
        playerPosition = position;
        currentState = Chase();
    }

    public void PlayerNotVisible()
    {
        currentState = Idle();
    }

    public void Investigate( Vector3 position )
    {
        currentState = Alert();
        playerPosition = position;
    }
}
//======================================================
