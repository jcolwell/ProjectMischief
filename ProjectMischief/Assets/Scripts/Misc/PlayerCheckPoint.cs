using UnityEngine;
using System.Collections;

public class PlayerCheckPoint : MonoBehaviour 
{
    public Vector3 checkPoint;
    public PlayerCheckPoint instance;

	// Use this for initialization
	void Start () 
	{
        instance = this;
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
