using UnityEngine;
using System.Collections;

public class GuardSample : MonoBehaviour 
{
	FOV2DEyes eyes;
	FOV2DVisionCone visionCone;

    float timeElapsed = 0.0f;
    float timeBeforeReActivation;
    Transform CamObject;
    GameObject cam;
    CamerSight camera;

    public bool active = true;
	
	void Start() 
	{
		eyes = GetComponentInChildren<FOV2DEyes>();
		visionCone = GetComponentInChildren<FOV2DVisionCone>();

        camera = GetComponentInParent<CamerSight>();
	}
	
	void Update()
	{
		//bool playerInView = false
        if(active)
        {
            foreach( RaycastHit hit in eyes.hits )
            {
                if( hit.transform && hit.transform.tag == "Player" )
                {
                    camera.Caught();
                }
            }
        }
	}
}
