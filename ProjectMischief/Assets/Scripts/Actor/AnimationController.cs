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
public class AnimationController : MonoBehaviour 
{
    //======================================================
    //Private
    //======================================================
    Animation anime;

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
        anime = this.GetComponentInChildren<Animation>();
        state = State.Idle;
    }

    //======================================================

    void Update()
    {
        switch(state)
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
       anime.Play( "idle" );
       print( "ITS FUCKING IDLE" );
    }

    //======================================================

    void UpdateWalk()
    {
        anime.Play( "walk" );
    }

    //======================================================

    void UpdateRun()
    {
        anime.Play( "run" );
    }

    //======================================================

    public void ChangeState(State s)
    {
        if(s != state)
        {
            anime.Stop();
            state = s;
        }
    }

    //======================================================
}
