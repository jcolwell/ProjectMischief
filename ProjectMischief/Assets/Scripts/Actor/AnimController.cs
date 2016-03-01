//======================================================
// File: AnimationController.cs
// Description:    Animation stuffs
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;

//======================================================
public class AnimController : MonoBehaviour
{
    //======================================================
    //Private
    //======================================================
    private Animator anime;

    //======================================================
    //State
    //======================================================
    public enum State
    {
        Idle,
        Walk,
        Run
    };

    public State state;
    State guardState;

    //======================================================

    void Start()
    {
        anime = this.GetComponentInChildren<Animator>();
        state = State.Idle;
    }

    //======================================================

    void UpdateState()
    {
        switch( state )
        {
        case State.Idle:
        {
            UpdateIdle();
        }
        break;
        case State.Walk:
        {
            UpdateWalk();
        }
        break;
        case State.Run:
        {
            UpdateRun();
        }
        break;
        }
    }

    //======================================================

    void UpdateIdle()
    {
        anime.SetTrigger( "idleTrigger" );
    }

    //======================================================

    void UpdateWalk()
    {
        anime.SetTrigger( "walkingTrigger" );
    }

    //======================================================

    void UpdateRun()
    {
        anime.SetTrigger( "alertTrigger" );
    }

    //======================================================

    public void ChangeState( State s )
    {
        state = s;
        if( s != state || true )
        {
            state = s;
            UpdateState();
        }
    }

    //HACKED TO SHIT
    public State GetState()
    {
        return state;
    }

    public void SetGuardState(State s)
    {
        guardState = s;
    }

    public State GetGuardState()
    {
        return guardState;
    }

    //======================================================
}