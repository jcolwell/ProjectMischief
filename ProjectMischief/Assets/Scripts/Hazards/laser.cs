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
    public GameObject mirror;
    public GameObject lazerControl;
    public float timePause = 1;
    public float timeActive = 1;
    public string PlayerTag = "Player";
    //======================================================

    //======================================================
    // Private
    //======================================================
    AudioSource sound;
    ParticleSystem mirrorPar = null;
    GameObject player;
    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    bool isActive = true;
    bool isTurn = true;
    bool dispatchCalled = false;
    //======================================================


    void OnCollisionEnter( Collision other )
    {
        if( other.collider.CompareTag( PlayerTag ) && lazerControl.activeSelf)
        {
            Transform lazerObject = gameObject.GetComponent<Transform>();
            PlayerLife playerLife = other.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eLazer, lazerObject, mirrorPar );
            dispatchCalled = true;
        }
    }

    //======================================================

    void OnCollisionStay( Collision other )
    {
        if( other.collider.CompareTag( PlayerTag ) && lazerControl.activeSelf && !dispatchCalled )
        {
            Transform lazerObject = gameObject.GetComponent<Transform>();
            PlayerLife playerLife = other.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eLazer, lazerObject, mirrorPar );
            dispatchCalled = true;
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

        mirror.SetActive( false );
        sound = gameObject.GetComponent<AudioSource>();
    }

    //======================================================

    void Update()
    {
        if( !isActive && timeElapsed >= timeBeforeReActivation )
        {
            isActive = true;
            mirror.SetActive( false );
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
            mirror.SetActive( false );
            isTurn = false;
            sound.Stop();
        }

        mirror.transform.Rotate( 0, 0, -1 );
        

        timeElapsed += Time.deltaTime;
    }

    //======================================================

    public void DeActivate( float tR )
    {
        timeBeforeReActivation = 1;
        timeElapsed = 0.0f;
        isActive = false;
        mirror.SetActive( true );
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
}
