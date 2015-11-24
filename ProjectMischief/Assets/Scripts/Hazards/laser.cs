using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour 
{
    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    ParticleSystem mirror;
    bool active = true;

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
        timeElapsed += Time.deltaTime;
    }

    public bool GetActive()
    {
        return active;
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

}
