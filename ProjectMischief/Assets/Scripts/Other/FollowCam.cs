using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

	GameObject target; //view target object
	public Vector3 offSet; // postion reltive to the view target
    public bool caclulateOffsetOnRunTime = true;

	void Start ()
	{
        target = GameObject.Find("Actor");

        if(target == null)
        {
            target = GameObject.Find("Actor(Clone)");
        }

        if (caclulateOffsetOnRunTime)
        {
            offSet = transform.position - target.transform.position;
        }
	}

	void Update () 
	{
        transform.position = target.transform.position + offSet;
	}
}
