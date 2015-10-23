//======================================================
// File: GuardAI.cs
// Discription:    This Script will drive Guard AI
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
//======================================================


//======================================================
// Class GuardAI
//======================================================
public class GuardAI : MonoBehaviour
{
    //==================================================
    // Private Variables
    //==================================================
    private enum State
    {
        Idle = 0,
        Alert,
        Chase
    }
    private NavMeshAgent agent;
    private State currentState;
    private int wayTarget;
    //private bool seePlayer;

    //==================================================

    //==================================================
    // Public Variables
    //==================================================
    public GameObject[] waypoints;
    //==================================================


    //==================================================

	void Start () 
    {
        agent = GetComponent<NavMeshAgent>();

        wayTarget = 0;
        agent.SetDestination( waypoints[wayTarget].transform.position );
        currentState = State.Idle;
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
        }
	}

    //==================================================

    State Idle()
    {
        //Determine Distance to target
        if( agent.remainingDistance < 1.0f )
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

    State Alert()
    {
        return State.Idle;
    }

    State Chase()
    {
        return State.Idle;
    }
}
//======================================================
