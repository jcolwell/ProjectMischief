using UnityEngine;
using System.Collections;

public class PlayerCheckPoint : MonoBehaviour 
{
    public Vector3 checkPoint;

	// Use this for initialization
	void Start () 
	{
        checkPoint = gameObject.transform.position;
	}
	
    public void SetCheckPoint(Vector3 postion)
    {
        checkPoint = postion;
    }

    public void GoToCheckPoint()
    {
        gameObject.transform.position = checkPoint;
        Moving.instance.SetTarget( checkPoint );
    }


    void Update ()
    {
        if(Input.GetKeyDown(KeyCode.R) == true)
        {
            GoToCheckPoint();
        }
    }
}
