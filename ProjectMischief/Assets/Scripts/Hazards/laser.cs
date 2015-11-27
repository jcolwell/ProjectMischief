using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour 
{
    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    public float timePause = 5;
    public float timeActive = 5;
    ParticleSystem mirror;
    bool active = true;
    bool isTurn = true;
    int pauseTime = 2;

    public GameObject lazerControl;

	void OnCollisionEnter(Collision other)
	{
		if(other.collider.CompareTag("Player") && lazerControl.activeSelf)
		{
            Transform lazerObject = gameObject.GetComponent<Transform>();
            PlayerLife playerLife = other.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eLazer, lazerObject, mirror );
		}
	}

    void OnCollisionStay( Collision other )
    {
        if( other.collider.CompareTag( "Player" ) && lazerControl.activeSelf )
        {
            Transform lazerObject = gameObject.GetComponent<Transform>();
            PlayerLife playerLife = other.gameObject.GetComponent<PlayerLife>();
            playerLife.CaughtPlayer( HazardTypes.eLazer, lazerObject, mirror );
        }
    }

    void Update()
    {
        if( !active && timeElapsed >= timeBeforeReActivation )
        {
            active = true;
        }

        else if( timeElapsed >= timeActive )
        {
            if(!isTurn)
            {
                ToggleLazer( true );
                pause( true );
            }

            else
            {
                ToggleLazer(false);
                pause( false );
            }
        }
        else
        {
            timeElapsed += Time.deltaTime;
        }
    }

    public void DeActivate(float timeBeforeReActivation)
    {
        this.timeBeforeReActivation = timeBeforeReActivation;
        timeElapsed = 0.0f;
        active = false;
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
    }


    IEnumerator pause( bool pause )
    {
        yield return 0;
        isTurn = pause;
    }

}
