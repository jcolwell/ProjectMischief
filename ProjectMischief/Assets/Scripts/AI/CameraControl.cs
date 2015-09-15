using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	RaycastHit hit;
	bool leftClickFlag = true;
	
	public GameObject actor;
	public string floorTag;
	public string PictureTag;

	Actor actorScript;

	void Start()
	{
		if (actor != null)
		{
			actorScript = (Actor)actor.GetComponent(typeof(Actor));
		}
	}
	
	void Update () 
	{
		/***Left Click****/
		if (Input.GetKey(KeyCode.Mouse0) && leftClickFlag)
			leftClickFlag = false;
		
		if (!Input.GetKey(KeyCode.Mouse0) && !leftClickFlag)
		{
			leftClickFlag = true;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 100))
			{
				if (hit.transform.tag == floorTag)
				{
					float X = hit.point.x;
					float Z = hit.point.z;
					Vector3 target = new Vector3(X, actor.transform.position.y, Z);
					
					actorScript.MoveOrder(target);
				}

				if (hit.transform.tag == PictureTag)
				{
					Debug.Log("I GOD DAMN PRESSED IT");
					ArtPiece art = hit.collider.gameObject.GetComponent<ArtPiece>();
					if(art.playerIsInRange == true)
					{
						art.LoadMenu();
                        PlayerCheckPoint playerCheckPoint = actor.GetComponent<PlayerCheckPoint>();
                        if(playerCheckPoint != null)
                        {
                            playerCheckPoint.SetCheckPoint(actor.transform.position);
                        }
					}
				}
			}
		}
	}
}
