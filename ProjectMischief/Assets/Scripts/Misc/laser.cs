using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour 
{
	void Start () 
    {
        //this.gameObject.SetActive( false );	
	}
	
	void Update () 
    {
	
	}

	void OnCollisionEnter(Collision other)
	{
		if(other.collider.CompareTag("Player"))
		{
			PlayerCheckPoint playerCheckPoint = other.gameObject.GetComponent<PlayerCheckPoint>();
			playerCheckPoint.GoToCheckPoint();
		}
	}

}
