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
    ParticleSystem mirrorPar = null;
    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    bool active = true;
    bool isTurn = true;
    //======================================================


    void OnCollisionEnter( Collision other )
    {
        if( other.collider.CompareTag( PlayerTag ) && lazerControl.activeSelf )
        {
            Transform lazerObject = gameObject.GetComponent<Transform>();
            PlayerLife playerLife = other.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eLazer, lazerObject, mirrorPar );
        }
    }

    //======================================================

    void OnCollisionStay( Collision other )
    {
        if( other.collider.CompareTag( PlayerTag ) && lazerControl.activeSelf )
        {
            Transform lazerObject = gameObject.GetComponent<Transform>();
            PlayerLife playerLife = other.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eLazer, lazerObject, mirrorPar );
        }
    }

    //======================================================

    void Start()
    {
        mirror.SetActive( false );
    }

    //======================================================

    void Update()
    {
        mirror.transform.Rotate( 0, 0, -1 );
        if( !active && timeElapsed >= timeBeforeReActivation )
        {
            active = true;
            mirror.SetActive( false );
        }

        else if( timeElapsed >= timePause && !isTurn )
        {
            ToggleLazer( true );
            isTurn = true;
        }

        else if( timeElapsed >= timeActive && isTurn )
        {
            ToggleLazer( false );
            isTurn = false;
        }

        timeElapsed += Time.deltaTime;
    }

    //======================================================

    public void DeActivate( float tR )
    {
        timeBeforeReActivation = 1;
        timeElapsed = 0.0f;
        active = false;
        mirror.SetActive( true );
        ToggleLazer( false );
    }

    //======================================================

    public void ToggleLazer( bool state )
    {
        if( active )
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
