using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour 
{
	void OnCollisionEnter(Collision other)
	{
		if(other.collider.CompareTag("Player"))
		{
			PlayerCheckPoint playerCheckPoint = other.gameObject.GetComponent<PlayerCheckPoint>();
			playerCheckPoint.GoToCheckPoint();
		}
	}

}
