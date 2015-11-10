using UnityEngine;
using System.Collections;

public class GuardSample : MonoBehaviour 
{
	FOV2DEyes eyes;
	FOV2DVisionCone visionCone;
	//float speed = -5;
	
	void Start() 
	{
		eyes = GetComponentInChildren<FOV2DEyes>();
		visionCone = GetComponentInChildren<FOV2DVisionCone>();
	}
	
	void Update()
	{
		bool playerInView = false;
		
		foreach (RaycastHit hit in eyes.hits)
		{
			if (hit.transform && hit.transform.tag == "Player")
			{
				playerInView = true;
                PlayerCheckPoint playerCheckPoint = hit.collider.gameObject.GetComponent<PlayerCheckPoint>();
                playerCheckPoint.GoToCheckPoint();
			}
		}
		
		if (playerInView)
		{
			visionCone.status = FOV2DVisionCone.Status.Alert;
		}
		else
		{
			visionCone.status = FOV2DVisionCone.Status.Idle;
		}
	}
}
