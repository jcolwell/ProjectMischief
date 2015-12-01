using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour 
{
    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    public float timePause = 1;
    public float timeActive = 1;
    ParticleSystem mirrorPar;
    public GameObject mirror;
    bool active = true;
    bool isTurn = true;

    public GameObject lazerControl;

	void OnCollisionEnter(Collision other)
	{
		if(other.collider.CompareTag("Player") && lazerControl.activeSelf)
		{
            Transform lazerObject = gameObject.GetComponent<Transform>();
            PlayerLife playerLife = other.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eLazer, lazerObject, mirrorPar );
		}
	}

    void OnCollisionStay( Collision other )
    {
        if( other.collider.CompareTag( "Player" ) && lazerControl.activeSelf )
        {
            Transform lazerObject = gameObject.GetComponent<Transform>();
            PlayerLife playerLife = other.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eLazer, lazerObject, mirrorPar );
        }
    }

    void Update()
    {
        mirror.transform.Rotate( 0, 0, -1);
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

    public void DeActivate(float tR)
    {
        timeBeforeReActivation = tR;
        timeElapsed = 0.0f;
        active = false;
        mirror.SetActive( true );
        ToggleLazer( false );
    }

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
