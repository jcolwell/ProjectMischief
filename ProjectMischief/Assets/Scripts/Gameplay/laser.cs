//======================================================
// File: Laser.cs
// Description:    Does Laser Things
//======================================================

//======================================================
// Includes
//======================================================
using UnityEngine;
using System.Collections;
//======================================================


//======================================================
// Class Laser
//======================================================
public class laser : MonoBehaviour 
{
    //======================================================
    // Public
    //======================================================
    //public ParticleSystem mirrorPar = null;
    public GameObject lazerControl;
    public float timePause = 1;
    public float timeActive = 1;
    public string PlayerTag = "Player";
    //======================================================

    //======================================================
    // Private
    //======================================================
    AudioSource sound;
    GameObject player;
    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    bool isActive = true;
    bool isTurn = true;
    bool dispatchCalled = false;
    public GameObject spawn;
    //======================================================


    void OnCollisionEnter( Collision other )
    {
        if( other.collider.CompareTag( PlayerTag ) && lazerControl.activeSelf)
        {
            Dispatch( other );
        }
    }

    //======================================================

    void OnCollisionStay( Collision other )
    {
        if( other.collider.CompareTag( PlayerTag ) && lazerControl.activeSelf && !dispatchCalled )
        {
            Dispatch( other );
        }
    }

    //======================================================

    void Start()
    {
        player = GameObject.Find( "Actor" );

        if( player == null )
        {
            player = GameObject.Find( "Actor(Clone)" );
        }
        
        sound = gameObject.GetComponent<AudioSource>();
    }

    //======================================================

    void Update()
    {
        if( !isActive && timeElapsed >= timeBeforeReActivation )
        {
            isActive = true;
            sound.Stop();
        }

        else if( timeElapsed >= timePause && !isTurn )
        {
            ToggleLazer( true );
            sound.Play();
            isTurn = true;
        }

        else if( timeElapsed >= timeActive && isTurn )
        {
            ToggleLazer( false );
            dispatchCalled = false;
            isTurn = false;
            sound.Stop();
        }

        timeElapsed += Time.deltaTime;
    }

    //======================================================

    public void DeActivate( float tR )
    {
        timeBeforeReActivation = 1;
        timeElapsed = 0.0f;
        isActive = false;
        ToggleLazer( false );
        dispatchCalled = false; ;
    }

    //======================================================

    public void ToggleLazer( bool state )
    {
        if( isActive )
        {
            lazerControl.SetActive( state );
        }

        else
        {
            lazerControl.SetActive( false );
        }

        timeElapsed = 0.0f;
    }

    void Dispatch(Collision other)
    {
        PlayerLife playerLife = other.gameObject.GetComponent<PlayerLife>();
        playerLife.CaughtPlayer(HazardTypes.eLazer, spawn.transform);//, mirrorPar );
        dispatchCalled = true;
    }
}
