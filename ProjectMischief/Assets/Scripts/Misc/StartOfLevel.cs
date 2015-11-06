﻿using UnityEngine;
using System.Collections;

public class StartOfLevel : MonoBehaviour 
{

    public GameObject player;

	// Use this for initialization
	void Awake () 
    {
        if (player != null)
        {
            player.transform.position = transform.position;
        }
        Destroy(gameObject);
	}
	
}
