using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

	public GameObject target; //view target object
	Vector3 offSet; // postion reltive to the view target

	void Start ()
	{
        target = GameObject.Find("Actor");
        if(target == null)
        {
            target = GameObject.Find("Actor(Clone)");
        }
		offSet = transform.position - target.transform.position;
	}

	void Update () 
	{
        transform.position = target.transform.position + offSet;
	}
}
