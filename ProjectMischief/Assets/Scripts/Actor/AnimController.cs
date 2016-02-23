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
    Animator anime;

    //======================================================
    //State
    //======================================================
    public enum State
    {
        Idle,
        Walk,
        Run
    };

    State state;

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
        anime.SetBool( "isCaught", false );
    }

    //======================================================

    void UpdateWalk()
    {
        anime.SetTrigger( "walkingTrigger" );
        anime.SetBool( "isCaught", false );
    }

    //======================================================

    void UpdateRun()
    {
        anime.SetBool( "isCaught", true );
    }

    //======================================================

    public void ChangeState( State s )
    {
        if( s != state || true )
        {
            state = s;
            UpdateState();
        }
    }

    //======================================================
}