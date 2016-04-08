using UnityEngine;
using System.Collections;

public class MovementReticle : MonoBehaviour 
{
    public float lifeTime;

    float timeElapsed = 0;

    public void Reset()
    {
        timeElapsed = 0.0f;
    }

    public void Reset(Vector3 pos)
    {
        timeElapsed = 0.0f;
        gameObject.transform.position = pos;
    }

	void Start () 
    {
        timeElapsed = 0.0f;
	}
	
	void Update () 
    {
        timeElapsed += Time.deltaTime;
        
        if(timeElapsed >= lifeTime)
        {
            gameObject.SetActive(false);
        }
	}

    void OnTriggerEnter( Collider other )
    {
       if(other.CompareTag("Player"))
       {
           gameObject.SetActive(false);
       }
    }

}
