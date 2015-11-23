﻿using UnityEngine;
using System.Collections;

public class PlayerCheckPoint : MonoBehaviour 
{
    Vector3 checkPoint;
    //PlayerCheckPoint instance;

	// Use this for initialization
	void Start () 
	{
        //instance = this;
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

}