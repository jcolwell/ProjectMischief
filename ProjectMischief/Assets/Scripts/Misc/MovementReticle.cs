using UnityEngine;
using System.Collections;

public class MovementReticle : MonoBehaviour 
{
    public float lifeTime;

    static MovementReticle curInstance;
    static bool isRunning = false;

    float timeElapsed = 0;
	
	void Start () 
    {
        if(isRunning)
        {
            if( curInstance != null )
            {
                Destroy( curInstance.gameObject );
                curInstance = null;
            }
        }
        else
        {
            isRunning = true;
        }

        curInstance = this;
        timeElapsed = 0.0f;
	}
	
	void Update () 
    {
        timeElapsed += Time.deltaTime;
        
        if(timeElapsed >= lifeTime)
        {
            isRunning = false;
            Destroy( this.gameObject );
        }
	}

    void OnTriggerEnter( Collider other )
    {
       if(other.CompareTag("Player"))
       {
           isRunning = false;
           Destroy( this.gameObject );
       }
    }

}
