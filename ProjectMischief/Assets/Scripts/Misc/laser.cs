using UnityEngine;
using System.Collections;

public class laser : MonoBehaviour 
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
